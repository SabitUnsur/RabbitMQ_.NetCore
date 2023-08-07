using ClosedXML.Excel;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FileCreateWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQClientService _rabbitmqClientService;
        private readonly IServiceProvider _serviceProvider;
        //AdventureContext scoped olarak eklendiği için (program.csde)
        //ve worker service da singleton olduğundan DI ile direkt alamıyoruz

        private IModel _channel;


        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitmqClientService, IServiceProvider serviceProvider, IModel channel)
        {
            _logger = logger;
            _rabbitmqClientService = rabbitmqClientService;
            _serviceProvider = serviceProvider;
            _channel = channel;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitmqClientService.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            return Task.CompletedTask;

        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);

            var createExcelMessage = JsonSerializer.Deserialize<CreateExcelMessage>
                (Encoding.UTF8.GetString(@event.Body.ToArray()));
            //Rabbitmqdaki mesajı aldık.

            using var ms = new MemoryStream();

            var workBook = new XLWorkbook();
            var dataSet = new DataSet();
            dataSet.Tables.Add(GetTable("Products"));

            workBook.Worksheets.Add(dataSet);
            workBook.SaveAs(ms); //Memorye olusturulan excel dosyasını kaydettik

            //FilesController içerisinde Upload metoduna gidecek parametreler hazırlanıyor

            MultipartFormDataContent multipartFormDataContent = new();

            //"file" burda filescontroller içine gidecek parametre adı ile aynı 
            multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()), "file",
                Guid.NewGuid().ToString() + ".xlsx");

            var baseUrl = "https://localhost:44382/api/files";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync($"{baseUrl}?fileId={createExcelMessage.FileId}"
                    ,multipartFormDataContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"File (Id:{createExcelMessage.FileId} created successfully)");
                    _channel.BasicAck(@event.DeliveryTag, false);
                }
            }    
        }


        //Memoryde table olusturduk
        private DataTable GetTable(string tableName)
        {
            List<DimProduct> products;

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AdventureWorksDW2019Context>();

                products = context.DimProducts.ToList();
            }

            DataTable dataTable = new() { TableName = tableName };
            dataTable.Columns.Add("ProductId", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("ProductNumber", typeof(string));
            dataTable.Columns.Add("SafetyStockLevel", typeof(int));

            products.ForEach(x =>
            {
                dataTable.Rows.Add(x.ProductId, x.EnglishProductName, x.SafetyStockLevel);
            });

            return dataTable;
        }
    }
}

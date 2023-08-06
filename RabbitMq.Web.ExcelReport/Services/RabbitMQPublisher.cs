using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

namespace RabbitMq.Web.ExcelReport.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitmqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService)
        {
            _rabbitmqClientService = rabbitmqClientService;
        }

        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel = _rabbitmqClientService.Connect();

            var bodyString = JsonSerializer.Serialize(createExcelMessage);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties(); //Memoryde durmasın fiziki kaydolsun diye.
            properties.Persistent = true;

            channel.BasicPublish
                (exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingExcel,
                basicProperties: properties, body: bodyByte);

        }
    }
}


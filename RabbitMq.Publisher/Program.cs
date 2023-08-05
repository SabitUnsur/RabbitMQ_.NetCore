
using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

public enum LogNames
{
    Critical=1,
    Error = 2,
    Warning=3,
    Information=4,
}

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqps://fqmwwsui:Zw_5VI0UFBBoPPLecKJbiE-oENX1WTJu@chimpanzee.rmq.cloudamqp.com/fqmwwsu");

        //Url Bağlantısı

        using var connection = factory.CreateConnection(); //RabbitMq bağlantısı açılır.

        var channel = connection.CreateModel(); //Kanal oluşturulur, bu kanal üzerinden rabbitmq ile haberleşilir.  

        //channel.QueueDeclare("hello-queue", true, false, false);

        //=> Fanout Exchange için kuyrukların consumerlar tarafından oluşturulmasını istedik ve kaldırdık.

        //durable: queue memoryde tutulur rabbitmq restartında tüm kuyruk gider, true derse fiziksel kaydedilir kaybolmaz.
        //exclusive: true yaparsak sadece burada oluşturduğumuz channel üzerinden bağlanabiliriz, biz farklı kanallardan yapacağımız için false dedik
        //autoDelete: Son subscriber da silinirse otomatik kuyruk da silinir.

        //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout); //Fanout Exchange tanımı

        //channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct); //Direct Exchange tanımı

       // channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Direct); //Topic Exchange tanımı


        channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers); //Header Exchange tanımı

        Dictionary<string,object> headers = new Dictionary<string, object>();
        headers.Add("format", "pdf");
        headers.Add("shape", "a4");

        var properties = channel.CreateBasicProperties();
        properties.Headers = headers;
        properties.Persistent = true;
        //kuyruklarda durable true yapınca kuyruk silinmiyordu, bu kod mesajların da silinmemesini sağlar.

        var product = new Product { Id = 1, Name = "Kalem", Price = 10, Stock = 100 }; 
        var productJsonString= JsonSerializer.Serialize(product);

        channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));
        //Complex types

        // channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("Header Message"));

        Console.WriteLine("Mesaj gönderildi");

        //Direct Exchange
        /* Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
         {      var routeKey = $"route-{x}"; => Direct Exchange
              var queueName = $"direct-queue-{x}"; 
             channel.QueueDeclare(queueName, true, false, false); //Direct Exchange için yazıldı.
             channel.QueueBind(queueName, "logs-direct", routeKey);
         });*/


      /*  Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            //LogNames log = (LogNames)new Random().Next(1,5); => direct exchange  

            LogNames log1 = (LogNames)new Random().Next(1, 5); => topic exchange
            LogNames log2 = (LogNames)new Random().Next(1, 5);
            LogNames log3 = (LogNames)new Random().Next(1, 5);

            var routeKey = $"{log1}.{log2}.{log3}";

            string message = $"log {log1} - {log2} - {log3}"; 

            var messageBody = Encoding.UTF8.GetBytes(message); //rabbitmqya mesajlar byte dizin olarak gider.

            //channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

            //Exchange kullanmadan direkt kuyruğa atacaksak default exchange adı verilir. Kuyruk ismi girilmesi gerekir.

            // channel.BasicPublish("log-fanout", " ", null, messageBody);

            //var routeKey = $"route-{log}"; //Direct Exchange

            //channel.BasicPublish("logs-direct",routeKey, null, messageBody); Direct Exchange

            channel.BasicPublish("logs-topic",routeKey, null, messageBody); //Topic Exchange

            Console.WriteLine($"Message has been sent : {message}");
        });

        //Liste al ve 1 den 50 ye dön ve rabbitmqye gönder.

        Console.ReadLine();
      */
    }
}

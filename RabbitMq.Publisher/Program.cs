
using RabbitMQ.Client;
using System.Text;

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
        factory.Uri = new Uri("amqps://tegedsqq:ncxR4Qvi_wkErwDukEx0cFuvBEWJCiA_@moose.rmq.cloudamqp.com/tegedsqq");

        //Url Bağlantısı

        using var connection = factory.CreateConnection(); //RabbitMq bağlantısı açılır.

        var channel = connection.CreateModel(); //Kanal oluşturulur, bu kanal üzerinden rabbitmq ile haberleşilir.  

        //channel.QueueDeclare("hello-queue", true, false, false);

        //=> Fanout Exchange için kuyrukların consumerlar tarafından oluşturulmasını istedik ve kaldırdık.

        //durable: queue memoryde tutulur rabbitmq restartında tüm kuyruk gider, true derse fiziksel kaydedilir kaybolmaz.
        //exclusive: true yaparsak sadece burada oluşturduğumuz channel üzerinden bağlanabiliriz, biz farklı kanallardan yapacağımız için false dedik
        //autoDelete: Son subscriber da silinirse otomatik kuyruk da silinir.

        //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout); //Fanout Exchange tanımı

        channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct); //Direct Exchange tanımı

        //Direct Exchange
        Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
        {
            var routeKey = $"route-{x}";
            var queueName = $"direct-queue-{x}";
            channel.QueueDeclare(queueName, true, false, false);
            channel.QueueBind(queueName, "logs-direct", routeKey);
        });


        Enumerable.Range(1, 50).ToList().ForEach(x =>
        {
            LogNames log = (LogNames)new Random().Next(1,5);

            string message = $"log {log}"; 

            var messageBody = Encoding.UTF8.GetBytes(message); //rabbitmqya mesajlar byte dizin olarak gider.

            //channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

            //Exchange kullanmadan direkt kuyruğa atacaksak default exchange adı verilir. Kuyruk ismi girilmesi gerekir.

            // channel.BasicPublish("log-fanout", " ", null, messageBody);

            var routeKey = $"route-{log}"; //Direct Exchange

            channel.BasicPublish("logs-direct",routeKey, null, messageBody);

            Console.WriteLine($"Message has been sent : {message}");
        });

        //Liste al ve 1 den 50 ye dön ve rabbitmqye gönder.

        Console.ReadLine();

    }
}

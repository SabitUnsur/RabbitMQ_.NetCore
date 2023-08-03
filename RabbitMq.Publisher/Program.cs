
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://tegedsqq:ncxR4Qvi_wkErwDukEx0cFuvBEWJCiA_@moose.rmq.cloudamqp.com/tegedsqq");

//Url Bağlantısı

using var connection = factory.CreateConnection(); //RabbitMq bağlantısı açılır.

var channel = connection.CreateModel(); //Kanal oluşturulur, bu kanal üzerinden rabbitmq ile haberleşilir.  

channel.QueueDeclare("hello-queue", true, false, false);

//durable: queue memoryde tutulur rabbitmq restartında tüm kuyruk gider, true derse fiziksel kaydedilir kaybolmaz.
//exclusive: true yaparsak sadece burada oluşturduğumuz channel üzerinden bağlanabiliriz, biz farklı kanallardan yapacağımız için false dedik
//autoDelete: Son subscriber da silinirse otomatik kuyruk da silinir.

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    string message = $"Messsage {x}";

    var messageBody = Encoding.UTF8.GetBytes(message); //rabbitmqya mesajlar byte dizin olarak gider.

    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);
    //Exchange kullanmadan direkt kuyruğa atacaksak default exchange adı verilir. Kuyruk ismi girilmesi gerekir.

    Console.WriteLine($"Message has been sent : {message}");
});

//Liste al ve 1 den 50 ye dön ve rabbitmqye gönder.



Console.ReadLine();
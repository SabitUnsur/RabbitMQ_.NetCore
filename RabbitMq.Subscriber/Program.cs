
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();

factory.Uri = new Uri("amqps://tegedsqq:ncxR4Qvi_wkErwDukEx0cFuvBEWJCiA_@moose.rmq.cloudamqp.com/tegedsqq");

using var connection = factory.CreateConnection(); 

var channel = connection.CreateModel();  

//channel.QueueDeclare("hello-queue", true, false, false);
//Publisherda bu kuyruğun olduğuna eminsen bu kodu silebilirsin,varsa da parametreler aynı olmalıdır.

var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume("hello-queue", true , consumer);

//AutoAck: true denince RabbitMQ dan bir mesaj gönderildiğinde doğru da işlense yanlış da işlense kuyruktan silinir.
// false denirse eğer doğru işlenirse kuyruktan sil, haberdar edeceğim anlamına gelir.

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine("Gelen Mesaj : " + message);
};

//RabbitMq, subscribera mesaj gönderdiğinde bu event fırlar


Console.ReadLine();
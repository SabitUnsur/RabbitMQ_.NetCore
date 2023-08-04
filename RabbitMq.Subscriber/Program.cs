
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();

factory.Uri = new Uri("amqps://tegedsqq:ncxR4Qvi_wkErwDukEx0cFuvBEWJCiA_@moose.rmq.cloudamqp.com/tegedsqq");

using var connection = factory.CreateConnection(); 

var channel = connection.CreateModel();

//channel.QueueDeclare("hello-queue", true, false, false);
//Publisherda bu kuyruğun olduğuna eminsen bu kodu silebilirsin,varsa da parametreler aynı olmalıdır.

//channel.BasicQos(0, 1, false);

// burada herhangi bir mesaj boyutu olarak ilk parametreyi girdik.
// 2.parametre burada kaç mesaj olacağı. 
// global parametresinin false değeri kaç subsscriber varsa hepsine 2.parametredeki mesaj gönder demektir.
// 2.parametrede 6 var diyelim , true ise 3 subscriber varsa 2-2-2 gönder yani totalde 6 mesaj gönder demektir. Eşit bölmeye çalışır.


//var consumer = new EventingBasicConsumer(channel);

//channel.BasicConsume("hello-queue", true , consumer);

//AutoAck: true denince RabbitMQ dan bir mesaj gönderildiğinde doğru da işlense yanlış da işlense kuyruktan silinir.
// false denirse eğer doğru işlenirse kuyruktan sil, haberdar edeceğim anlamına gelir.

//channel.BasicConsume("hello-queue", false , consumer); => aynı ayarla subscriber ve consumer tarafına yazarsak hata vermez.

//channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);


//var randomQueueName = channel.QueueDeclare().QueueName; Fanout Exchange
//Burada her seferinde rastgele kuyruk oluşturur ve uygulama kapanınca kuyruk silinir

//channel.QueueDeclare deseydi uygulama kapanınca da silinmezdi.

//channel.QueueBind(randomQueueName, "logs-fanout", "", null); Fanout exchange


channel.BasicQos(0, 1, false);
var consumer = new EventingBasicConsumer(channel);

//channel.BasicConsume(randomQueueName, false, consumer);Fanout exchange

//var queueName = "direct-queue-Critical"; // hangi kuyruğa bind olacağımızı belirttik // Direct Exchange

var queueName = channel.QueueDeclare().QueueName;

//var routeKey = "*.Error.*"; //bu regex ifadeye uygun bir mesaj geldiğinde kuyruğa gelsin => Topic Exchange

//channel.QueueBind(queueName, "logs-direct", routeKey); //direct exchange

Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("x-match", "all"); //all dersek mutlaka key-value çiftleri eşleşmesi gerekir.
//any der isek publisherdan herhangi bir ismin burada olması yeterli.
//Örnek "format","pdf" ikisi de publisherdaki ile aynı olmak yerine "format2","pdf" de kabul edilir any için.


//channel.QueueBind(queueName, "logs-topic", routeKey); topic exchange
channel.QueueBind(queueName, "header-exchange", string.Empty, headers);
channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Loglar dinleniyor...");

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine("Gelen Mesaj : " + message);

    //File.AppendAllText("log-critical.txt",message+"\n"); logları txte yazdık.

    Thread.Sleep(1000);

    channel.BasicAck(e.DeliveryTag, false); 
    //Bu kod ile ilgili mesajı silebileceğini rabbitmqya haber ederiz.
    //Rabbitmq hangi tag ile ulaştırmışsa bulur siler.
    //multiple özelliği ise true ise memoryde işlenmiş ama rabbitmqye gitmemiş başka mesaj varsa onları da rabbitmqya bildirir.  

};

//RabbitMq, subscribera mesaj gönderdiğinde bu event fırlar


Console.ReadLine();
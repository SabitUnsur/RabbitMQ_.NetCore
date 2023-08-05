namespace RabbitMqWeb.WaterMark.Services
{
    //RabbitMQ'ya gidecek event class
    public class ProductImageCreatedEvent
    {
        public string ImageName{ get; set; }
    }
}

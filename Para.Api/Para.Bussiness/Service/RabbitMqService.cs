using RabbitMQ.Client;
using System.Text;
using Base.Interfaces;
using IModel = RabbitMQ.Client.IModel;
using Newtonsoft.Json;
using Para.Base.Model;

namespace Business
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqService()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "admin"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "email_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public async Task Publish(EmailMessage message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            _channel.BasicPublish(exchange: "",
                routingKey: "email_queue",
                basicProperties: null,
                body: body);
        }
    }
}

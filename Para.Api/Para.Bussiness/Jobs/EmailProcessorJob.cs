using System.Text;
using Base.Interfaces;
using Newtonsoft.Json;
using Para.Base.Model;
using Para.Bussiness.Notification;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Para.Bussiness.Job
{
    public class EmailProcessorJob
    {
        private readonly INotificationService notificationService;
        private readonly IRabbitMqService rabbitMQService;

        public EmailProcessorJob(INotificationService notificationService, IRabbitMqService rabbitMQService)
        {
            this.notificationService = notificationService;
            this.rabbitMQService = rabbitMQService;
        }

        public void ProcessEmailQueue()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin"
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(message);
                    notificationService.SendEmail(emailMessage.Email, emailMessage.Subject, emailMessage.Body);
                };

                channel.BasicConsume(queue: "email_queue", autoAck: true, consumer: consumer);
            }
        }
    }

}
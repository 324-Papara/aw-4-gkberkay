using Para.Base.Model;

namespace Base.Interfaces
{
    public interface IRabbitMqService
    {
        Task Publish(EmailMessage message);
    }
}

using RabbitMQ.QueueCommunication.Model;

namespace MessageConsumer.Services
{
    public interface IDataHandler
    {
        void HandleData(TimeStampDto data);
    }
}
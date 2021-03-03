using MessageConsumer.Services;
using RabbitMQ.QueueCommunication.MessageQueue;

namespace MessageConsumer
{
    public interface ISetup
    {
        void SetupConsumer();
    }

    public class Setup : ISetup
    {
        private readonly IQueue _queue;
        private readonly IDataHandler _handler;

        public Setup(IQueue queue, IDataHandler handler)
        {
            _queue = queue;
            _handler = handler;
        }

        public void SetupConsumer()
        {
            _queue.MessageReceived += (s, e) => _handler.HandleData(e.Data);
            _queue.ReceiveData();
        }
    }
}
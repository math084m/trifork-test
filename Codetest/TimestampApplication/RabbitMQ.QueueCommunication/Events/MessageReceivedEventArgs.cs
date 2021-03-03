using RabbitMQ.QueueCommunication.Model;

namespace RabbitMQ.QueueCommunication.Events
{
    public class MessageReceivedEventArgs
    {
        public MessageReceivedEventArgs(TimeStampDto data)
        {
            Data = data; }
        public TimeStampDto Data { get; }
    }
}
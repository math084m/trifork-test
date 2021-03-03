using MessageConsumer.Model;

namespace MessageConsumer.Repo
{
    public interface IMessageRepository
    {
        void Add(Message message);
    }
}
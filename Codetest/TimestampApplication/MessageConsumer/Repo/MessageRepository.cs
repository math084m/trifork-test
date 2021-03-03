using System;
using System.Threading.Tasks;
using MessageConsumer.Model;
using Serilog;

namespace MessageConsumer.Repo
{
    //https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/#avoiding-dbcontext-threading-issues
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageDataContext _context;

        public MessageRepository(MessageDataContext context)
        {
            _context = context;
        }

        public void Add(Message message)
        {
            if (message == null)
            {
                throw new ArgumentException($"{nameof(Add)} entity must not be null");
            }

            try
            {
                _context.Add(message);
                Log.Information("Data added to database, trying to save changes.");
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"{nameof(message)} could not be saved: {e.Message}");
            }
        }
    }
}
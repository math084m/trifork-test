using System;
using System.Threading.Tasks;
using MessageConsumer.Model;
using MessageConsumer.Repo;
using RabbitMQ.QueueCommunication.MessageQueue;
using RabbitMQ.QueueCommunication.Model;
using Serilog;
using Utilities.UnixEpoch;

namespace MessageConsumer.Services
{
    public class DataHandler : IDataHandler
    {
        private readonly IQueue _queue;
        private readonly IUnixTimeStamp _timeStamp;
        private readonly IMessageRepository _repository;

        public DataHandler(IQueue queue, IUnixTimeStamp timeStamp, IMessageRepository repository)
        {
            _queue = queue;
            _timeStamp = timeStamp;
            _repository = repository;
        }

        public void HandleData(TimeStampDto data)
        {
            var tempStamp = _timeStamp.GetUnixEpochNow();

            //Over 1 min gammel = discard
            if (data.TimeStamp + 60 > tempStamp)
            {
                try
                {
                    //Even = persistering
                    if (IsEven(data.TimeStamp))
                    {
                        //Brug factories til det her.
                        Log.Information($"TimeStamp {data.TimeStamp} is even and therefore placed in database.");
                        _repository.Add(new Message
                        {
                            Data = "This is a dummy data",
                            TimeStamp = data.TimeStamp
                        });
                    }
                    //Odds = rebase i kø med ny data
                    else
                    {
                        var newStamp = _timeStamp.GetUnixEpochNow();
                        _queue.Send(new TimeStampDto
                        {
                            TimeStamp = newStamp
                        });
                        Log.Information($"TimeStamp: {data.TimeStamp} is odd and therefore placed back in the queue with new timestamp: {newStamp}.");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    throw;
                }
            }
            else
            {
                Log.Information($"Timestamp: {data.TimeStamp} is removed because it's to old.");
            }
        }

        //Helperfunction til at tjekke om tal er lige.
        private bool IsEven(Int32 value)
        {
            return (value % 2 == 0);
        }
    }
    

}
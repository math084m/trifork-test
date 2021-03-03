using System;
using System.Text;
using System.Threading;
using RabbitMQ.QueueCommunication.MessageQueue;
using RabbitMQ.QueueCommunication.Model;
using Serilog;
using Utilities.UnixEpoch;

namespace MessageProducer
{
    class Program
    {

        private static IQueue _queue;
        private static IUnixTimeStamp _timeStamp;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            _timeStamp = new UnixTimeStamp();

            _queue = new Queue(new QueueOptions
            {
                Channel = Environment.GetEnvironmentVariable("Channel"),
                ConnectionHostName = Environment.GetEnvironmentVariable("ConnectionHostName"),
                ConnectionPort = int.Parse(Environment.GetEnvironmentVariable("ConnectionPort"))
            });

            while (true)
            {
                var timeStamp = _timeStamp.GetUnixEpochNow();
                Log.Information(timeStamp.ToString());
                _queue.Send(new TimeStampDto
                {
                    TimeStamp = timeStamp
                });
                Thread.Sleep(1000);
            }
        }
    }
}

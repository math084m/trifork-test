using System;
using MessageConsumer.Repo;
using MessageConsumer.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.QueueCommunication.MessageQueue;
using Serilog;
using Utilities.UnixEpoch;

namespace MessageConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();

            var sc = new ServiceCollection();

            sc.AddDbContext<MessageDataContext>();

            sc.AddSingleton<IQueueOptions>(new QueueOptions
            {
                Channel = Environment.GetEnvironmentVariable("Channel"),
                ConnectionHostName = Environment.GetEnvironmentVariable("ConnectionHostName"),
                ConnectionPort = int.Parse(Environment.GetEnvironmentVariable("ConnectionPort"))
            });

            sc.AddTransient<IQueue, Queue>();
            sc.AddTransient<IUnixTimeStamp, UnixTimeStamp>();
            sc.AddTransient<IMessageRepository, MessageRepository>();
            sc.AddTransient<IDataHandler, DataHandler>();
            sc.AddTransient<ISetup, Setup>();

            var serviceProvider = sc.BuildServiceProvider();

            serviceProvider.GetRequiredService<ISetup>().SetupConsumer();

            Console.ReadLine();
            
        }
    }
}

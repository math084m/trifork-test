using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.QueueCommunication.Events;
using RabbitMQ.QueueCommunication.Model;
using Serilog;

namespace RabbitMQ.QueueCommunication.MessageQueue
{

    /*Man kunne med fordel gøre dette generisk, så det nemmere kan genbruges med andre modeller.
     Jeg havde ikke helt tiden til at gøre det denne gang.*/

    public interface IQueue
    {
        void Send(TimeStampDto data);
        void ReceiveData();
        event MessageReceivedEventHandler MessageReceived;
    }

    public class Queue : IQueue
    {
        private readonly ConnectionFactory _factory;
        private readonly IQueueOptions _options;
        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

        public Queue(IQueueOptions options)
        {
            _options = options;
            _factory = new ConnectionFactory { HostName = _options.ConnectionHostName, Port = _options.ConnectionPort };
            Log.Information($"Connecting to queue with host name: {_options.ConnectionHostName}, port: {_options.ConnectionPort}, and channel: {_options.Channel}");
        }

        public void Send(TimeStampDto data)
        {
            using(var connection = _factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _options.Channel,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = JsonSerializer.SerializeToUtf8Bytes<TimeStampDto>(data);

                channel.BasicPublish(exchange:"",
                    routingKey: _options.Channel,
                    basicProperties: null,
                    body: body);
            }
        }

        public void ReceiveData()
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _options.Channel,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var data = JsonSerializer.Deserialize<TimeStampDto>(body.Span);
                    Log.Information($"Received data: {data.TimeStamp}");
                    
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(data));
                };

                channel.BasicConsume(queue: _options.Channel,
                    autoAck: true,
                    consumer: consumer);

                //This is only because the event cant trigger, if we go out of scope. 
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        
                    }
                });
                Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
                _closing.WaitOne();

            }
        }

        public event MessageReceivedEventHandler MessageReceived;

        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exit");
            _closing.Set();
        }
    }

    public interface IQueueOptions
    {
        string ConnectionHostName { get; set; }
        int ConnectionPort { get; set; }
        string Channel { get; set; }
    }

    public class QueueOptions : IQueueOptions
    {
        public string ConnectionHostName { get; set; }
        public int ConnectionPort { get; set; }
        public string Channel { get; set; }
    }
}
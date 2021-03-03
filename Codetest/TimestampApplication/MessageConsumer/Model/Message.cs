using System;

namespace MessageConsumer.Model
{
    public class Message
    {
        public int MessageId { get; set; }
        public Int32 TimeStamp { get; set; }
        public string Data { get; set; }
    }
}
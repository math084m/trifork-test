using System;

namespace RabbitMQ.QueueCommunication.Model
{
    public interface ITimeStampDto
    {
        Int32 TimeStamp { get; set; }
    }
    public class TimeStampDto : ITimeStampDto
    {
        public int TimeStamp { get; set; }  
    }
}
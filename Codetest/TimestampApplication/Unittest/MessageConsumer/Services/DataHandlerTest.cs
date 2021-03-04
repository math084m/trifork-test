using System;
using MessageConsumer.Model;
using MessageConsumer.Repo;
using MessageConsumer.Services;
using Moq;
using NUnit.Framework;
using RabbitMQ.QueueCommunication.MessageQueue;
using RabbitMQ.QueueCommunication.Model;
using Utilities.UnixEpoch;

namespace UnitTest.MessageConsumer.Services
{
    [TestFixture]
    public class DataHandlerTest
    {
        [SetUp]
        public void SetupTestFixture()
        {
            //Kunne være brugt, hvis der var noget fælles opsætning for de forskellige testcases.
        }

        [Test]
        public void Should_Do_Nothing_When_TimeStamp_Is_OlderThan60Seconds()
        {
            //Arrange
            var queue = new Mock<IQueue>();
            var timeStamp = new Mock<IUnixTimeStamp>();
            timeStamp.Setup(p => p.GetUnixEpochNow()).Returns(CreateTestTimeStamp());
            var repository = new Mock<IMessageRepository>();

            var uut = new DataHandler(queue.Object, timeStamp.Object, repository.Object);

            var data = new TimeStampDto
            {
                TimeStamp = CreateOldTimeStamp()
            };

            //Act
            uut.HandleData(data);

            //Assert
            repository.Verify(s => s.Add(It.IsAny<Message>()), Times.Never);
            queue.Verify(s => s.Send(It.IsAny<TimeStampDto>()), Times.Never);
            timeStamp.Verify(s => s.GetUnixEpochNow(), Times.Once);
        }


        [Test]
        public void Should_Add_Data_To_Database_When_TimeStamp_EvenAndLessThan60Seconds()
        {
            //Arrange
            var queue = new Mock<IQueue>();
            var timeStamp = new Mock<IUnixTimeStamp>();
            timeStamp.Setup(p => p.GetUnixEpochNow()).Returns(CreateTestTimeStamp());
            var repository = new Mock<IMessageRepository>();

            var uut = new DataHandler(queue.Object, timeStamp.Object, repository.Object);

            var data = new TimeStampDto
            {
                TimeStamp = CreateEvenTimeStamp()
            };

            //Act
            uut.HandleData(data);

            //Assert
            repository.Verify(s=>s.Add(It.IsAny<Message>()),Times.Once);
        }

        [Test]
        public void Should_Add_Data_Back_To_Queue_When_TimeStamp_OddsAndLessThan60Seconds()
        {
            //Arrange
            var queue = new Mock<IQueue>();
            var timeStamp = new Mock<IUnixTimeStamp>();
            timeStamp.Setup(p => p.GetUnixEpochNow()).Returns(CreateTestTimeStamp());
            var repository = new Mock<IMessageRepository>();

            var uut = new DataHandler(queue.Object, timeStamp.Object, repository.Object);

            var data = new TimeStampDto
            {
                TimeStamp = CreateOddTimeStamp()
            };

            //Act
            uut.HandleData(data);

            //Assert
            queue.Verify(s=>s.Send(It.IsAny<TimeStampDto>()),Times.Once);
        }

        #region TimeStampHelpers

        private int CreateEvenTimeStamp()
        {
            var timestamp = CreateTestTimeStamp();

            if (timestamp % 2 == 0)
            {
                return timestamp;
            }

            return timestamp - 1;

        }

        private int CreateOddTimeStamp()
        {
            var timestamp = CreateTestTimeStamp();

            if (timestamp % 2 != 0)
            {
                return timestamp;
            }

            return timestamp - 1;
        }

        private int CreateOldTimeStamp()
        {
            return (CreateTestTimeStamp() - 61);

        }


        private int CreateTestTimeStamp()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        #endregion


    }
}
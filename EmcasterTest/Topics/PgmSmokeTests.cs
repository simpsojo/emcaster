using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

using Emcaster.Topics;
using Emcaster.Sockets;

namespace EmcasterTest.Explicit
{
    [TestFixture]
    [Explicit]
    public class PgmSmokeTests
    {

        [Test]
        public void PubSubTest()
        {
            IList<object> msgsReceived = new List<object>();
            MessageParserFactory msgParser = new MessageParserFactory();
            PgmReader reader = new PgmReader(msgParser);
            PgmReceiver receiveSocket = new PgmReceiver("224.0.0.23", 40001, reader);

            TopicSubscriber topicSubscriber = new TopicSubscriber("MSFT", msgParser);
            topicSubscriber.Start();
            receiveSocket.Start();
            topicSubscriber.TopicMessageEvent += delegate(IMessageParser parser){
                msgsReceived.Add(parser.ParseObject());
            };


            PgmSource sendSocket = new PgmSource("224.0.0.23", 40001);
            sendSocket.Start();
            BatchWriter asyncWriter = new BatchWriter(sendSocket, 1024 * 64);
            TopicPublisher topicPublisher = new TopicPublisher(asyncWriter);
            topicPublisher.Start();

            Thread.Sleep(1000);

            for(int i = 0; i < 10; i++)
	            topicPublisher.PublishObject("MSFT", i, 1000);

            Thread.Sleep(3000);

            sendSocket.Dispose();
            receiveSocket.Dispose();	

            Assert.AreEqual(10, msgsReceived.Count);
            Assert.AreEqual(0, msgsReceived[0]);
            Assert.AreEqual(9, msgsReceived[9]);
        }
    }
}

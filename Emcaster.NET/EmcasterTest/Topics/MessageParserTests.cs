using System;
using System.Text;
using Emcaster.Topics;
using NUnit.Framework;

namespace EmcasterTest.Topics
{
    [TestFixture]
    public class MessageParserTests
    {
        [Test]
        public void TestParsing()
        {
            MessageParserFactory factory = new MessageParserFactory();
            MessageParser parser = new MessageParser(factory);
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] data = TopicPublisher.CreateMessage("test", new byte[0], 0, 0, encoder);
            data = CreateBatch(data, 10);
            int msgCount = 0;
            factory.MessageEvent += delegate { msgCount++; };
            for (int i = 0; i < 10000; i++)
            {
                parser.OnBytes(null, data, 0, data.Length);
                Assert.AreEqual((i + 1)*10, msgCount);
            }
        }

        [Test]
        public void ParseBytes()
        {
            MessageParserFactory factory = new MessageParserFactory();
            MessageParser parser = new MessageParser(factory);
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] result = new byte[0];
            factory.MessageEvent += delegate(IMessageParser p)
            {
                result = p.ParseBytes();
            };
         
            byte[] body = encoder.GetBytes("body");
            byte[] data = TopicPublisher.CreateMessage("test", body, 0, body.Length, encoder);
            parser.OnBytes(null, data, 0, data.Length);

            Assert.AreEqual("body", encoder.GetString(result));
        }

        [Test]
        [Explicit]
        public void PerfParsing()
        {
            MessageParserFactory factory = new MessageParserFactory();
            MessageParser parser = new MessageParser(factory);
            UTF8Encoding encoder = new UTF8Encoding();
            TopicSubscriber subscriber = new TopicSubscriber("test", factory);
            long msgCount =0;
            subscriber.TopicMessageEvent += delegate
            {
                msgCount++;
            };
            subscriber.Start();

            byte[] data = TopicPublisher.CreateMessage("test", new byte[0], 0, 0, encoder);
            int batchSize = 1000;
            data = CreateBatch(data, batchSize);
            DateTime start = DateTime.Now;
            int loopCount = 5000;
            for (int i = 0; i < loopCount; i++)
            {
                parser.OnBytes(null, data, 0, data.Length);
            }
            TimeSpan time = DateTime.Now.Subtract(start);
            double avgMsg = (msgCount)/time.TotalSeconds;
            Console.WriteLine("avg msg /sec: " + avgMsg); 
        }

        private byte[] CreateBatch(byte[] data, int number)
        {
            byte[] result = new byte[data.Length*number];
            for (int i = 0; i < number; i++)
            {
                Array.Copy(data, 0, result, i*data.Length, data.Length);
            }
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Emcaster.Topics;

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
            factory.MessageEvent += delegate
            {
                msgCount++;
            };
            for (int i = 0; i < 10000; i++)
            {
                parser.OnBytes(data, 0, data.Length);
                Assert.AreEqual((i+1)*10, msgCount);
            }
        }

        private byte[] CreateBatch(byte[] data, int number)
        {
            byte[] result = new byte[data.Length * number];
            for (int i = 0; i < number; i++)
            {
                Array.Copy(data, 0, result, i * data.Length, data.Length);
            }
            return result;
        }
    }
}

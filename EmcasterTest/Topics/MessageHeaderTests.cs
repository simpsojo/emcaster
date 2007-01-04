using Emcaster.Topics;
using NUnit.Framework;

namespace EmcasterTest.Topics
{
    [TestFixture]
    public class MessageHeaderTests
    {
        [Test]
        public unsafe void Buffer()
        {
            MessageHeader header = new MessageHeader(10, 20);
            Assert.AreEqual(30, header.TotalSize);
            int size = sizeof (MessageHeader);
            byte[] buffer = new byte[size];
            header.WriteToBuffer(buffer);
            fixed (byte* pBytes = &buffer[0])
            {
                MessageHeader* hdr = (MessageHeader*) pBytes;
                Assert.AreEqual(30, hdr->TotalSize);
            }
        }
    }
}
using Emcaster.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Emcaster.Topics
{
    public class TopicPublisher
    {
        private readonly UTF8Encoding _encoder = new UTF8Encoding();
        private readonly IByteWriter _writer;
        public static readonly int HEADER_SIZE = CalculateHeaderSize();

        public TopicPublisher(IByteWriter writer)
        {
            _writer = writer;
        }

        public unsafe static int CalculateHeaderSize()
        {
            return sizeof(MessageHeader);
        }

        public void Start()
        {
            _writer.Start();
        }

        public void PublishObject(string topic, object data, int msToWaitForWriteLock)
        {
            byte[] allData = ToBytes(data);
            Publish(topic, allData, 0, allData.Length, msToWaitForWriteLock); 
        }

        private static byte[] ToBytes(object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream outputStream = new MemoryStream();
            formatter.Serialize(outputStream, obj);
            return outputStream.ToArray();
        }

        public unsafe void Publish(string topic, byte[] data, int offset, int length, int msToWaitForWriteLock)
        {
            byte[] topicBytes = _encoder.GetBytes(topic);
            MessageHeader header = new MessageHeader(topicBytes.Length, length);
            int headerSize = Marshal.SizeOf(header);
            int totalSize = headerSize + header.TotalSize;
            byte[] allData = new byte[totalSize];
            GCHandle handle =
                GCHandle.Alloc(allData,GCHandleType.Pinned);
            Marshal.StructureToPtr(header,
                handle.AddrOfPinnedObject(),
                false);
            handle.Free();
            System.Array.Copy(topicBytes, 0, allData, headerSize, topicBytes.Length);
            System.Array.Copy(data, offset, allData, headerSize + topicBytes.Length, length);
            _writer.Write(allData, 0, totalSize, msToWaitForWriteLock);
        }
    }
}

using Emcaster.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System;

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

        public static byte[] CreateMessage(string topic, byte[] data, int offset, int length, UTF8Encoding encoder){
            byte[] topicBytes = encoder.GetBytes(topic);
            MessageHeader header = new MessageHeader(topicBytes.Length, length);
            int headerSize = Marshal.SizeOf(header);
            int totalSize = headerSize + header.TotalSize;
            byte[] allData = new byte[totalSize];
            header.WriteToBuffer(allData, 0);
            Array.Copy(topicBytes, 0, allData, headerSize, topicBytes.Length);
            Array.Copy(data, offset, allData, headerSize + topicBytes.Length, length);
            return allData;
        }

        public unsafe void Publish(string topic, byte[] data, int offset, int length, int msToWaitForWriteLock)
        {
            byte[] allData = CreateMessage(topic, data, offset, length, _encoder);
            _writer.Write(allData, 0, allData.Length, msToWaitForWriteLock);
        }
    }
}

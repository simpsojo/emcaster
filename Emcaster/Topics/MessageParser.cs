using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

using Emcaster.Sockets;

namespace Emcaster.Topics
{
    public class MessageParser: IMessageParser, IByteParser
    {
        private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();
        private string _topic;
        private object _object;
        private int _offset;
        private byte[] _buffer;
        private readonly IMessageListener _listener;

        public MessageParser(IMessageListener listener)
        {
            _listener = listener;
        }
   
        private unsafe int ParseTopicSize()
        {
            fixed (byte* pHeader = &_buffer[0])
            {
                return ((MessageHeader*)pHeader)->TopicSize;
            }
        }

        private unsafe int ParseBodySize()
        {
            fixed (byte* pHeader = &_buffer[0])
            {
                return ((MessageHeader*)pHeader)->BodySize;
            }
        }


        public string Topic
        {
            get
            {
                if (_topic == null)
                {
                    int topicSize = ParseTopicSize();
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream(_buffer, TopicPublisher.HEADER_SIZE, topicSize);
                    _topic = (string)formatter.Deserialize(stream);
                }
                return _topic;
            }
        }

        public object ParseObject()
        {
            if (_object == null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                int bodySize = ParseBodySize();
                int topicSize = ParseTopicSize();
                int totalOffset = _offset + topicSize + TopicPublisher.HEADER_SIZE;
                MemoryStream stream = new MemoryStream(_buffer, totalOffset, bodySize);
                _object = formatter.Deserialize(stream);
            }
            return _object;
        }

        public void OnBytes(byte[] data, int offset, int length)
        {
            ParseBytes(data, offset, length);
        }
    

        public unsafe void ParseBytes(byte[] buffer, int offset, int received)
        {
            _buffer = buffer;
            _offset = 0;
            _topic = null;
            while (_offset < received)
            {
                _object = null;
                fixed (byte* pByte = &_buffer[_offset])
                {
                    int msgSize = TopicPublisher.HEADER_SIZE + ParseTopicSize() + ParseBodySize();
                    _listener.OnMessage(this);
                    _offset += msgSize;
                }
            }
        }
    }
}

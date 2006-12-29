using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Emcaster.Topics
{
    public class MessageParser: IMessageParser
    {
        public OnTopicMessage MessageEvent;

        private readonly byte[] _buffer;
        private string _topic;
        private object _object;
        private int _offset;
  
   
        public MessageParser(byte[] buffer)
        {
            _buffer = buffer;
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

        public unsafe void ReadNext(Socket target)
        {
            int received = target.Receive(_buffer, _buffer.Length, SocketFlags.None);
            if (received < 1)
            {
                return;
            }
            ParseBytesInBuffer(received);
        }
        public unsafe void ParseBytesInBuffer(int received)
        {
            _offset = 0;
            _topic = null;
            _object = null;
            while (_offset < received)
            {
                fixed (byte* pByte = &_buffer[_offset])
                {
                    OnTopicMessage msg = MessageEvent;
                    int msgSize = TopicPublisher.HEADER_SIZE + ParseTopicSize() + ParseBodySize();
                    if (msg != null)
                    {
                        msg(this);
                    }
                    _offset += msgSize;
                }
            }
        }
    }
}

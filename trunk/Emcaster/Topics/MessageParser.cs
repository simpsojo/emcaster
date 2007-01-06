using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Emcaster.Sockets;

namespace Emcaster.Topics
{
    public unsafe class MessageParser : IMessageParser, IByteParser
    {
        private readonly UTF8Encoding _decoder = new UTF8Encoding();
        private string _topic;
        private object _object;
        private int _offset;

        private byte[] _buffer;
        private readonly IMessageListener _listener;
        private MessageHeader* _currentHeader;

        public MessageParser(IMessageListener listener)
        {
            _listener = listener;
        }

    
        public string Topic
        {
            get
            {
                if (_topic == null)
                {
                    int topicSize = _currentHeader->TopicSize;
                    _topic = _decoder.GetString(_buffer, TopicPublisher.HEADER_SIZE, topicSize);
                }
                return _topic;
            }
        }

        public object ParseObject()
        {
            if (_object == null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                int bodySize = _currentHeader->BodySize;
                int topicSize = _currentHeader->TopicSize;
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
            fixed (byte* pArray = buffer)
            {
                while (_offset < received)
                {
                    _topic = null;
                    _object = null;
                    byte* pHeader = (pArray + _offset);
                    _currentHeader = (MessageHeader*)pHeader;
                    int msgSize = TopicPublisher.HEADER_SIZE + _currentHeader->TopicSize + _currentHeader->BodySize;
                    _listener.OnMessage(this);
                    _offset += msgSize;
                }
            }
        }
    }
}
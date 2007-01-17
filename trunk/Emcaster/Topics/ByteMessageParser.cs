using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Emcaster.Topics
{
    public class ByteMessageParser:IMessageParser
    {
        private readonly string _topic;
        private readonly byte[] _body;

        public ByteMessageParser(string topic, byte[] body)
        {
            _topic = topic;
            _body = body;
        }

        public string Topic
        {
            get { return _topic; }
        }

        public object ParseObject()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            return formatter.Deserialize(stream);
        }
        public byte[] ParseBytes()
        {
            return _body;
        }
    }
}

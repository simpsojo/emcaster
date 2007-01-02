using Emcaster.Sockets;
using System.Net.Sockets;

namespace Emcaster.Topics
{
    public class MessageParserFactory:IByteParserFactory, IMessageListener, IMessageEvent
    {
        public event OnTopicMessage MessageEvent;

        public IByteParser Create(Socket socket)
        {
            return new MessageParser(this);
        }

        public void OnMessage(IMessageParser parser)
        {
            OnTopicMessage msg = MessageEvent;
            if (msg != null)
            {
                msg(parser);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Emcaster.Sockets
{
    public class ByteParser:IByteParserFactory, IByteParser
    {
        public event OnReceive ReceiveEvent;

        public IByteParser Create(Socket socket)
        {
            return this;
        }

        public void OnBytes(byte[] data, int offset, int length)
        {
            OnReceive onMsg = ReceiveEvent;
            if (onMsg != null)
            {
                onMsg(data, offset, length);
            }
        }
    }
}

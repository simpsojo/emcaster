using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Emcaster.Topics;
using Common.Logging;

namespace Emcaster.Sockets
{
    public class SourceReader:ISourceReader
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(SourceReader));

        private readonly IByteParserFactory _parserFactory;

        private int _receiveBufferSize = 1024 * 128;
        private int _readBuffer = 1024 * 128;


        public SourceReader(IByteParserFactory factory)
        {
            _parserFactory = factory;
        }

        public int ReceiveBuffer
        {
            set { _receiveBufferSize = (value * 1024); }
        }

        public int ReadBuffer
        {
            set { _readBuffer = (value * 1024); }
        }


        public void AcceptSocket(Socket receiveSocket, ref bool _running)
        {
            IByteParser parser = _parserFactory.Create(receiveSocket);
            using (receiveSocket)
            {
                PgmReceiver.EnableGigabit(receiveSocket);
                if (_receiveBufferSize > 0)
                {
                    receiveSocket.ReceiveBufferSize = _receiveBufferSize;
                }
                byte[] buffer = new byte[_readBuffer];
                try
                {
                    int read = receiveSocket.Receive(buffer, 0, _readBuffer, SocketFlags.None);
                    while (read > 0 && _running)
                    {
                        parser.OnBytes(buffer, 0, read);
                        receiveSocket.Blocking = true;
                        read = receiveSocket.Receive(buffer, 0, _readBuffer, SocketFlags.None);
                    }
                }
                catch (Exception failed)
                {
                    log.Info("Closing", failed);
                }
            }
        }
         
    }
}

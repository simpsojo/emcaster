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

        private int _receiveBufferSize = 1024 * 1024;
        private int _readBuffer = 1024 * 130;


        public SourceReader(IByteParserFactory factory)
        {
            _parserFactory = factory;
        }

        public int ReceiveBufferInBytes
        {
            set { _receiveBufferSize = (value); }
        }

        public int ReadBufferInBytes
        {
            set { _readBuffer = (value); }
        }


        public void AcceptSocket(Socket receiveSocket, ref bool _running)
        {
            IByteParser parser = _parserFactory.Create(receiveSocket);
            using (receiveSocket)
            {
                PgmSocket.EnableGigabit(receiveSocket);
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
                catch (SocketException socketFailed)
                {
                    log.Info("Native Error: " + socketFailed.NativeErrorCode);
                    log.Info("Socket Error Code: " + socketFailed.SocketErrorCode);
                    log.Info("Socket Error: " + socketFailed.ErrorCode, socketFailed);
                }
                catch (Exception failed)
                {
                    log.Info("Unknown Exception", failed);
                }
            }
        }
         
    }
}

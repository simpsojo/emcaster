using System;
using System.Net.Sockets;
using Common.Logging;

namespace Emcaster.Sockets
{

    public class PgmReader : ISourceReader, IPacketEvent
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (PgmReader));

        public event OnReceive ReceiveEvent;

        private readonly IByteParserFactory _parserFactory;

        private int _receiveBufferSize = 1024*1024;
        private int _readBuffer = 1024*130;
        private bool _forceBlockingOnEveryReceive = false;

        public event OnSocketException SocketExceptionEvent;
        public event OnException ExceptionEvent;

        public PgmReader(IByteParserFactory factory)
        {
            _parserFactory = factory;
        }

        public int ReceiveBufferInBytes
        {
            set { _receiveBufferSize = (value); }
            get { return _receiveBufferSize;  }
        }

        public int ReadBufferInBytes
        {
            set { _readBuffer = (value); }
            get { return _readBuffer;  }
        }

        /// <summary>
        /// Set to true to compensate for strange bug in socket protocol.
        /// Not always needed.
        /// </summary>
        public bool ForceBlockingOnEveryReceive
        {
            get { return _forceBlockingOnEveryReceive; }
            set { _forceBlockingOnEveryReceive = value; }
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
                receiveSocket.Blocking = true;

                byte[] buffer = new byte[_readBuffer];
                try
                {
                    int read = receiveSocket.Receive(buffer, 0, _readBuffer, SocketFlags.None);
                    while (read > 0 && _running)
                    {
                        OnReceive recv = ReceiveEvent;
                        if (recv != null)
                        {
                            recv(buffer, 0, read);
                        }
                        parser.OnBytes(buffer, 0, read);
                        if (_forceBlockingOnEveryReceive)
                        {
                            receiveSocket.Blocking = true;
                        }
                        read = receiveSocket.Receive(buffer, 0, _readBuffer, SocketFlags.None);
                    }
                }
                catch (SocketException socketFailed)
                {
                    log.Info("Native Error: " + socketFailed.NativeErrorCode);
                    log.Info("Socket Error Code: " + socketFailed.SocketErrorCode);
                    log.Info("Socket Error: " + socketFailed.ErrorCode, socketFailed);
                    OnSocketException socketExc = SocketExceptionEvent;
                    if (socketExc != null)
                    {
                        socketExc(receiveSocket, socketFailed);
                    }
                }
                catch (Exception failed)
                {
                    log.Info("Unknown Exception", failed);
                    OnException excEvent = ExceptionEvent;
                    if (excEvent != null)
                    {
                        excEvent(receiveSocket, failed);
                    }
                }
            }
        }
    }
}
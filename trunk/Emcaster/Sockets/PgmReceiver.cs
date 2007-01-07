using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;

namespace Emcaster.Sockets
{
    public delegate void OnReceive(byte[] data, int offset, int length);

    public class PgmReceiver : IDisposable
    {
        private static ILog log = LogManager.GetLogger(typeof (PgmReceiver));

        private bool _running = true;
        private string _ip;
        private int _port;
        private PgmSocket _socket;
        private ISourceReader _reader;
        private int _receiveBufferInBytes = 1024*128;

        public PgmReceiver(string address, int port, ISourceReader reader)
        {
            _socket = new PgmSocket();
            _ip = address;
            _port = port;
            _reader = reader;
        }

        public string Address
        {
            set { _ip = value; }
        }

        public int Port
        {
            set { _port = value; }
        }

        public int ReceiveBufferInBytes
        {
            get { return _receiveBufferInBytes;  }
            set { _receiveBufferInBytes = value; }
        }

        public void Start()
        {
            _socket.ReceiveBufferSize = _receiveBufferInBytes;
            IPAddress ipAddr = IPAddress.Parse(_ip);
            IPEndPoint end = new IPEndPoint(ipAddr, _port);
            _socket.Bind(end);
            _socket.ApplySocketOptions();
            PgmSocket.EnableGigabit(_socket);
            _socket.Listen(5);
            log.Info("Listening: " + end);
            if (!ThreadPool.QueueUserWorkItem(RunAccept))
            {
                log.Error("Not able to enqueue accept delegate in thread pool.");
            }
        }

        private void RunAccept(object state)
        {
            while (_running)
            {
                try
                {
                    Socket conn = _socket.Accept();
                    log.Info("Connection from: " + conn.RemoteEndPoint);
                    WaitCallback runner = delegate { RunReceiver(conn); };
                    if (!ThreadPool.QueueUserWorkItem(runner))
                    {
                        log.Error("Unable to Enqueue PGM Receiver");
                    }
                }
                catch (Exception failed)
                {
                    if (_running)
                        log.Warn("Accept Failed", failed);
                }
            }
        }

        private void RunReceiver(Socket receiveSocket)
        {
            _reader.AcceptSocket(receiveSocket, ref _running);
        }

        public void Dispose()
        {
            _running = false;
            try
            {
                _socket.Close();
            }
            catch (Exception failed)
            {
                log.Warn("close failed", failed);
            }
        }
    }
}
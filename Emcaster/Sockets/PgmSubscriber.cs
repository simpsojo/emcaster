using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;

namespace Emcaster.Sockets
{
    public delegate void OnReceive(byte[] data, int offset, int length);

    public class PgmSubscriber : IDisposable
    {
        public event OnReceive ReceiveEvent;

        private static ILog log = LogManager.GetLogger(typeof(PgmSubscriber));
 
        private bool _running = true;
        private string _ip;
        private int _port;
        private PgmSocket _socket;
        private int _receiveBufferSize = 1024*128;
        private int _readBuffer=1024*128;

        public PgmSubscriber(string address, int port)
        {
            _socket = new PgmSocket();
            _ip = address;
            _port = port;
        }

        public string Address
        {
            set { _ip = value; }
        }

        public int Port
        {
            set { _port = value; }
        }

        public int ReceiveBuffer
        {
            set { _receiveBufferSize = (value * 1024); }
        }

        public int ReadBuffer
        {
            set { _readBuffer = (value * 1024); }
        }


        private static void SetSocketOption(Socket socket, string name, int option, ulong val)
        {
            try
            {
                byte[] bits = BitConverter.GetBytes(val);
                PgmSocket.SetPgmOption(socket, option, bits);
                log.Info("Set: " + name + " Option : " + option + " value: " + val);
            }
            catch (Exception failed)
            {
                log.Debug(name + " Option : " + option + " value: " + val, failed);
            }
        }

        public void Start()
        {
            IPAddress ipAddr = IPAddress.Parse(_ip);
            IPEndPoint end = new IPEndPoint(ipAddr, _port);
            _socket.Bind(end);

            EnableGigabit(_socket);
            _socket.Listen(5);
            log.Info("Listening: " + end);
            ThreadPool.QueueUserWorkItem(RunAccept);
        }

        private static void EnableGigabit(Socket socket)
        {
            SetSocketOption(socket, "Gigabit", 1014, 1);
        }

        private void RunAccept(object state)
        {
            while (_running)
            {
                try
                {
                    Socket conn = _socket.Accept();
                    conn.Blocking = true;
                    log.Info("Connection from: " + conn.RemoteEndPoint);
                    WaitCallback runner = delegate { RunReceiver(conn); };
                    ThreadPool.QueueUserWorkItem(runner);
                }
                catch (Exception failed)
                {
                    if(_running)
                        log.Warn("Accept Failed", failed);
                }
            }
        }

        private void RunReceiver(Socket receiveSocket)
        {
            using (receiveSocket)
            {
                EnableGigabit(receiveSocket);
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
                        OnReceive onMsg = ReceiveEvent;
                        if (onMsg != null)
                        {
                            ReceiveEvent(buffer, 0, read);
                        }
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
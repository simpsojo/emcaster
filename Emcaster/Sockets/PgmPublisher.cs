using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;

namespace Emcaster.Sockets
{
 
    public class PgmPublisher : IDisposable
    {
   
        private static ILog log = LogManager.GetLogger(typeof(PgmPublisher));
 
        private string _ip;
        private int _port;
        private PgmSocket _socket;

        public PgmPublisher(string address, int port)
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

  
        public void Start()
        {
            IPAddress ipAddr = IPAddress.Parse(_ip);
            IPEndPoint end = new IPEndPoint(ipAddr, _port);
            _socket.Connect(end);
        }

        public int Publish(params byte[] dataToPublish)
        {
            return _socket.Send(dataToPublish);
        }


        public void Dispose()
        {
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
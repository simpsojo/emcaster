using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using Common.Logging;

namespace Emcaster.Sockets
{
    public class UdpReceiver : IPacketEvent, IDisposable
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(UdpReceiver));

        public event OnReceive ReceiveEvent;

        private readonly UdpClient _client;
        private readonly IPAddress _address;
        private bool _running = true;


        public UdpReceiver(string address, int port)
        {
            _client = new UdpClient(port);
            _address = IPAddress.Parse(address);
        }

        public UdpClient Client
        {
            get { return _client; }
        }

        public void Start()
        {
            _client.JoinMulticastGroup(_address);
            WaitCallback runner = delegate
            {
                try
                {
                    ReadAll();
                }
                catch (Exception failed)
                {
                    log.Warn("read failed. ending connection: " + _address, failed);
                }
            };
            ThreadPool.QueueUserWorkItem(runner);
   
       }

        private void ReadAll()
        {
                        while (_running)
                        {
                            IPEndPoint endpoint = null;
                            byte[] packet = _client.Receive(ref endpoint);
                            OnReceive rcv = ReceiveEvent;
                            if (rcv != null)
                            {
                                rcv(packet, 0, packet.Length);
                            }
                        }
        }

       public void Dispose()
       {
           _running = false;
           _client.Close();
       }
    }
}

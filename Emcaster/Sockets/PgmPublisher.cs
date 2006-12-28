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
 
        private readonly string _ip;
        private readonly int _port;
        private readonly PgmSocket _socket;
 
        public PgmPublisher(string address, int port)
        {
            _socket = new PgmSocket();
            _ip = address;
            _port = port;
        }

        public PgmSocket Socket
        {
            get { return _socket; }
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
        
        public unsafe _RM_SENDER_STATS GetSenderStats()
        {
            int size = sizeof(_RM_SENDER_STATS);
            byte[] data = _socket.GetSocketOption(PgmSocket.PGM_LEVEL, (SocketOptionName)1005, size);
            fixed(byte* pBytes = &data[0])
            {
                return *((_RM_SENDER_STATS*)pBytes);
            }
        }
    }

    public struct _RM_SENDER_STATS
    {
        public ulong DataBytesSent;          // # client data bytes sent out so far
        public ulong TotalBytesSent;         // SPM, OData and RData bytes
        public ulong NaksReceived;           // # NAKs received so far
        public ulong NaksReceivedTooLate;    // # NAKs recvd after window advanced
        public ulong NumOutstandingNaks;     // # NAKs yet to be responded to
        public ulong NumNaksAfterRData;      // # NAKs yet to be responded to
        public ulong RepairPacketsSent;      // # Repairs (RDATA) sent so far
        public ulong BufferSpaceAvailable;   // # partial messages dropped
        public ulong TrailingEdgeSeqId;      // smallest (oldest) Sequence Id in the window
        public ulong LeadingEdgeSeqId;       // largest (newest) Sequence Id in the window
        public ulong RateKBitsPerSecOverall; // Internally calculated send-rate from the beginning
        public ulong RateKBitsPerSecLast;    // Send-rate calculated every INTERNAL_RATE_CALCULATION_FREQUENCY
    } 

}
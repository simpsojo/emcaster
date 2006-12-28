using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Common.Logging;

namespace Emcaster.Sockets
{
    public class PgmSocket: Socket
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PgmSocket));

        public static readonly int PROTOCOL_TYPE_NUMBER = 113;
        public static readonly ProtocolType PGM_PROTOCOL_TYPE = (ProtocolType)113;
        public static readonly SocketOptionLevel PGM_LEVEL = (SocketOptionLevel)PROTOCOL_TYPE_NUMBER;

        private IDictionary<int, ulong> _socketOptions = new Dictionary<int, ulong>();

        public PgmSocket(): base(AddressFamily.InterNetwork, SocketType.Rdm,PGM_PROTOCOL_TYPE) 
        {
        }

        public static void SetPgmOption(Socket socket, int option, byte[] value)
        {
            socket.SetSocketOption(PGM_LEVEL, (SocketOptionName)option, value);
        }

        public void SetPgmOption(int option, byte[] value)
        {
            SetSocketOption(PGM_LEVEL, (SocketOptionName)option, value);
        }

        public void AddSocketOption(int opt, ulong val)
        {
            _socketOptions[opt] = val;
        }

        public IDictionary<int, ulong> SocketOptions
        {
            set { _socketOptions = value; }
        }

        public void ApplySocketOptions()
        {
            foreach (int option in _socketOptions.Keys)
            {
                SetSocketOption(this, option.ToString(), option, _socketOptions[option]);
            }
        }

        public static void SetSocketOption(Socket socket, string name, int option, ulong val)
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


  
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Emcaster.Sockets
{
    public class PgmSocket: Socket
    {
        public static readonly int PROTOCOL_TYPE_NUMBER = 113;
        public static readonly ProtocolType PGM_PROTOCOL_TYPE = (ProtocolType)113;
        public static readonly SocketOptionLevel PGM_LEVEL = (SocketOptionLevel)PROTOCOL_TYPE_NUMBER;

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
  
    }
}

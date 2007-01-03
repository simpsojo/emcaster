using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Common.Logging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;


namespace Emcaster.Sockets
{
    public class PgmSocket: Socket
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PgmSocket));

        public static readonly int PROTOCOL_TYPE_NUMBER = 113;
        public static readonly ProtocolType PGM_PROTOCOL_TYPE = (ProtocolType)113;
        public static readonly SocketOptionLevel PGM_LEVEL = (SocketOptionLevel)PROTOCOL_TYPE_NUMBER;

        private IDictionary<int, uint> _socketOptions = new Dictionary<int, uint>();

        public PgmSocket(): base(AddressFamily.InterNetwork, SocketType.Rdm,PGM_PROTOCOL_TYPE) 
        {
        }

        public static void SetPgmOption(Socket socket, int option, byte[] value)
        {
            socket.SetSocketOption(PGM_LEVEL, (SocketOptionName)option, value);
        }

        public void SetPgmOption(int option, byte[] value)
        {
            try
            {
                SetSocketOption(PGM_LEVEL, (SocketOptionName)option, value);
            }
            catch (Exception failed)
            {
                log.Warn("failed", failed);
            }
        }

        public void AddSocketOption(int opt, uint val)
        {
            _socketOptions[opt] = val;
        }

        public IDictionary<int, uint> SocketOptions
        {
            set { _socketOptions = value; }
        }

        internal void ApplySocketOptions()
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

        public unsafe _RM_RECEIVER_STATS GetReceiverStats(Socket socket)
        {
            int size = sizeof(_RM_RECEIVER_STATS);
            byte[] data = socket.GetSocketOption(PgmSocket.PGM_LEVEL, (SocketOptionName)1013, size);
            fixed (byte* pBytes = &data[0])
            {
                return *((_RM_RECEIVER_STATS*)pBytes);
            }
        }

        public static byte[] ConvertStructToBytes(object obj)
        {
            int structSize = Marshal.SizeOf(obj);
            byte[] allData = new byte[structSize];
            GCHandle handle =
                GCHandle.Alloc(allData, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj,
                handle.AddrOfPinnedObject(),
                false);
            handle.Free();
            return allData;
        }

  
    }
}

using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Emcaster.Sockets
{
    public interface ISourceReader
    {
        void AcceptSocket(Socket socket, ref bool running);
    }
}

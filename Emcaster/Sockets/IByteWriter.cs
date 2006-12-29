using System;
using System.Collections.Generic;
using System.Text;

namespace Emcaster.Sockets
{
    public interface IByteWriter
    {
        bool Write(byte[] data, int offset, int length);

        void Start();
    }
}

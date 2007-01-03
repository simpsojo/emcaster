using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;


namespace Emcaster.Topics
{
    public struct MessageHeader
    {
        private int _topicSize;
        private int _bodySize;

        public MessageHeader(int topicSize, int bodySize)
        {
            _topicSize = topicSize;
            _bodySize = bodySize;
        }

        public int TopicSize
        {
            get { return _topicSize; }
        }

        public int BodySize
        {
            get { return _bodySize; }
        }

        public int TotalSize
        {
            get { return _topicSize + _bodySize; }
        }

        public unsafe void WriteToBuffer(byte[] allData, int p)
        {
            GCHandle handle = GCHandle.Alloc(allData, GCHandleType.Pinned);
            Marshal.StructureToPtr(this,
                handle.AddrOfPinnedObject(),
                false);
            handle.Free();
        }
    }
}

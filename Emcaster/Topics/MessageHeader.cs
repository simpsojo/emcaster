using System;
using System.Collections.Generic;
using System.Text;

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
    }
}

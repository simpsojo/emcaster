using System;
using System.Collections.Generic;
using System.Text;

namespace Emcaster.Topics
{
    public interface IMessageListener
    {
        void OnMessage(IMessageParser parser);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Emcaster.Topics
{
    public interface IMessageEvent
    {
        event OnTopicMessage MessageEvent;
    }
}

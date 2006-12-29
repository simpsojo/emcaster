using System;
using System.Collections.Generic;
using System.Text;

namespace Emcaster.Topics
{
    public interface IMessageParser
    {

        string Topic
        {
            get;
        }

        object ParseObject();
    }
}

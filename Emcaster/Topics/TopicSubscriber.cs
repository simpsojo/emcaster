using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Emcaster.Topics
{
    public delegate void OnTopicMessage(IMessageParser parser);

    public class TopicSubscriber
    {
        public event OnTopicMessage TopicMessageEvent;

        private readonly Regex _regex;

        public TopicSubscriber(string topic)
        {
            _regex = new Regex(topic);
        }

        public void OnTopicMessage(IMessageParser parser)
        {
            OnTopicMessage msg = TopicMessageEvent;
            if (msg != null)
            {
                string topic = parser.Topic;
                if (_regex.IsMatch(parser.Topic))
                {
                    msg(parser);
                }
            }
        }

    }
}

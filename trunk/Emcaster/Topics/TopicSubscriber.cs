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
        private readonly IMessageEvent _msgEvent;

        public TopicSubscriber(string topic, IMessageEvent msgEvent)
        {
            _regex = new Regex(topic);
            _msgEvent = msgEvent;
        }

        public void Start()
        {
            _msgEvent.MessageEvent += OnTopicMessage;
        }

        public void Stop()
        {
            _msgEvent.MessageEvent -= OnTopicMessage;
        }

        private void OnTopicMessage(IMessageParser parser)
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

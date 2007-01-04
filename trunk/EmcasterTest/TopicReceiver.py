import clr
import sys
clr.AddReference("Emcaster")
clr.AddReference("EmcasterTest")

from Emcaster.Sockets import *
from Emcaster.Topics import *
from System.Threading import *
from EmcasterTest import *

args = sys.argv[1:]

if len(args) != 3:
    print "Usage: EmReceiver 223.0.0.23 4002 my-topic"
    sys.exit(1)

Startup.ConfigureLogging()

msgParser = MessageParserFactory()
reader = SourceReader(msgParser)
port = int(args[1])
receiveSocket = PgmReceiver(args[0], port, reader)

topicSubscriber = TopicSubscriber(args[2], msgParser)
monitor = TopicMonitor(args[2], 10);
topicSubscriber.TopicMessageEvent += monitor.OnMessage;

def start_all():
	topicSubscriber.Start()
	monitor.Start()
	receiveSocket.Start()

def dispose_all():
	topiSubscriber.Dispose()
	monitor.Dispose()
	receiveSocket.Dispose()
	
sys.exitfunc = dispose_all
	
print args[0], ":", port

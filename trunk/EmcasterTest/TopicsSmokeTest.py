import clr
clr.AddReference("Emcaster")
clr.AddReference("EmcasterTest")
clr.AddReference("nunit.framework")

from Emcaster.Sockets import *
from Emcaster.Topics import *
from EmcasterTest import *
from System.Threading import *

from NUnit.Framework import *

Startup.Init();

msgsReceived = []
receiveSocket = PgmSubscriber("224.0.0.23", 40001)
msgParser = MessageParser()
receiveSocket.ReceiveEvent += msgParser.ParseBytes
topicSubscriber = TopicSubscriber("MSFT")
msgParser.MessageEvent += topicSubscriber.OnTopicMessage
receiveSocket.Start()
def OnMsg(msg):
	msgsReceived.append(msg.ParseObject())

topicSubscriber.TopicMessageEvent += OnMsg


sendSocket = PgmPublisher("224.0.0.23", 40001)
sendSocket.Start()
asyncWriter = AsyncByteWriter(sendSocket, 1024*64)
topicPublisher = TopicPublisher(asyncWriter);
topicPublisher.Start()

Thread.Sleep(1000)

for x in xrange(10):
	topicPublisher.PublishObject("MSFT", x)
	Thread.Sleep(1000)

sendSocket.Dispose()
receiveSocket.Dispose()	


Assert.AreEqual(10, len(msgsReceived))
Assert.AreEqual(0, msgsReceived[0])
Assert.AreEqual(9, msgsReceived[9])
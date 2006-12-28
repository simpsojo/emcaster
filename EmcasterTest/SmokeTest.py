import clr
clr.AddReference("Emcaster")
clr.AddReference("EmcasterTest")
clr.AddReference("nunit.framework")

from Emcaster.Sockets import *
from EmcasterTest import *
from System.Threading import *

from NUnit.Framework import *

Startup.Init();

receiveSocket = PgmSubscriber("224.0.0.23", 40001)

socketMonitor = SocketMonitor();

receiveSocket.ReceiveEvent += socketMonitor.OnReceive

receiveSocket.Start()

sendSocket = PgmPublisher("224.0.0.23", 40001)
sendSocket.Start()

Thread.Sleep(1000)

for x in xrange(10):
	sendSocket.Publish(x)
	Thread.Sleep(1000)

sendSocket.Dispose()
print "closed sender"
receiveSocket.Dispose()
print "closed receiver"

Assert.AreEqual(10, socketMonitor.BytesReceived)
Assert.AreEqual(10, socketMonitor.MessagesReceived)
import clr
clr.AddReference("Emcaster")
clr.AddReference("EmcasterTest")
clr.AddReference("nunit.framework")

from Emcaster.Sockets import *
from EmcasterTest import *
from System.Threading import *

from NUnit.Framework import *

Startup.Init();

parser = ByteParser()
reader = SourceReader(parser)
receiveSocket = PgmReceiver("224.0.0.23", 40001, reader)

socketMonitor = SocketMonitor();

parser.ReceiveEvent += socketMonitor.OnReceive

receiveSocket.Start()

sendSocket = PgmSource("224.0.0.23", 40001)
sendSocket.Start()

Thread.Sleep(1000)

for x in xrange(10):
	sendSocket.Publish(x)
	Thread.Sleep(1000)

sendStats = sendSocket.GetSenderStats()

sendSocket.Dispose()

receiveSocket.Dispose()	


Assert.AreEqual(10, sendStats.DataBytesSent)
Assert.AreEqual(10, socketMonitor.BytesReceived)
Assert.AreEqual(10, socketMonitor.MessagesReceived)
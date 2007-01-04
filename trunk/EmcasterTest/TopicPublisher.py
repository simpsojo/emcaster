import clr
clr.AddReference("Emcaster")
clr.AddReference("EmcasterTest")

from EmcasterTest import *
from Emcaster.Sockets import *
from Emcaster.Topics import *
from System.Threading import *

import sys

Startup.Init();

address = "224.0.0.23"
port = 8001

sendSocket = PgmSource(address, port)
sendSocket.RateKbitsPerSec = 50000
sendSocket.WindowSizeInMSecs = 2000
sendSocket.WindowSizeinBytes = 0

sys.exitfunc = sendSocket.Dispose

asyncWriter = AsyncByteWriter(sendSocket, 1024*128)
asyncWriter.PrintStats = 1

publisher = TopicPublisher(asyncWriter);

topic = "test"
numBytes = 40
waitTime = 1000
msgCount = 9999999

def go():
	sendSocket.Start()
	publisher.Start()
	Startup.PublishBatch(publisher, topic, numBytes, waitTime, msgCount)

print "Ready to Publish: ", address, " " , port
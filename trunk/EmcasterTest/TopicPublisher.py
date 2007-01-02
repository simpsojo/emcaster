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
sys.exitfunc = sendSocket.Dispose
sendSocket.Start()

asyncWriter = AsyncByteWriter(sendSocket, 1024*64)
asyncWriter.SleepOnMin = 20
asyncWriter.MinFlushSize = 1024*5

publisher = TopicPublisher(asyncWriter);
publisher.Start()

Thread.Sleep(1000)
print "Ready to Publish: ", address, " " , port
package com.emcaster.topics;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import junit.framework.TestCase;

public class MessageParserTest extends TestCase{
	
	public void testParsing(){
		String msg = "test";
		String topic = "topic";
		ByteBuffer buffer = ByteBuffer.allocate(1024);
		buffer.order(ByteOrder.LITTLE_ENDIAN);
		MessageParserImpl.WriteToBuffer(topic, msg.getBytes(), 0, msg.getBytes().length, buffer);
		
		buffer.rewind();
		MessageParserImpl parser = new MessageParserImpl(buffer, null);
		parser.parseNextMessage();
		assertEquals(topic, parser.getTopic());
		assertEquals(msg, new String(parser.getMessage()));
	}

}

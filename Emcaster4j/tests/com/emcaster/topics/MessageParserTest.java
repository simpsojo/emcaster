package com.emcaster.topics;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import junit.framework.TestCase;

public class MessageParserTest extends TestCase{
	
	public void testWriting2Messages()throws Exception{
		int expectedSize = "1234".getBytes(MessageParserImpl.STRING_ENCODING).length;
		int packetSize = expectedSize + 8;
		ByteBuffer buffer = ByteBuffer.allocate(packetSize * 2 );		
		assertTrue(MessageParserImpl.writeToBuffer("1234", new byte[0], 0,0, buffer));
		assertTrue(MessageParserImpl.writeToBuffer("1234", new byte[0], 0,0, buffer));
		assertFalse(MessageParserImpl.writeToBuffer("1234", new byte[0], 0,0, buffer));
	}
	
	public void testWritingOversizedMessages()throws Exception{
		int expectedSize = "1234".getBytes(MessageParserImpl.STRING_ENCODING).length;
		int packetSize = expectedSize + 8;
		ByteBuffer buffer = ByteBuffer.allocate( (packetSize * 2)-1 );		
		assertTrue(MessageParserImpl.writeToBuffer("1234", new byte[0], 0,0, buffer));
		assertFalse(MessageParserImpl.writeToBuffer("1234", new byte[0], 0,0, buffer));
		assertFalse(MessageParserImpl.writeToBuffer("1234", new byte[0], 0,0, buffer));
	}
	
	public void testParsing(){
		String msg = "test";
		String topic = "topic";
		ByteBuffer buffer = ByteBuffer.allocate(1024);
		buffer.order(ByteOrder.LITTLE_ENDIAN);
		MessageParserImpl.writeToBuffer(topic, msg.getBytes(), 0, msg.getBytes().length, buffer);
		
		buffer.rewind();
		MessageParserImpl parser = new MessageParserImpl(buffer, null);
		parser.parseNextMessage();
		assertEquals(topic, parser.getTopic());
		assertEquals(msg, new String(parser.getMessage()));
	}

	public void testWritingOversizedMessage(){
		ByteBuffer buffer = ByteBuffer.allocate(5);
		try{
			MessageParserImpl.writeToBuffer("123456", new byte[0], 0,0, buffer);
			fail("should throw exception");
		}catch(IllegalArgumentException expected){
			
		}
	}
	
}

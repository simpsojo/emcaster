package com.emcaster.topics;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

/**
 * Converts messages into a byte array for transport. Instances are not thread safe, but
 * may be reused after the publish or writeTo methods are called. 
 * 
 * @author mrettig
 */
public class MessageBuffer {

	private final ByteBuffer _buffer;
	private final DatagramPacket _packet;
	
	public MessageBuffer(int bufferSize, InetAddress address, int port){
		_buffer = ByteBuffer.allocate(bufferSize);
		_buffer.order(ByteOrder.LITTLE_ENDIAN);
		_packet = new DatagramPacket(_buffer.array(), _buffer.capacity());
		_packet.setPort(port);
		_packet.setAddress(address);
	}	

	public boolean publish(DatagramPacketPublisher pub, String topic, byte[] msg){
		if(appendMessage(topic, msg, 0, msg.length)){
			writeTo(pub);
			return true;
		}
		return false;
	}
	
	public boolean appendMessage(String topic, byte[] msg, int offset, int length) {		
		if(!MessageParserImpl.WriteToBuffer(topic, msg, offset, length, _buffer)){
			return false;
		}
		_packet.setLength(_buffer.position());
		return true;
	}
	
	public void writeTo(DatagramPacketPublisher publisher){
		publisher.publish(_packet);
		reset();
	}

	public void reset() {
		_buffer.rewind();
	}

	public int getLength() {
		return _buffer.position();
	}
	
}

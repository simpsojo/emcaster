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

	/**
	 * Sends the underlying packet to the publisher, then resets the internal
	 * buffer.
	 * @param pub
	 * @param topic
	 * @param msg
	 * @return
	 */
	public boolean publish(DatagramPacketPublisher pub, String topic, byte[] msg){
		if(appendMessage(topic, msg, 0, msg.length)){
			writeTo(pub);
			return true;
		}
		return false;
	}
	
	public boolean appendMessage(String topic, byte[] msg, int offset, int length) {		
		if(!MessageParserImpl.writeToBuffer(topic, msg, offset, length, _buffer)){
			return false;
		}
		_packet.setLength(_buffer.position());
		return true;
	}
	
	/**
	 * Writes packet to publisher, then resets buffer.
	 * @param publisher
	 */
	public void writeTo(DatagramPacketPublisher publisher){
		publisher.publish(_packet);
		reset();
	}

	/**
	 * reset buffer back to the first byte. Called after each send.
	 */
	public void reset() {
		_buffer.rewind();
	}

	/**
	 * number of bytes in the message buffer.
	 * @return
	 */
	public int getLength() {
		return _buffer.position();
	}
	
}

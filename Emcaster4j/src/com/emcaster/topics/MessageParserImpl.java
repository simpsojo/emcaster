package com.emcaster.topics;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.nio.ByteBuffer;

public class MessageParserImpl implements MessageParser, Message {

	private final ByteBuffer _buffer;

	private String _topic;

	private byte[] _message;

	private DatagramPacket _packet;

	public MessageParserImpl(ByteBuffer buffer, DatagramPacket packet) {
		_buffer = buffer;
		_packet = packet;
	}

	public static boolean WriteToBuffer(String topic, byte[] message, int offset,
			int length, ByteBuffer buffer) {
		byte[] topicBytes = topic.getBytes();
		int totalLength = topicBytes.length + length;
		if(buffer.position() + totalLength > buffer.capacity()){
			return false;
		}
		buffer.putInt(topicBytes.length);
		buffer.putInt(message.length);
		buffer.put(topicBytes);
		buffer.put(message, offset, length);
		return true;
	}

	public InetAddress getAddress(){
		return _packet.getAddress();
	}
	
	public int getPort(){
		return _packet.getPort();
	}
	
	public int getPosition(){
		return _buffer.position();
	}
	
	public Message copy(){
		return new MessageImpl(this);
	}
	
	public void parseNextMessage() {
		int topicLength = _buffer.getInt();
		int msgLength = _buffer.getInt();
		byte[] topicBytes = new byte[topicLength];
		_message = new byte[msgLength];
		_buffer.get(topicBytes);
		_topic = new String(topicBytes);
		_buffer.get(_message);
	}
	
	public String getTopic(){
		return _topic;
	}
	
	public byte[] getMessage(){
		return _message;
	}
}

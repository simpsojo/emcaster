package com.emcaster.topics;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.nio.ByteBuffer;

public class MessageParserImpl implements MessageParser, Message {

	public static String STRING_ENCODING = "UTF-8";
	
	private final ByteBuffer _buffer;

	private String _topic;

	private byte[] _message;

	private DatagramPacket _packet;

	public MessageParserImpl(ByteBuffer buffer, DatagramPacket packet) {
		_buffer = buffer;
		_packet = packet;
	}

	public static boolean writeToBuffer(String topic, byte[] message, int offset,
			int length, ByteBuffer buffer) {
		byte[] topicBytes = toBytes(topic);
		int totalLength = topicBytes.length + length + 8;
		if(totalLength > buffer.capacity()){
			throw new IllegalArgumentException("Message length: " + totalLength + " is greater than the capacity of the buffer: " + buffer.capacity());
		}
		if(buffer.position() + totalLength > buffer.capacity()){
			return false;
		}
		buffer.putInt(topicBytes.length);
		buffer.putInt(message.length);
		buffer.put(topicBytes);
		buffer.put(message, offset, length);
		return true;
	}

	private static byte[] toBytes(String topic) {
		try{
			return topic.getBytes(STRING_ENCODING);
		}catch(Exception failed){
			throw new RuntimeException(failed);
		}
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
	
	public Message parseNextMessage() {
		int topicLength = _buffer.getInt();
		int msgLength = _buffer.getInt();
		byte[] topicBytes = new byte[topicLength];
		_message = new byte[msgLength];
		_buffer.get(topicBytes);
		_topic = toString(topicBytes);
		_buffer.get(_message);
		return this;
	}
	
	private String toString(byte[] topicBytes) {
		try{
			return new String(topicBytes, STRING_ENCODING);
		}catch(Exception failed){
			throw new RuntimeException(failed);
		}
	}

	public String getTopic(){
		return _topic;
	}
	
	public byte[] getMessage(){
		return _message;
	}
}

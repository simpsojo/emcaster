package com.emcaster.topics;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public class TopicPublisherImpl{

	private final String _address;
	private final int _port;
	private final ByteBuffer _buffer;
	private final MulticastSocket _socket;
	private final DatagramPacket _packet;
	
	public TopicPublisherImpl(String address, int port, int bufferSize) throws IOException{
		_address = address;
		_port = port;
		_buffer = ByteBuffer.allocate(bufferSize);
		_buffer.order(ByteOrder.LITTLE_ENDIAN);
		_socket = new MulticastSocket(_port);		
		_packet = new DatagramPacket(_buffer.array(), _buffer.capacity());
	}
	
	public MulticastSocket getSocket(){
		return _socket;
	}
	
	public void start() throws IOException{
		InetAddress inet = InetAddress.getByName(_address);
		_socket.joinGroup(inet);
		_packet.setPort(_port);
		_packet.setAddress(inet);
	}
	
	public void publish(String topic, byte[] msg, int offset, int length) throws IOException{
		_buffer.rewind();
		MessageParserImpl.WriteToBuffer(topic, msg, offset, length, _buffer);
		_packet.setLength(_buffer.position());
		_socket.send(_packet);
	}
	
	public void stop() throws IOException{
		InetAddress inet = InetAddress.getByName(_address);
		_socket.leaveGroup(inet);
		_socket.close();
	}

	public void publish(String topic, byte[] bytes) throws IOException {
		publish(topic, bytes, 0, bytes.length);		
	}

}

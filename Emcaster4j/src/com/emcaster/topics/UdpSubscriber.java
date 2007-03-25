package com.emcaster.topics;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Iterator;

public class UdpSubscriber implements TopicSubscriber {

	private final String _address;
	private final int _port;
	private final ByteBuffer _buffer;
	private final MulticastSocket _socket;
	private final MessageParserImpl _parser;
	private final DatagramPacket _packet;
	private final MessageIterator _iterator;
	
	public UdpSubscriber(String address, int port, int bufferSize) throws IOException{
		_address = address;
		_port = port;
		_buffer = ByteBuffer.allocate(bufferSize);
		_buffer.order(ByteOrder.LITTLE_ENDIAN);
		_socket = new MulticastSocket(_port);		
		_packet = new DatagramPacket(_buffer.array(), _buffer.capacity());
		_parser = new MessageParserImpl(_buffer, _packet);
		_iterator = new MessageIterator(_parser);
	}
	
	public MulticastSocket getSocket(){
		return _socket;
	}
	
	public void start() throws IOException{
		InetAddress inet = InetAddress.getByName(_address);
		_socket.joinGroup(inet);
	}

	public Iterator<Message> readNext() throws IOException{
		_socket.receive(_packet);
		_iterator.setLength(_packet.getLength());
		_buffer.rewind();
		return _iterator;
	}
	
	public void stop() throws IOException{
		InetAddress inet = InetAddress.getByName(_address);
		_socket.leaveGroup(inet);
		_socket.close();
	}
	
}

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

	public UdpSubscriber(String address, int port, int bufferSize) {
		_address = address;
		_port = port;
		_buffer = ByteBuffer.allocate(bufferSize);
		_buffer.order(ByteOrder.LITTLE_ENDIAN);
		try {
			_socket = new MulticastSocket(_port);
		} catch (IOException e) {
			throw new InvalidSocketException(e);
		}
		_packet = new DatagramPacket(_buffer.array(), _buffer.capacity());
		_parser = new MessageParserImpl(_buffer, _packet);
		_iterator = new MessageIterator(_parser);
	}

	public MulticastSocket getSocket() {
		return _socket;
	}

	public void connect() {
		try {
			InetAddress inet = InetAddress.getByName(_address);
			_socket.joinGroup(inet);
		} catch (IOException exc) {
			throw new InvalidSocketException(exc);
		}
	}

	public Iterator<Message> readNext() {
		try {
			_socket.receive(_packet);
		} catch (IOException e) {
			throw new InvalidSocketException(e);
		}
		_iterator.setLength(_packet.getLength());
		_buffer.rewind();
		return _iterator;
	}

	public void stop() {
		try {
			InetAddress inet = InetAddress.getByName(_address);
			_socket.leaveGroup(inet);
			_socket.close();
		} catch (IOException exc) {
			throw new InvalidSocketException(exc);
		}
	}

}

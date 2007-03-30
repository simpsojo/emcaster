package com.emcaster.topics;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.net.UnknownHostException;

public class UdpPublisher implements DatagramPacketPublisher {

	private final String _address;

	private final int _port;

	private final MulticastSocket _socket;

	public UdpPublisher(String address, int port) {
		_address = address;
		_port = port;
		try {
			_socket = new MulticastSocket(_port);
		} catch (IOException exc) {
			throw new InvalidSocketException(exc);
		}
	}

	public MessageBuffer createBuffer(int size){
		return new MessageBuffer(size, getAddress(), getPort());
	}
	
	public MulticastSocket getSocket() {
		return _socket;
	}

	public InetAddress getAddress() {
		try {
			return InetAddress.getByName(_address);
		} catch (UnknownHostException exc) {
			throw new RuntimeException(exc);
		}
	}

	public int getPort() {
		return _port;
	}

	public void connect() {
		try {
			_socket.joinGroup(getAddress());
		} catch (IOException exc) {
			throw new InvalidSocketException(exc);
		}
	}

	public void stop() throws IOException {
		InetAddress inet = InetAddress.getByName(_address);
		_socket.leaveGroup(inet);
		_socket.close();
	}

	public void publish(DatagramPacket packet) {
		try {
			_socket.send(packet);
		} catch (IOException failed) {
			throw new PublishException(failed);
		}
	}

}

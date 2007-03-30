package com.emcaster.topics;

import java.net.InetAddress;

public interface Message {


	/**
	 * @return originating address of message
	 */
	InetAddress getAddress();	
	
	/**
	 * @return originating port
	 */
	int getPort();
	
	String getTopic();
	byte[] getMessage();
	
	/**
	 * creates a defensive copy of the message. when a message arrives it is read directly from the 
	 * underlying buffer. copy needs to be called to copy values out of the network buffer. Copy the message
	 * if the message is accessed on another thread or a long lived reference is needed otherwise the message values
	 * will be overwritten the next time a message is read from the socket.
	 * @return
	 */
	Message copy();
	
}

package com.emcaster.topics;

import java.net.DatagramPacket;

public interface DatagramPacketPublisher {

	public void publish(DatagramPacket packet);
	
}

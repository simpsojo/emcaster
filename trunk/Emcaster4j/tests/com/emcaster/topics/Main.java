package com.emcaster.topics;

import java.io.IOException;
import java.util.Iterator;

import com.emcaster.topics.Message;
import com.emcaster.topics.TopicPublisherImpl;
import com.emcaster.topics.TopicSubscriberImpl;

public class Main {

	public static void main(String[] args) throws Exception {
		String address = "224.0.0.23";
		int port = 8001;
		// receive(address, port);
		send(address, port);
	}

	private static void send(String address, int port) throws IOException,
			InterruptedException {
		TopicPublisherImpl pub = new TopicPublisherImpl(address, port,
				1024 * 64);
		pub.start();
		int count = 0;
		while (true) {
			String msg = "msg: " + count;
			pub.publish("test", msg.getBytes());
		}
	}

	private static void receive(String address, int port) throws IOException {
		TopicSubscriberImpl sub = new TopicSubscriberImpl(address, port,
				64 * 1024);
		sub.start();
		long count = 0;
		long startTime = System.currentTimeMillis();
		while (true) {
			Iterator<Message> iter = sub.readNext();
			while (iter.hasNext()) {
				iter.next();
				count++;
				if ((count % 500000) == 0) {
					long totalTime = System.currentTimeMillis() - startTime;
					System.out.println("count: " + count);
					double avg = count / (totalTime / 1000.00);
					System.out.println("avg/sec: " + avg);
					count = 0;
					startTime = System.currentTimeMillis();
				}
			}
		}
	}
}

package com.emcaster.topics;

import java.io.IOException;
import java.util.ArrayList;
import java.util.regex.Pattern;

import junit.framework.TestCase;

public class RoundTripTest extends TestCase {

	public void testSendAndReceive() throws Exception {
		String address = "224.0.0.23";
		int port = 8001;
		final UdpSubscriber subscriber = new UdpSubscriber(address, port, 1024);
		subscriber.start();
		final ArrayList<Message> list = new ArrayList<Message>();
		MessageListener runner = new MessageListener() {
			public void onMessage(Message msg) {
				list.add(msg);
			}
		};

		Pattern pattern = Pattern.compile("topic.*");
		PatternListener patternListener = new PatternListener(pattern, runner);
		final SubscriberRunnable runnable = new SubscriberRunnable(subscriber);
		runnable.add(patternListener);
		Runnable run = new Runnable() {
			public void run() {
				try {
					runnable.dispatchNext();
					runnable.dispatchNext();
					runnable.dispatchNext();
					subscriber.stop();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		};
		Thread thread = new Thread(run);
		thread.start();
		UdpPublisher publisher = new UdpPublisher(address, port);
		publisher.connect();
		MessageBuffer buffer = publisher.createBuffer(1024);
		buffer.publish(publisher, "no match", "msg 0".getBytes());
		buffer.publish(publisher, "topic 1", "msg 1".getBytes());
		buffer.publish(publisher, "topic 2", "msg 2".getBytes());
		publisher.stop();
		thread.join(10000);
		assertEquals(2, list.size());
		Message msg = list.get(0);
		assertEquals("topic 1", msg.getTopic());
		assertEquals("msg 1", new String(msg.getMessage()));
		assertNotNull(msg.getAddress());
		assertEquals(port, msg.getPort());

		msg = list.get(1);
		assertEquals("topic 2", msg.getTopic());
		assertEquals("msg 2", new String(msg.getMessage()));
		assertNotNull(msg.getAddress());
		assertEquals(port, msg.getPort());
	}
}

package com.emcaster.topics;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.regex.Pattern;

import junit.framework.TestCase;

public class RoundTripTest extends TestCase {

	private static void readNext(final UdpSubscriber subscriber, final ArrayList<Message> msg) {
		Iterator<Message> iter = null;
		try {
			iter = subscriber.readNext();
		} catch (IOException e) {
			e.printStackTrace();
		}
		while(iter.hasNext()){
			//add copy to list. don't keep original b/c it references the internal buffer
			msg.add(iter.next().copy());
		}
	};	
	
	public void testSendAndReceive() throws Exception{
		String address = "224.0.0.23";
		int port = 8001;
		final UdpSubscriber subscriber = new UdpSubscriber(address, port,1024);
		subscriber.start();
		final ArrayList<Message> list = new ArrayList<Message>();
		MessageListener runner = new MessageListener(){
			public void onMessage(Message msg) {
				list.add(msg);
			}
		};

		Pattern pattern = Pattern.compile("topic.*");
		PatternListener patternListener = new PatternListener(pattern, runner);
		final SubscriberRunnable runnable = new SubscriberRunnable(subscriber);
		runnable.add(patternListener);
		Runnable run = new Runnable(){
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
		TopicPublisherImpl publisher = new TopicPublisherImpl(address, port, 1024);
		publisher.start();
		publisher.publish("no match", "msg 0".getBytes());
		publisher.publish("topic 1", "msg 1".getBytes());
		publisher.publish("topic 2", "msg 2".getBytes());
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

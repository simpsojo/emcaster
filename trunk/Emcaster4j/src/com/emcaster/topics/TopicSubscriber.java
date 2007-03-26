package com.emcaster.topics;

import java.util.Iterator;

public interface TopicSubscriber {

	void connect();	
	Iterator<Message> readNext();

}

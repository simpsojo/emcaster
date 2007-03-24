package com.emcaster.topics;

import java.io.IOException;
import java.util.Iterator;

public interface TopicSubscriber {

	void start() throws IOException;	
	Iterator<Message> readNext() throws IOException;

}

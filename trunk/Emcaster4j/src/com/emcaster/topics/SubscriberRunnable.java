package com.emcaster.topics;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Iterator;

public class SubscriberRunnable implements Runnable{

	private final TopicSubscriber _parser;
	private boolean _running = true;
	private ArrayList<MessageListener> _listeners = new ArrayList<MessageListener>();
	private boolean _copy = true;
	
	public SubscriberRunnable(TopicSubscriber parser){
		_parser = parser;		
	}
	
	public void setCopy(boolean b){
		_copy = b;
	}

	public boolean getCopy(){
		return _copy;
	}
	
	public void run(){
		try{
			dispatchMessages();
		}catch(Exception failed){
			throw new RuntimeException(failed);
		}
	}
	
	public void dispatchMessages() throws IOException {
		while(_running){
			Iterator<Message> msgs = _parser.readNext();
			while(msgs.hasNext()){
				Message next = msgs.next();
				if(_copy){
					next = next.copy();
				}
				for (MessageListener listener : _listeners) {
					listener.onMessage(next);
				}
			}
		}
	}	
}

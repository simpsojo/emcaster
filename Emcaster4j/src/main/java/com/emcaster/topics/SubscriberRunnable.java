package com.emcaster.topics;

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
	
	/**
	 * determines whether the messages is copied from
	 * the I/O buffer before passing it to listeners.
	 * @param b
	 */
	public void setCopy(boolean b){
		_copy = b;
	}

	public boolean getCopy(){
		return _copy;
	}
	
	public void add(MessageListener listener){
		_listeners.add(listener);
	}
	
	public void run(){
			dispatchMessages();
	}
	
	public void dispatchMessages(){
		while(_running){
			dispatchNext();
		}
	}

	void dispatchNext() {
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

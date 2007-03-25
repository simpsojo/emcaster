package com.emcaster.topics;

import java.util.Iterator;

public class MessageIterator implements Iterator<Message>{

	private int _length;
	private final MessageParser _parser;
	
	public MessageIterator(MessageParser parser){
		_parser = parser;
	}
	
	public void setLength(int length){
		_length = length;
	}
	
	public int getLength(){
		return _length;
	}
	
	public boolean hasNext() {
		return _parser.getPosition() < _length;
	}

	public Message next() {
		_parser.parseNextMessage();
		return _parser;
	}

	public void remove() {
		
	}

}

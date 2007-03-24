package com.emcaster.topics;

public interface MessageParser extends Message{
	
	int getPosition();
	void parseNextMessage();
}

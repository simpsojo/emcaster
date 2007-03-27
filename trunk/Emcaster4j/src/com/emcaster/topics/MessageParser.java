package com.emcaster.topics;

public interface MessageParser {
	
	int getPosition();
	Message parseNextMessage();
}

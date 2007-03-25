package com.emcaster.topics;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class PatternListener implements MessageListener {
	
	private final Pattern _pattern;
	private final MessageListener _listener;
	
	public PatternListener(Pattern pattern, MessageListener listener){
		_pattern = pattern;
		_listener = listener;
	}
	
	public void onMessage(Message msg){
		Matcher matcher = _pattern.matcher(msg.getTopic());
		if(matcher.matches()){
			_listener.onMessage(msg);
		}
	}
}

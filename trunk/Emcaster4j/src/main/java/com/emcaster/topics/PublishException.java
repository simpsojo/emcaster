package com.emcaster.topics;

public class PublishException extends RuntimeException {

	public PublishException(Exception exc){
		super(exc);
	}
}

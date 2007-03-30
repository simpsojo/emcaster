package com.emcaster.topics;

public class InvalidSocketException extends RuntimeException {

	public InvalidSocketException(Exception exc){
		super(exc);
	}
}

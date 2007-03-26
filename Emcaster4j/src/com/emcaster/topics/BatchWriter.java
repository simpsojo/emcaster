package com.emcaster.topics;

import java.net.InetAddress;

/**
 * Writes messages to a pending batch that is periodically flushed. The class
 * is thread safe. Provides optimal performance for small messages.
 * 
 * @author mrettig
 */
public class BatchWriter implements Runnable {

	private final Object _lock = new Object();
	
	private int _afterFlushSleep = 0;
	private boolean _running = true;
	private MessageBuffer _pendingBatch;
	private MessageBuffer _flushingBatch;
	private final DatagramPacketPublisher _publisher;

	public BatchWriter(int size, DatagramPacketPublisher publisher, InetAddress address, int port){
		_pendingBatch = new MessageBuffer(size, address, port);
		_flushingBatch = new MessageBuffer(size, address, port);
		_publisher = publisher;
	}

	public void setAfterFlushSleep(int time){
		_afterFlushSleep = time;
	}
	public int getAfterFlushSleep(){
		return _afterFlushSleep;
	}
	
	public void publish(String topic, byte[] msg, int offset, int length){
		synchronized(_lock){
			while(!_pendingBatch.appendMessage(topic, msg, offset, length)
					&& _running){
				try{
					_lock.wait();
				} catch(InterruptedException exc){
					throw new RuntimeException(exc);
				}
			}
			_lock.notifyAll();
		}
	}
	
	public void run() {
		while(_running){
			getNextPacket();
			if(!_running){
				return;
			}
			_flushingBatch.writeTo(_publisher);
			if(_afterFlushSleep >= 0){
				try{
					Thread.sleep(_afterFlushSleep);
				}catch(InterruptedException exc){
					
				}
			}
		}		
	}

	private void getNextPacket() {
		synchronized(_lock){
			while(_pendingBatch.getLength() == 0 && _running){
				try{
					_lock.wait();
				}catch(InterruptedException exc){					
				}
			}
			if(!_running){
				return;
			}
			MessageBuffer temp = _pendingBatch;
			_pendingBatch = _flushingBatch;
			_flushingBatch = temp;
			_lock.notifyAll();
		}
	}
	
	public void stop(){
		synchronized(_lock){
			_running = false;
			_lock.notifyAll();
		}
	}
}

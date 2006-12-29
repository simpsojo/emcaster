using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;

namespace Emcaster.Sockets
{

    /// <summary>
    /// Writes bytes to a pending buffer. Thread safe for several writing threads.
    /// </summary>
    public class AsyncByteWriter: IByteWriter
    {
        private static ILog log = LogManager.GetLogger(typeof(AsyncByteWriter));

        private object _lock = new object();

        private MemoryStream _pendingBuffer;
        private MemoryStream _flushBuffer;
        private Socket _target;
        private bool _running = true;
        private int _minFlushSize = 100;
        private int _sleepOnMin = 1;

        public AsyncByteWriter(PgmPublisher pubber, int maxBufferSize)
            :this(pubber.Socket, maxBufferSize)
        {
        }

        public AsyncByteWriter(Socket target, int maxBufferSize)
        {
            _target = target;
            _pendingBuffer = new MemoryStream(maxBufferSize);
            _flushBuffer = new MemoryStream(maxBufferSize);
        }

        public int SleepOnMin
        {
            set { _sleepOnMin = value; }
        }

        public int MinFlushSize
        {
            set { _minFlushSize = value; }
        }

        /// <summary>
        /// Add bytes to the buffer. If the buffer is full, then the thread waits for
        /// the buffer to be flushed by the flushing thread. Thread Safe.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns>true if bytes are bufferred. false if the wait times out.</returns>
        public bool Write(byte[] buffer, int offset, int size)
        {
            lock (_lock)
            {
                if ((_pendingBuffer.Length + size) > _pendingBuffer.Capacity && _running)
                {
                    return false;
                }
                if (!_running)
                    return false;

                _pendingBuffer.Write(buffer, offset, size);
                // other threads could be waiting to buffer Or the flush thread 
                // could be waiting to retry.
                Monitor.PulseAll(_lock);
                return true;
            }
        }

        internal void FlushBuffer()
        {
            lock (_lock)
            {
                while (_running && _pendingBuffer.Length == 0)
                {
                    Monitor.Wait(_lock);
                }
                if (!_running)
                {
                    return;
                }
                MemoryStream swap = _flushBuffer;
                _flushBuffer = _pendingBuffer;
                _pendingBuffer = swap;
                // there may be many threads waiting to add to the buffer.
                Monitor.PulseAll(_lock);
            }
            long length = _flushBuffer.Length;
            if (length > 0)
            {
                try
                {
                    byte[] allData = _flushBuffer.ToArray();
                    _target.Send(allData);
                    log.Debug(GetType().Name + " Flushed " + _flushBuffer.Length);
                }
                catch (Exception failed)
                {
                    log.Error("Async Flush Failed msg: " + failed.Message + " stack: " + failed.StackTrace);
                }
                _flushBuffer.Position = 0;
                _flushBuffer.SetLength(0);
                if (length < _minFlushSize)
                {
                    Thread.Sleep(_sleepOnMin);
                }
            }
        }

        internal void FlushRunner()
        {
            log.Info("Started Flush Thread for " + GetType().FullName);
            while (_running)
            {
                FlushBuffer();
            }
        }

        public void Start()
        {
            WaitCallback callback =
                delegate
                {
                    FlushRunner();
                };
            ThreadPool.QueueUserWorkItem(callback);
        }

        public void Dispose()
        {
            log.Info(GetType().FullName + " Disposed");
            lock (_lock)
            {
                _running = false;
                Monitor.PulseAll(_lock);
            }
        }
    }
}

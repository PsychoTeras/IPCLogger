using System;
using System.Threading;

namespace IPCLogger.Core.Loggers.Base
{
    public abstract class QueueableLogger<T> : BaseLogger<T>
        where T: QueueableSettings
    {

#region Private fields

        private DateTime _lastFlushed;

        private object _lockObj;
        private object _lockFlush;

        private Thread _threadPeriodicFlush;
        private ManualResetEvent _evStopPeriodicFlush;

        private ManualResetEvent _reSusped;
        private volatile bool _suspended;

        private volatile bool _initialized;

#endregion

#region Properties

        protected abstract bool ShouldFlushQueue { get; }

#endregion

#region Ctor

        protected QueueableLogger()
        {
            _lockObj = new object();
            _lockFlush = new object();
        }

#endregion

#region ILogger

        protected override void OnSetupSettings()
        {
            lock (_lockObj)
            {
                if (_initialized)
                {
                    Deinitialize();
                    Initialize();
                }
            }
        }

        protected abstract void InitializeQueue();

        public override void Initialize()
        {
            lock (_lockObj)
            {
                if (_initialized) return;

                try
                {
                    InitializeQueue();

                    _lastFlushed = DateTime.UtcNow;
                    _reSusped = new ManualResetEvent(true);

                    if (Settings.QueueSize > 0)
                    {
                        _evStopPeriodicFlush = new ManualResetEvent(false);
                        _threadPeriodicFlush = new Thread(DoPeriodicFlush);
                        _threadPeriodicFlush.IsBackground = true;
                        _threadPeriodicFlush.Start();
                    }

                    _initialized = true;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Initialize failed for {0}", this);
                    CatchLoggerException(msg, ex);
                }
            }
        }

        protected abstract void DeinitializeQueue();

        public override void Deinitialize()
        {
            lock (_lockObj)
            {
                if (!_initialized) return;

                try
                {
                    if (_threadPeriodicFlush != null)
                    {
                        _reSusped.Set();
                        _evStopPeriodicFlush.Set();
                        _threadPeriodicFlush.Join();
                        _threadPeriodicFlush = null;
                    }

                    if (_initialized)
                    {
                        Flush();
                    }

                    DeinitializeQueue();

                    _initialized = false;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Deinitialize failed for {0}", this);
                    CatchLoggerException(msg, ex);
                }
            }
        }

        protected abstract void WriteQueue(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine);

        protected internal override void Write(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine)
        {
            lock (_lockObj)
            {
                if (!_initialized) return;

                WriteQueue(callerType, eventType, eventName, text, writeLine);

                if (ShouldFlushQueue)
                {
                    Flush();
                }
            }
        }

        protected abstract void FlushQueue();

        public override void Flush()
        {
            lock (_lockFlush)
            {
                if (!_initialized) return;

                try
                {
                    FlushQueue();
                    _lastFlushed = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Flush failed for {0}", this);
                    CatchLoggerException(msg, ex);
                }
            }
        }

        protected virtual bool SuspendQueue() { return true; }

        public override bool Suspend()
        {
            lock (_lockObj)
            {
                if (!_initialized || _suspended) return false;
                
                try
                {
                    _suspended = SuspendQueue();
                    if (_suspended)
                    {
                        _reSusped.Reset();
                    }
                    return _suspended;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Suspend failed for {0}", this);
                    CatchLoggerException(msg, ex);
                }
                return false;
            }
        }

        protected virtual bool ResumeQueue() { return true; }

        public override bool Resume()
        {
            lock (_lockObj)
            {
                if (!_initialized || !_suspended) return false;
                try
                {
                    if (ResumeQueue())
                    {
                        _suspended = false;
                        _reSusped.Set();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Resume failed for {0}", this);
                    CatchLoggerException(msg, ex);
                }
                return false;
            }
        }

#endregion

#region Class methods

        private void DoPeriodicFlush()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                TimeSpan queueAge = DateTime.UtcNow - _lastFlushed;
                if (queueAge.TotalSeconds >= Settings.MaxQueueAge &&
                    _reSusped.WaitOne() && Thread.CurrentThread.IsAlive)
                {
                    lock (_lockObj)
                    {
                        Flush();
                    }
                }
                if (_evStopPeriodicFlush.WaitOne(Settings.PeriodicFlushInterval))
                {
                    return;
                }
            }
        }

#endregion

    }
}

using System;
using System.Threading;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Loggers.Base
{
    public abstract class QueueableLogger<T> : BaseLogger<T>
        where T: QueueableSettings
    {

#region Private fields

        private LightLock _lockObj;
        private bool _shouldLock;

        private Thread _threadPeriodicFlush;
        private ManualResetEvent _evStopPeriodicFlush;

        private ManualResetEvent _reSusped;
        private volatile bool _suspended;

#endregion

#region Protected fields

        protected bool Initialized { get; private set; }

#endregion

#region Properties

        protected abstract bool ShouldFlushQueue { get; }

#endregion

#region Ctor

        protected QueueableLogger(bool threadSafetyIsGuaranteed)
            : base(threadSafetyIsGuaranteed)
        {
            _lockObj = new LightLock();
            _shouldLock = !threadSafetyIsGuaranteed;
        }

#endregion

#region ILogger

        protected override void OnSetupSettings()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized) return;
                Deinitialize();
                Initialize();
            }
            catch (Exception ex)
            {
                string msg = string.Format("OnSetupSettings failed for {0}", this);
                CatchLoggerException(msg, ex);
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

        protected abstract bool InitializeQueue();

        public override void Initialize()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (Initialized) return;

                Initialized = InitializeQueue();
                if (Initialized)
                {
                    _reSusped = new ManualResetEvent(true);

                    _shouldLock |= Settings.MaxQueueAge > 0;
                    if (Settings.MaxQueueAge > 0)
                    {
                        _evStopPeriodicFlush = new ManualResetEvent(false);
                        _threadPeriodicFlush = new Thread(DoPeriodicFlush);
                        _threadPeriodicFlush.IsBackground = true;
                        _threadPeriodicFlush.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Initialize failed for {0}", this);
                CatchLoggerException(msg, ex);
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

        protected abstract bool DeinitializeQueue();

        public override void Deinitialize()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized) return;

                if (_threadPeriodicFlush != null)
                {
                    _reSusped.Set();
                    _evStopPeriodicFlush.Set();
                    _threadPeriodicFlush.Join();
                    _threadPeriodicFlush = null;
                }

                if (Initialized)
                {
                    Flush();

                    Initialized = !DeinitializeQueue();

                    if (!Initialized)
                    {
                        _suspended = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Deinitialize failed for {0}", this);
                CatchLoggerException(msg, ex);
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

        protected abstract void WriteQueue(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine);

        protected internal override void Write(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine, bool immediateFlush)
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized || _suspended) return;
                WriteQueue(callerType, eventType, eventName, text, writeLine);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Write failed for {0}", this);
                CatchLoggerException(msg, ex);
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }

            if (immediateFlush || ShouldFlushQueue)
            {
                Flush();
            }
        }

        protected abstract void FlushQueue();

        public override void Flush()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized) return;
                FlushQueue();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Flush failed for {0}", this);
                CatchLoggerException(msg, ex);
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

        protected virtual bool SuspendQueue() { return true; }

        public override bool Suspend()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized || _suspended) return false;

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
                return false;
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

        protected virtual bool ResumeQueue() { return true; }

        public override bool Resume()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized || !_suspended) return false;

                _suspended = !ResumeQueue();
                if (!_suspended)
                {
                    _reSusped.Set();
                }
                return !_suspended;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Resume failed for {0}", this);
                CatchLoggerException(msg, ex);
                return false;
            }
            finally
            {
                _lockObj.Set(_shouldLock);
            }
        }

#endregion

#region Class methods

        private void DoPeriodicFlush()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                if (_evStopPeriodicFlush.WaitOne(Settings.MaxQueueAge)) return;

                if (_reSusped.WaitOne() && Thread.CurrentThread.IsAlive)
                {
                    Flush();
                }
            }
        }

#endregion

    }
}

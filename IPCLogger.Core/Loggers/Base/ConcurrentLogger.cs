using System;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Loggers.Base
{
    public abstract class ConcurrentLogger<T> : BaseLogger<T>
        where T: BaseSettings
    {

#region Private fields

        private LightLock _lockObj;
        private bool _shouldLock;

        private volatile bool _suspended;

#endregion

#region Protected fields

        protected bool Initialized { get; private set; }

#endregion

#region Ctor

        protected ConcurrentLogger(bool threadSafetyIsGuaranteed) 
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

        protected abstract void WriteConcurrent(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine);

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            string text, bool writeLine, bool immediateFlush)
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized) return;
                WriteConcurrent(callerType, eventType, eventName, text, writeLine);
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

            if (immediateFlush)
            {
                Flush();
            }
        }

        protected abstract bool InitializeConcurrent();

        public override void Initialize()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized)
                {
                    Initialized = InitializeConcurrent();
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

        protected abstract bool DeinitializeConcurrent();

        public override void Deinitialize()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (Initialized)
                {
                    Initialized = !DeinitializeConcurrent();
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

        protected virtual void FlushConcurrent() { }

        public override void Flush()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized) return;
                FlushConcurrent();
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

        protected virtual bool SuspendConcurrent() { return true; }

        public override bool Suspend()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized || _suspended) return false;
                return _suspended = SuspendConcurrent();
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

        protected virtual bool ResumeConcurrent() { return true; }

        public override bool Resume()
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized || !_suspended) return false;
                return !(_suspended = !ResumeConcurrent());
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

    }
}

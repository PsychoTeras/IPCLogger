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
        protected bool InSetupSettings { get; private set; }

#endregion

#region Ctor

        protected ConcurrentLogger(bool threadSafetyGuaranteed) 
            : base(threadSafetyGuaranteed)
        {
            _lockObj = new LightLock();
            _shouldLock = !threadSafetyGuaranteed;
        }

#endregion
       
#region ILogger

        protected override void OnSetupSettings()
        {
            _lockObj.WaitOne(_shouldLock);
            InSetupSettings = true;
            try
            {
                if (!Initialized) return;
                Deinitialize();
                Initialize();
            }
            catch (Exception ex)
            {
                string msg = $"OnSetupSettings failed for {this}";
                CatchLoggerException(msg, ex);
            }
            finally
            {
                InSetupSettings = false;
                _lockObj.Set(_shouldLock);
            }
        }

        protected abstract void WriteConcurrent(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine);

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine, bool immediateFlush)
        {
            _lockObj.WaitOne(_shouldLock);
            try
            {
                if (!Initialized || _suspended) return;
                WriteConcurrent(callerType, eventType, eventName, data, text, writeLine);
            }
            catch (Exception ex)
            {
                string msg = $"Write failed for {this}";
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
                string msg = $"Initialize failed for {this}";
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
                    if (!Initialized)
                    {
                        _suspended = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = $"Deinitialize failed for {this}";
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
                string msg = $"Flush failed for {this}";
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
                string msg = $"Suspend failed for {this}";
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
                string msg = $"Resume failed for {this}";
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

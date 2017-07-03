using System;
using System.Threading;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Loggers.Base
{
    public abstract class ConcurrentLogger<T> : BaseLogger<T>
        where T: BaseSettings
    {

#region Private fields

        private LightLock _lockObj;
        private bool _shouldLock;
        private Thread _onSetupSettingsThread;

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
                _onSetupSettingsThread = Thread.CurrentThread;
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
                _onSetupSettingsThread = null;
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
            if (_onSetupSettingsThread == null || _onSetupSettingsThread != Thread.CurrentThread)
            {
                _lockObj.WaitOne();
            }
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
                if (_onSetupSettingsThread == null || _onSetupSettingsThread != Thread.CurrentThread)
                {
                    _lockObj.Set();
                }
            }
        }

        protected abstract bool DeinitializeConcurrent();

        public override void Deinitialize()
        {
            if (_onSetupSettingsThread == null || _onSetupSettingsThread != Thread.CurrentThread)
            {
                _lockObj.WaitOne(_shouldLock);
            }
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
                if (_onSetupSettingsThread == null || _onSetupSettingsThread != Thread.CurrentThread)
                {
                    _lockObj.Set(_shouldLock);
                }
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

#endregion

    }
}

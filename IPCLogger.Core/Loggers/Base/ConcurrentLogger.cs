using System;

namespace IPCLogger.Core.Loggers.Base
{
    public abstract class ConcurrentLogger<T> : BaseLogger<T>
        where T: BaseSettings
    {

#region Private fields

        private object _lockObj;
        private volatile bool _initialized;

#endregion

#region Ctor

        protected ConcurrentLogger()
        {
            _lockObj = new object();
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

        protected abstract void WriteConcurrent(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine);

        protected internal override void Write(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine, bool immediateFlush)
        {
            lock (_lockObj)
            {
                if (_initialized)
                {
                    WriteConcurrent(callerType, eventType, eventName, text, writeLine);
                    if (immediateFlush)
                    {
                        Flush();
                    }
                }
            }
        }

        protected abstract void InitializeConcurrent();

        public override void Initialize()
        {
            lock (_lockObj)
            {
                if (_initialized) return;

                try
                {
                    InitializeConcurrent();
                    _initialized = true;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Initialize failed for {0}", this);
                    CatchLoggerException(msg, ex);
                }
            }
        }

        protected abstract void DeinitializeConcurrent();

        public override void Deinitialize()
        {
            lock (_lockObj)
            {
                if (!_initialized) return;
                
                try
                {
                    DeinitializeConcurrent();
                    _initialized = false;
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Deinitialize failed for {0}", this);
                    CatchLoggerException(msg, ex);
                }
            }
        }

        protected virtual void FlushConcurrent() { }

        public override void Flush()
        {
            lock (_lockObj)
            {
                try
                {
                    FlushConcurrent();
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Flush failed for {0}", this);
                    CatchLoggerException(msg, ex);
                }
            }
        }

#endregion

    }
}

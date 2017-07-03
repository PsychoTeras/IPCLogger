using System;

namespace IPCLogger.Core.Loggers.Base
{
    public abstract class SimpleLogger<T> : BaseLogger<T>
        where T: BaseSettings
    {

#region Protected fields

        protected bool Initialized { get; private set; }

#endregion

#region Ctor

        protected SimpleLogger(bool threadSafetyIsGuaranteed) 
            : base(threadSafetyIsGuaranteed)
        {
        }

#endregion
       
#region ILogger

        protected override void OnSetupSettings()
        {
            if (Initialized)
            {
                Deinitialize();
                Initialize();
            }
        }

        protected abstract void WriteSimple(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine);

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            string text, bool writeLine, bool immediateFlush)
        {
            if (Initialized)
            {
                WriteSimple(callerType, eventType, eventName, text, writeLine);
                if (immediateFlush)
                {
                    Flush();
                }
            }
        }

        protected abstract bool InitializeSimple();

        public override void Initialize()
        {
            if (Initialized) return;
            try
            {
                Initialized = InitializeSimple();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Initialize failed for {0}", this);
                CatchLoggerException(msg, ex);
            }
        }

        protected abstract bool DeinitializeSimple();

        public override void Deinitialize()
        {
            if (!Initialized) return;

            try
            {
                Initialized = !DeinitializeSimple();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Deinitialize failed for {0}", this);
                CatchLoggerException(msg, ex);
            }
        }

        protected virtual void FlushSimple() { }

        public override void Flush()
        {
            try
            {
                FlushSimple();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Flush failed for {0}", this);
                CatchLoggerException(msg, ex);
            }
        }

#endregion

    }
}

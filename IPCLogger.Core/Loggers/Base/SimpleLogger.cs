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

        protected SimpleLogger(bool threadSafetyGuaranteed) 
            : base(threadSafetyGuaranteed)
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
            byte[] data, string text, bool writeLine);

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine, bool immediateFlush)
        {
            try
            {
                if (!Initialized) return;
                WriteSimple(callerType, eventType, eventName, data, text, writeLine);
                if (immediateFlush)
                {
                    Flush();
                }
            }
            catch (Exception ex)
            {
                string msg = $"Write failed for {this}";
                CatchLoggerException(msg, ex);
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
                string msg = $"Initialize failed for {this}";
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
                string msg = $"Deinitialize failed for {this}";
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
                string msg = $"Flush failed for {this}";
                CatchLoggerException(msg, ex);
            }
        }

#endregion

    }
}

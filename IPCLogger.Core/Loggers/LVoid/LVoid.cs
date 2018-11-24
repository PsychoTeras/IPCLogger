using IPCLogger.Core.Loggers.Base;
using System;

namespace IPCLogger.Core.Loggers.LVoid
{
    public class LVoid : BaseLogger<LVoidSettings>
    {

#region Ctor

        public LVoid(bool threadSafetyGuaranteed)
            : base(threadSafetyGuaranteed)
        {
        }

#endregion

#region ILogger

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine, bool immediateFlush) { }

        public override void Initialize() { }

        public override void Deinitialize() { }

#endregion


    }
}

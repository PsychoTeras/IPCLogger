using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LVoid
{
    internal sealed class LVoid : BaseLogger<LVoidSettings>
    {

#region Ctor

        public LVoid(bool threadSafetyIsGuaranteed)
            : base(threadSafetyIsGuaranteed)
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

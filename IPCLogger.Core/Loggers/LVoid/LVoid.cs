using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LVoid
{
    internal sealed class LVoid : BaseLogger<LVoidSettings>
    {

#region ILogger

        protected internal override void Write(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine) { }

        public override void Initialize() { }

        public override void Deinitialize() { }

#endregion

    }
}

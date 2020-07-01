using IPCLogger.Loggers.Base;
using System;

namespace IPCLogger.Loggers.LVoid
{
    public sealed class LVoidSettings : BaseSettings
    {

#region Ctor

        public LVoidSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges) { }

#endregion

    }
}

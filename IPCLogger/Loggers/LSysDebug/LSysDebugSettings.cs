using IPCLogger.Loggers.Base;
using System;

namespace IPCLogger.Loggers.LSysDebug
{
    public sealed class LSysDebugSettings : BaseSettings
    {

#region Ctor

        public LSysDebugSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges) { }

#endregion

    }
}
using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LSysDebug
{
    public sealed class LSysDebugSettings : BaseSettings
    {

#region Ctor

        public LSysDebugSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges) { }

#endregion

    }
}
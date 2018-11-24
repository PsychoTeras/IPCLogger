using IPCLogger.Core.Loggers.Base;
using System;

namespace IPCLogger.Core.Loggers.LVoid
{
    public sealed class LVoidSettings : BaseSettings
    {

#region Ctor

        public LVoidSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges) { }

#endregion

    }
}

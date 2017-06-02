using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LVoid
{
    internal sealed class LVoidSettings : BaseSettings
    {

#region Ctor

        public LVoidSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges) { }

#endregion

    }
}

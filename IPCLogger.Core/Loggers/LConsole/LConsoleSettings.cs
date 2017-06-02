using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LConsole
{
    public sealed class LConsoleSettings : BaseSettings
    {

#region Ctor

        public LConsoleSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges) { }

#endregion

    }
}

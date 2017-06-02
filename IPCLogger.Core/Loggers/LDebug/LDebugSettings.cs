using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LDebug
{
    public sealed class LDebugSettings : BaseSettings
    {

#region Properties

        public bool Trace { get; set; }
        public bool ImmediateFlush { get; set; }

#endregion

#region Ctor

        public LDebugSettings(Type loggerType, Action onApplyChanges) 
            : base(loggerType, onApplyChanges)
        {
            Trace = false;
            ImmediateFlush = true;
        }

#endregion

    }
}
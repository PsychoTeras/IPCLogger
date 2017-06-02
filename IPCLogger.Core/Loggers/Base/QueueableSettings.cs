using System;

namespace IPCLogger.Core.Loggers.Base
{
    public abstract class QueueableSettings : BaseSettings
    {

#region Defaults

        protected virtual int DefQueueSize { get { return 100; } }
        protected virtual int DefMaxQueueAge { get { return 3; } }
        public virtual int PeriodicFlushInterval { get { return 1000; } }

#endregion

#region Properties

        public virtual int QueueSize { get; set; }
        public virtual int MaxQueueAge { get; set; }

#endregion

#region Ctor

        protected QueueableSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges)
        {
            QueueSize = DefQueueSize;
            MaxQueueAge = DefMaxQueueAge;
        }

#endregion

    }
}

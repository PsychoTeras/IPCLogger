using System;

namespace IPCLogger.Loggers.Base
{
    public abstract class QueueableSettings : BaseSettings
    {

#region Defaults

        protected virtual int DefQueueSize { get { return 100; } }
        protected virtual int DefMaxQueueAge { get { return 3; } }

#endregion

#region Private fields

        private int _maxQueueAge;

#endregion

#region Properties

        public int MaxQueueAge
        {
            get { return _maxQueueAge; }
            set { _maxQueueAge = value*1000; }
        }

        public virtual int QueueSize { get; set; }

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

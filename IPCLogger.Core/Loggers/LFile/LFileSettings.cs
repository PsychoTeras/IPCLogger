using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LFile
{
    public sealed class LFileSettings : BaseSettings
    {

#region Constants

        private const int QUEUE_SIZE = 32;

#endregion

#region Private fields

        private int _queueSize;

#endregion

#region Properties

        public int QueueSize
        {
            get { return _queueSize; }
            set { _queueSize = value*1024; }
        }
        public string LogDir { get; set; }
        public string LogFile { get; set; }
        public bool RecreateFile { get; set; }

#endregion

#region Ctor

        public LFileSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges)
        {
            QueueSize = QUEUE_SIZE;
        }

#endregion

    }
}

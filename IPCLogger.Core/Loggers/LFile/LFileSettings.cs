using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LFile
{
    public sealed class LFileSettings : BaseSettings
    {

#region Constants

        private const int BUFFER_SIZE = 32;

#endregion

#region Private fields

        private int _bufferSize;

#endregion

#region Properties

        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value*1024; }
        }
        public string LogDir { get; set; }
        public string LogFile { get; set; }
        public bool RecreateFile { get; set; }

#endregion

#region Ctor

        public LFileSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges)
        {
            BufferSize = BUFFER_SIZE;
        }

#endregion

    }
}

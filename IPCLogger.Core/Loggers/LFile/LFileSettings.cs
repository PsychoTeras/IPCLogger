using System;
using IPCLogger.Core.Attributes;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LFile
{
    public sealed class LFileSettings : BaseSettings
    {

#region Constants

        private const int BUFFER_SIZE = 32768;

#endregion

#region Properties

        [BytesConversion]
        public int BufferSize { get; set; }

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

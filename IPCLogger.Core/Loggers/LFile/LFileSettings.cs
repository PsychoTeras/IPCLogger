using System;
using System.IO;
using IPCLogger.Core.Attributes;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LFile
{
    public sealed class LFileSettings : BaseSettings
    {

#region Constants

        private const int BUFFER_SIZE = 32*1024;

#endregion

#region Properties

        [BytesStringConversion]
        public int BufferSize { get; set; }

        public string LogDir { get; set; }

        public string LogFile { get; set; }

        public bool RecreateFile { get; set; }

        [BytesStringConversion]
        public int MaxFileSize { get; set; }

        [TimeStringConversion]
        public TimeSpan MaxFileAge { get; set; }

        [NonSetting]
        internal bool RollByFileSize { get; private set; }

        [NonSetting]
        internal bool RollByFileAge { get; private set; }

        [NonSetting]
        internal string LogFileName { get; private set; }

        [NonSetting]
        internal string LogFileExt { get; private set; }

        [NonSetting]
        internal bool RollingIsEnabled { get; private set; }

#endregion

#region Ctor

        public LFileSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges)
        {
            BufferSize = BUFFER_SIZE;
        }

#endregion

#region Class methods

        public override void FinalizeSetup()
        {
            RollByFileSize = MaxFileSize > 0;
            RollByFileAge = MaxFileAge.Ticks > 0;
            LogFileName = Path.GetFileNameWithoutExtension(LogFile);
            LogFileExt = Path.GetExtension(LogFile);
            RollingIsEnabled = RollByFileAge || RollByFileSize;
        }

#endregion

    }
}

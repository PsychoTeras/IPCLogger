﻿using System;
using System.IO;
using IPCLogger.Core.Attributes;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LFile
{
    public sealed class LFileSettings : BaseSettings
    {

#region Constants

        private const int BUFFER_SIZE = 32*1024;

        internal static readonly string IdxPlaceMark = "?*";

#endregion

#region Properties

        [SizeStringConversion]
        public int BufferSize { get; set; }

        public string LogDir { get; set; }

        public string LogFile { get; set; }

        public bool RecreateFile { get; set; }

        [SizeStringConversion]
        public int MaxFileSize { get; set; }

        [TimeStringConversion]
        public TimeSpan MaxFileAge { get; set; }

        public bool DynamicFilePath { get; set; }

        [NonSetting]
        internal bool RollByFileSize { get; private set; }

        [NonSetting]
        internal bool RollByFileAge { get; private set; }

        [NonSetting]
        internal string ExpandedLogFilePathWithMark { get; private set; }

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
            ExpandedLogFilePathWithMark = string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(LogFile),
                IdxPlaceMark, Path.GetExtension(LogFile));
            ExpandedLogFilePathWithMark = Path.Combine(LogDir, ExpandedLogFilePathWithMark);
            ExpandedLogFilePathWithMark = Environment.ExpandEnvironmentVariables(ExpandedLogFilePathWithMark);
        }

#endregion

    }
}

using System;
using System.Diagnostics;
using System.IO;
using IPCLogger.Core.Attributes;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LEventLog
{
    public sealed class LEventLogSettings : BaseSettings
    {

#region Constants

        private const string MACHINE_NAME = ".";

        private static readonly string SOURCE = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);

        private const OverflowAction OVERFLOW_ACTION = OverflowAction.OverwriteOlder;

        private const int OVERFLOW_ACTION_RETENTION_DAYS = 30;

        private const int MAX_LOG_SIZE = 512*1024;

#endregion

#region Properties

        public string MachineName { get; set; }

        public string LogName { get; set; }

        public string Source { get; set; }

        public short EventId { get; set; }

        public int Category { get; set; }

        [SizeStringConversion]
        public long MaxLogSize { get; set; }

        public OverflowAction OverflowAction { get; set; }

        public int OverwriteOlderRetentionDays { get; set; }

#endregion

#region Ctor

        public LEventLogSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges)
        {
            MachineName = MACHINE_NAME;
            Source = LogName = SOURCE;
            OverflowAction = OVERFLOW_ACTION;
            OverwriteOlderRetentionDays = OVERFLOW_ACTION_RETENTION_DAYS;
            MaxLogSize = MAX_LOG_SIZE;
        }

        protected override void FinalizeSetup()
        {
            MaxLogSize = Math.Max(MaxLogSize, 65536);
            long remainder = MaxLogSize%65536;
            if (remainder > 0)
            {
                MaxLogSize += 65536 - remainder;
            }
            MaxLogSize /= 1024;
        }

#endregion

    }
}

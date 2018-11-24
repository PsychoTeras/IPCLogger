using IPCLogger.Core.Attributes;
using IPCLogger.Core.Loggers.Base;
using System;
using System.IO;
using System.Web.Hosting;

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
        public long BufferSize { get; set; }

        public string LogDir { get; set; }

        [RequiredSetting]
        public string LogFile { get; set; }

        public bool RecreateFile { get; set; }

        [SizeStringConversion]
        public long MaxFileSize { get; set; }

        [TimeStringConversion]
        public TimeSpan MaxFileAge { get; set; }

        public string NetUser { get; set; }

        public string NetPassword { get; set; }

        [NonSetting]
        internal bool RollByFileSize { get; private set; }

        [NonSetting]
        internal bool RollByFileAge { get; private set; }

        [NonSetting]
        internal string ExpandedLogFilePathWithMark { get; private set; }

        [NonSetting]
        internal bool ConnectNetShare { get; private set; }

#endregion

#region Ctor

        public LFileSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges)
        {
            BufferSize = BUFFER_SIZE;
        }

#endregion

#region Class methods

        protected override void FinalizeSetup()
        {
            RollByFileSize = MaxFileSize > 0;
            RollByFileAge = MaxFileAge.Ticks > 0;
            ExpandedLogFilePathWithMark = $"{Path.GetFileNameWithoutExtension(LogFile)}{IdxPlaceMark}{Path.GetExtension(LogFile)}";

            string logDir = LogDir ?? string.Empty;
            if (logDir.StartsWith("~\\"))
            {
                logDir = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, logDir.Remove(0, 2));
            }

            ExpandedLogFilePathWithMark = Path.Combine(logDir, ExpandedLogFilePathWithMark);
            ExpandedLogFilePathWithMark = Environment.ExpandEnvironmentVariables(ExpandedLogFilePathWithMark);

            ConnectNetShare = !string.IsNullOrWhiteSpace(NetUser);
        }

#endregion

    }
}

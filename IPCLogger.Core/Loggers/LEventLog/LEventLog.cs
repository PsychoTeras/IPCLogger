using System;
using System.Diagnostics;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LEventLog
{
    public sealed class LEventLog : SimpleLogger<LEventLogSettings>
    {

#region Private fields

        private EventLog _eventLog;

#endregion

#region Ctor

        public LEventLog(bool threadSafetyIsGuaranteed) 
            : base(threadSafetyIsGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override void WriteSimple(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine)
        {
            _eventLog.WriteEntry(text);
        }

        protected override bool InitializeSimple()
        {
            if (!EventLog.Exists(Settings.LogName, Settings.MachineName))
            {
                EventLog.CreateEventSource(Settings.Source, Settings.LogName);
            }
            _eventLog = new EventLog(Settings.LogName, Settings.MachineName, Settings.Source);
            if (_eventLog.OverflowAction != Settings.OverflowAction ||
                _eventLog.MinimumRetentionDays != Settings.OverwriteOlderRetentionDays)
            {
                _eventLog.ModifyOverflowPolicy(Settings.OverflowAction, Settings.OverwriteOlderRetentionDays);
            }
            if (_eventLog.MaximumKilobytes != Settings.MaxLogSize)
            {
                _eventLog.MaximumKilobytes = Settings.MaxLogSize;
            }
            return true;
        }

        protected override bool DeinitializeSimple()
        {
            _eventLog.Dispose();
            return true;
        }

#endregion

    }
}

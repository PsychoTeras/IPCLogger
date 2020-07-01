using IPCLogger.Loggers.Base;
using IPCLogger.Snippets;
using System;
using System.Diagnostics;

namespace IPCLogger.Loggers.LEventLog
{
    public sealed class LEventLog : SimpleLogger<LEventLogSettings>
    {

#region Private fields

        private EventLog _eventLog;
        private int? _eventId;
        private short? _category;

#endregion

#region Ctor

        public LEventLog(bool threadSafetyGuaranteed) 
            : base(threadSafetyGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override void WriteSimple(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine)
        {
            int eventId;
            if (_eventId.HasValue)
            {
                eventId = _eventId.Value;
            }
            else
            {
                string sEventId = SFactory.Process(Settings.EventId, Patterns);
                if (!int.TryParse(sEventId, out eventId))
                {
                    _eventId = eventId = 0;
                }
            }

            short category;
            if (_category.HasValue)
            {
                category = _category.Value;
            }
            else
            {
                string sCategory = SFactory.Process(Settings.Category, Patterns);
                if (!short.TryParse(sCategory, out category))
                {
                    _category = category = 0;
                }
            }

            EventLogEntryType logType = Settings.GetLogEntryType(eventName);
            _eventLog.WriteEntry(text, logType, eventId, category, data);
        }

        protected override bool InitializeSimple()
        {
            string logName = SFactory.Process(Settings.LogName, Patterns);
            string source = SFactory.Process(Settings.Source, Patterns);

            if (!EventLog.Exists(logName, Settings.MachineName))
            {
                EventSourceCreationData data = new EventSourceCreationData(source, logName)
                {
                    MachineName = Settings.MachineName
                };
                EventLog.CreateEventSource(data);
            }

            _eventLog = new EventLog(logName, Settings.MachineName, source);
            if (_eventLog.OverflowAction != Settings.OverflowAction ||
                _eventLog.MinimumRetentionDays != Settings.OverwriteOlderRetentionDays)
            {
                _eventLog.ModifyOverflowPolicy(Settings.OverflowAction, Settings.OverwriteOlderRetentionDays);
            }
            if (_eventLog.MaximumKilobytes != Settings.MaxLogSize)
            {
                _eventLog.MaximumKilobytes = Settings.MaxLogSize;
            }

            int eventId;
            if (int.TryParse(Settings.EventId, out eventId))
            {
                _eventId = eventId;
            }
            short category;
            if (short.TryParse(Settings.Category, out category))
            {
                _category = category;
            }

            return true;
        }

        protected override bool DeinitializeSimple()
        {
            _eventId = null;
            _category = null;
            _eventLog.Dispose();
            return true;
        }

#endregion

    }
}

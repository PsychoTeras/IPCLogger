using IPCLogger.Core.Attributes;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IPCLogger.Core.Loggers.LEventLog
{
    public sealed class LEventLogSettings : BaseSettings
    {

#region Constants

        private static readonly char _paramSplitter = ';';
        private static readonly char _paramValSplitter = '=';

        private const string MACHINE_NAME = ".";

        private static readonly string _source = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);

        private const OverflowAction OVERFLOW_ACTION = OverflowAction.OverwriteOlder;

        private const EventLogEntryType DEFAULT_LOG_ENTRY_TYPE = EventLogEntryType.Information;

        private const int OVERFLOW_ACTION_RETENTION_DAYS = 30;

        private const int MAX_LOG_SIZE = 512*1024;

        private const string EVENT_ID = "0";

        private const string CATEGORY = "0";

#endregion

#region Private fields

        private Dictionary<string, EventLogEntryType> _logEntryTypeMatches;

#endregion

#region Properties

        public string MachineName { get; set; }

        public string LogName { get; set; }

        public string Source { get; set; }

        public string EventId { get; set; }

        public string Category { get; set; }

        public EventLogEntryType DefaultLogEntryType { get; set; }

        public string LogEntryTypeMatches { get; set; }

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
            Source = LogName = _source;
            EventId = EVENT_ID;
            Category = CATEGORY;
            DefaultLogEntryType = DEFAULT_LOG_ENTRY_TYPE;
            MaxLogSize = MAX_LOG_SIZE;
            OverflowAction = OVERFLOW_ACTION;
            OverwriteOlderRetentionDays = OVERFLOW_ACTION_RETENTION_DAYS;
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
            EventId = EventId;
            Category = Category;
            SetLogEntryTypeMatches();
        }

        private void SetLogEntryTypeMatches()
        {
            _logEntryTypeMatches = new Dictionary<string, EventLogEntryType>
            {
                {LogEvent.Info.ToString(), EventLogEntryType.Information},
                {LogEvent.Warn.ToString(), EventLogEntryType.Warning},
                {LogEvent.Error.ToString(), EventLogEntryType.Error}
            };

            if (!string.IsNullOrEmpty(LogEntryTypeMatches))
            {

                string[] pairs = LogEntryTypeMatches.Split(new[] { _paramSplitter }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string paramVal in pairs)
                {
                    try
                    {
                        string[] kv = paramVal.Split(new[] { _paramValSplitter }, StringSplitOptions.RemoveEmptyEntries);
                        if (kv.Length != 2)
                        {
                            string msg = "invalid pair";
                            throw new Exception(msg);
                        }

                        string eventName = kv[0].Trim();
                        if (_logEntryTypeMatches.ContainsKey(eventName))
                        {
                            string msg = $"duplicated event name '{eventName}'";
                            throw new Exception(msg);
                        }

                        string sEntryType = kv[1].Trim();
                        EventLogEntryType entryType;
                        if (!Enum.TryParse(sEntryType, out entryType))
                        {
                            string msg = $"improper entry type '{sEntryType}'. Possible values: {string.Join(", ", Enum.GetNames(typeof(EventLogEntryType)))}";
                            throw new Exception(msg);
                        }

                        _logEntryTypeMatches.Add(eventName, entryType);
                    }
                    catch (Exception ex)
                    {
                        string msg = $"Failed to parse LogEntryTypeMatches '{paramVal}'";
                        if (!string.IsNullOrEmpty(ex.Message))
                        {
                            msg += $": {ex.Message}";
                        }
                        throw new Exception(msg);
                    }
                }
            }
        }

        internal EventLogEntryType GetLogEntryType(string eventName)
        {
            EventLogEntryType entryType;
            if (eventName == null || !_logEntryTypeMatches.TryGetValue(eventName, out entryType))
            {
                entryType = DefaultLogEntryType;
            }
            return entryType;
        }

#endregion

    }
}

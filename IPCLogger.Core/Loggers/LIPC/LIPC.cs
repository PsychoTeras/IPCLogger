using System;
using System.Diagnostics;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Loggers.LIPC.FileMap;
using IPCLogger.Core.Proto;

namespace IPCLogger.Core.Loggers.LIPC
{
    public sealed class LIPC : ConcurrentLogger<LIPCSettings>
    {

#region Private fields

        private LogRecord _eventItem;
        private MapRingBuffer<LogRecord> _ipcEventRecords;

#endregion

#region Ctor

        public LIPC(bool threadSafetyIsGuaranteed) 
            : base(threadSafetyIsGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override void WriteConcurrent(Type callerType, Enum eventType, string eventName,
            string text, bool writeLine)
        {
            if (writeLine) text += Constants.NewLine;
            _eventItem.Setup(eventType != null ? (int) (object) eventType : 0, text);
            _ipcEventRecords.Add(_eventItem);
        }

        protected override bool InitializeConcurrent()
        {
            _eventItem = new LogRecord();

            string mmfName;
            if (!string.IsNullOrEmpty(Settings.CustomName))
            {
                mmfName = Settings.CustomName;
            }
            else
            {
                Process process = Process.GetCurrentProcess();
                mmfName = string.Format(@"{0}_{1}", process.ProcessName, process.Id);
            }
            string name = string.Format(@"Global\LIPC~{0}", mmfName);
            _ipcEventRecords = MapRingBuffer<LogRecord>.Host(name, Settings.CachedRecordsNum);

            return true;
        }

        protected override bool DeinitializeConcurrent()
        {
            _ipcEventRecords.Dispose();
            _ipcEventRecords = null;
            return true;
        }

#endregion

    }
}

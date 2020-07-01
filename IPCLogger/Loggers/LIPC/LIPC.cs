using IPCLogger.Common;
using IPCLogger.Loggers.Base;
using IPCLogger.Loggers.LIPC.FileMap;
using IPCLogger.Proto;
using System;
using System.Diagnostics;

namespace IPCLogger.Loggers.LIPC
{
    public sealed class LIPC : ConcurrentLogger<LIPCSettings>
    {

#region Private fields

        private LogItem _eventItem;
        private MapRingBuffer<LogItem> _ipcEventRecords;

#endregion

#region Ctor

        public LIPC(bool threadSafetyGuaranteed) 
            : base(threadSafetyGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override void WriteConcurrent(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine)
        {
            if (writeLine) text += Constants.NewLine;
            _eventItem.Setup(eventType != null ? (int)(object)eventType : 0, text);
            _ipcEventRecords.Write(ref _eventItem);
        }

        protected override void FlushConcurrent()
        {
            _ipcEventRecords.Flush();
        }

        protected override bool InitializeConcurrent()
        {
            _eventItem = new LogItem();

            string mmfName;
            if (!string.IsNullOrEmpty(Settings.CustomName))
            {
                mmfName = Settings.CustomName;
            }
            else
            {
                Process process = Process.GetCurrentProcess();
                mmfName = $"{process.ProcessName}_{process.Id}";
            }
            string name = $"Global\\LIPC~{mmfName}";
            _ipcEventRecords = MapRingBuffer<LogItem>.Host(name, Settings.CachedRecordsNum);

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

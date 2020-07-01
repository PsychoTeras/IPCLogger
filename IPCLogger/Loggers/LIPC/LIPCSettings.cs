using IPCLogger.Loggers.Base;
using System;

namespace IPCLogger.Loggers.LIPC
{
    public sealed class LIPCSettings : BaseSettings
    {

#region Constants

        private const ushort CACHED_RECORDS_NUM = 100;

#endregion

#region Private fields

        private ushort _cachedRecordsNum;

#endregion

#region Properties

        public ushort CachedRecordsNum
        {
            get { return _cachedRecordsNum; }
            set { _cachedRecordsNum = Math.Max(value, (ushort) 1); }
        }

        public string CustomName { get; set; }

#endregion

#region Ctor

        public LIPCSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges)
        {
            _cachedRecordsNum = CACHED_RECORDS_NUM;
        }

#endregion

    }
}

using IPCLogger.Common;
using System.Collections.Generic;
using System.Threading;

namespace IPCLogger.Storages
{
    internal static class LS 
    {

#region Private fields

        private static readonly Dictionary<int, LSObject> _threadStorage = new Dictionary<int, LSObject>();
        private static readonly LightLock _lockObj = new LightLock();

#endregion

#region Static methods

        public static LSObject Push()
        {
            _lockObj.WaitOne();
            LSObject lsObj;
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_threadStorage.TryGetValue(threadId, out lsObj))
            {
                lsObj = new LSObject();
                _threadStorage.Add(threadId, lsObj);
            }
            _lockObj.Set();
            return lsObj;
        }

        public static LSObject Peek()
        {
            _lockObj.WaitOne();
            LSObject lsObj;
            int threadId = Thread.CurrentThread.ManagedThreadId;
            LSObject val = _threadStorage.TryGetValue(threadId, out lsObj) ? lsObj : null;
            _lockObj.Set();
            return val;
        }

        public static void Pop()
        {
            _lockObj.WaitOne();

            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (_threadStorage.ContainsKey(threadId))
            {
                _threadStorage.Remove(threadId);
            }
            _lockObj.Set();
        }

#endregion

    }
}

using System.Collections.Generic;
using System.Threading;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Storages
{
    internal static class Ls 
    {

#region Private fields

        private static readonly Dictionary<int, LsObject> _threadStorage = new Dictionary<int, LsObject>();
        private static readonly LightLock _lockObj = new LightLock();

#endregion

#region Static methods

        public static LsObject Push()
        {
            _lockObj.WaitOne();
            LsObject lsObj;
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_threadStorage.TryGetValue(threadId, out lsObj))
            {
                lsObj = new LsObject();
                _threadStorage.Add(threadId, lsObj);
            }
            _lockObj.Set();
            return lsObj;
        }

        public static LsObject Peek()
        {
            _lockObj.WaitOne();
            LsObject lsObj;
            int threadId = Thread.CurrentThread.ManagedThreadId;
            LsObject val = _threadStorage.TryGetValue(threadId, out lsObj) ? lsObj : null;
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

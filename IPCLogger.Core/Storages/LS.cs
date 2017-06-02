using System.Collections.Generic;
using System.Threading;

namespace IPCLogger.Core.Storages
{
    internal static class LS 
    {

#region Private fields

        private static readonly object LockObj = new object();
        private static readonly Dictionary<int, LSObject> ThreadStorage = 
            new Dictionary<int, LSObject>();

#endregion

#region Static methods

        public static LSObject Push()
        {
            lock (LockObj)
            {
                LSObject lsObj;
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (!ThreadStorage.TryGetValue(threadId, out lsObj))
                {
                    lsObj = new LSObject();
                    ThreadStorage.Add(threadId, lsObj);
                }
                return lsObj;
            }
        }

        public static LSObject Peek()
        {
            lock (LockObj)
            {
                LSObject lsObj;
                int threadId = Thread.CurrentThread.ManagedThreadId;
                return ThreadStorage.TryGetValue(threadId, out lsObj) ? lsObj : null;
            }
        }

        public static void Pop()
        {
            lock (LockObj)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (ThreadStorage.ContainsKey(threadId))
                {
                    ThreadStorage.Remove(threadId);
                }
            }
        }

#endregion

    }
}

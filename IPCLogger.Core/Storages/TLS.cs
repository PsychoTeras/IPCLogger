using System.Collections.Generic;
using System.Threading;

namespace IPCLogger.Core.Storages
{
    public static class TLS
    {

#region Private fields

        private static readonly object LockObj = new object();
        private static readonly Dictionary<int, TLSObject> ThreadStorage =
            new Dictionary<int, TLSObject>();

#endregion

#region Static methods

        public static TLSObject Create()
        {
            return new TLSObject();
        }

        public static void Push(TLSObject tlsObj)
        {
            lock (LockObj)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (ThreadStorage[threadId] != null)
                {
                    ThreadStorage[threadId] = tlsObj;
                }
                else
                {
                    ThreadStorage.Add(threadId, tlsObj);
                }
            }
        }

        public static TLSObject Push()
        {
            lock (LockObj)
            {
                TLSObject tlsObj;
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (!ThreadStorage.TryGetValue(threadId, out tlsObj))
                {
                    tlsObj = new TLSObject();
                    ThreadStorage.Add(threadId, tlsObj);
                }
                return tlsObj;
            }
        }

        public static TLSObject Peek()
        {
            lock (LockObj)
            {
                TLSObject tlsObj;
                int threadId = Thread.CurrentThread.ManagedThreadId;
                return ThreadStorage.TryGetValue(threadId, out tlsObj) ? tlsObj : null;
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

        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            TLSObject tlsObj = Peek();
            return tlsObj != null ? tlsObj[key] : null;
        }

#endregion

    }
}

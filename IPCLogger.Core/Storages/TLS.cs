using System.Collections.Generic;
using System.Threading;

namespace IPCLogger.Core.Storages
{
    public static class TLS
    {

#region Constants

        private const int USE_DYNAMIC_THREAD_ID = 0;

#endregion

#region Private fields

        private static readonly object LockObj = new object();
        private static readonly Dictionary<int, TLSObject> ThreadStorage = new Dictionary<int, TLSObject>();

        private static int _currentThreadId;

#endregion

#region Properties

        private static int CurrentThreadId
        {
            get
            {
                return _currentThreadId == USE_DYNAMIC_THREAD_ID
                    ? Thread.CurrentThread.ManagedThreadId
                    : _currentThreadId;
            }
        }

#endregion

#region Static methods

        public static void PinCurrentThreadId()
        {
            _currentThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static void UnpinCurrentThreadId()
        {
            _currentThreadId = _currentThreadId = USE_DYNAMIC_THREAD_ID;
        }

        public static TLSObject Create()
        {
            return new TLSObject();
        }

        public static void Push(TLSObject tlsObj)
        {
            lock (LockObj)
            {
                int threadId = CurrentThreadId;
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
                int threadId = CurrentThreadId;
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
                int threadId = CurrentThreadId;
                return ThreadStorage.TryGetValue(threadId, out tlsObj) ? tlsObj : null;
            }
        }

        public static void Pop()
        {
            lock (LockObj)
            {
                int threadId = CurrentThreadId;
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

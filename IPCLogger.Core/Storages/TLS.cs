using System.Collections;
using System.Collections.Generic;
using System.Threading;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Storages
{
    public static class TLS
    {

#region Constants

        private const int USE_DYNAMIC_THREAD_ID = 0;

#endregion

#region Private fields

        private static readonly Dictionary<int, TLSObject> _threadStorage = new Dictionary<int, TLSObject>();
        private static readonly LightLock _lockObj = new LightLock();

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
            _lockObj.WaitOne();
            int threadId = CurrentThreadId;
            if (_threadStorage[threadId] != null)
            {
                _threadStorage[threadId] = tlsObj;
            }
            else
            {
                _threadStorage.Add(threadId, tlsObj);
            }
            _lockObj.Set();
        }

        public static TLSObject Push()
        {
            _lockObj.WaitOne();
            TLSObject tlsObj;
            int threadId = CurrentThreadId;
            if (!_threadStorage.TryGetValue(threadId, out tlsObj))
            {
                tlsObj = new TLSObject();
                _threadStorage.Add(threadId, tlsObj);
            }
            _lockObj.Set();
            return tlsObj;
        }

        public static TLSObject Peek()
        {
            _lockObj.WaitOne();
            TLSObject tlsObj;
            int threadId = CurrentThreadId;
            TLSObject val = _threadStorage.TryGetValue(threadId, out tlsObj) ? tlsObj : null;
            _lockObj.Set();
            return val;
        }

        public static void Pop()
        {
            _lockObj.WaitOne();
            int threadId = CurrentThreadId;
            if (_threadStorage.ContainsKey(threadId))
            {
                _threadStorage.Remove(threadId);
            }
            _lockObj.Set();
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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Storages
{
    public static class TLS
    {

#region Constants

        private const int USE_DYNAMIC_THREAD_ID = 0;
        private static readonly char GLOBALS_MEMBER_ACCESS_SEPARATOR = '.';

#endregion

#region Private fields

        private static readonly Dictionary<string, TLSObject> _threadGlobalStorage = new Dictionary<string, TLSObject>();
        private static readonly LightLock _lockObjGlobal = new LightLock();

        private static readonly Dictionary<int, TLSObject> _threadStorage = new Dictionary<int, TLSObject>();
        private static readonly LightLock _lockObjThread = new LightLock();

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
            _lockObjThread.WaitOne();
            int threadId = CurrentThreadId;
            if (_threadStorage.ContainsKey(threadId))
            {
                _threadStorage[threadId] = tlsObj;
            }
            else
            {
                _threadStorage.Add(threadId, tlsObj);
            }
            _lockObjThread.Set();
        }

        public static TLSObject Push()
        {
            _lockObjThread.WaitOne();
            TLSObject tlsObj;
            int threadId = CurrentThreadId;
            if (!_threadStorage.TryGetValue(threadId, out tlsObj))
            {
                tlsObj = new TLSObject();
                _threadStorage.Add(threadId, tlsObj);
            }
            _lockObjThread.Set();
            return tlsObj;
        }

        public static TLSObject Peek()
        {
            _lockObjThread.WaitOne();
            TLSObject tlsObj;
            int threadId = CurrentThreadId;
            TLSObject val = _threadStorage.TryGetValue(threadId, out tlsObj) ? tlsObj : null;
            _lockObjThread.Set();
            return val;
        }

        public static void Pop()
        {
            _lockObjThread.WaitOne();
            int threadId = CurrentThreadId;
            if (_threadStorage.ContainsKey(threadId))
            {
                _threadStorage.Remove(threadId);
            }
            _lockObjThread.Set();
        }

        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            TLSObject tlsObj;

            int memAccSepIdx = key.IndexOf(GLOBALS_MEMBER_ACCESS_SEPARATOR);
            if (memAccSepIdx != -1)
            {
                string globalObjectName = key.Substring(0, memAccSepIdx);
                if (_threadGlobalStorage.TryGetValue(globalObjectName, out tlsObj))
                {
                    string globalMemberName = key.Substring(memAccSepIdx + 1);
                    return globalMemberName != string.Empty ? tlsObj[globalMemberName] : tlsObj;
                }
            }

            if (!_threadGlobalStorage.TryGetValue(key, out tlsObj))
            {
                tlsObj = Peek();
            }
            return tlsObj != null ? tlsObj[key] : null;
        }

        private static void AsssertGlobalName(string globalName)
        {
            if (string.IsNullOrEmpty(globalName))
            {
                throw new Exception("Global name should'n be empty");
            }
        }

        public static void SetClosureGlobal<T>(string globalName, Expression<Func<T>> memberExpression)
        {
            AsssertGlobalName(globalName);

            _lockObjGlobal.WaitOne();
            TLSObject tlsObj = new TLSObject();
            tlsObj.SetClosure(memberExpression);
            _threadGlobalStorage[globalName] = tlsObj;
            _lockObjGlobal.Set();
        }

        public static void SetClosureGlobal(string globalName, params Expression<Func<object>>[] membersExpressions)
        {
            AsssertGlobalName(globalName);

            _lockObjGlobal.WaitOne();
            TLSObject tlsObj = new TLSObject();
            tlsObj.SetClosure(membersExpressions);
            _threadGlobalStorage[globalName] = tlsObj;
            _lockObjGlobal.Set();
        }

        public static void SetClosureGlobal<T>(string globalName, string key, Expression<Func<T>> memberExpression)
        {
            AsssertGlobalName(globalName);

            _lockObjGlobal.WaitOne();
            TLSObject tlsObj = new TLSObject();
            tlsObj.SetClosure(key, memberExpression);
            _threadGlobalStorage[globalName] = tlsObj;
            _lockObjGlobal.Set();
        }

        public static void CaptureObjectGlobal<T>(string globalName, string key, T obj, bool useFullClassName = true,
            BindingFlags? bfFields = BindingFlags.GetField | TLSObject.BF_DEFAULT,
            BindingFlags? bfProperties = BindingFlags.GetProperty | TLSObject.BF_DEFAULT,
            HashSet<string> excludeNames = null)
        {
            AsssertGlobalName(globalName);

            _lockObjGlobal.WaitOne();
            TLSObject tlsObj = new TLSObject();
            tlsObj.CaptureObject(key, obj, useFullClassName, bfFields, bfProperties, excludeNames);
            _threadGlobalStorage[globalName] = tlsObj;
            _lockObjGlobal.Set();
        }

        public static void RemoveGlobal(string globalName)
        {
            AsssertGlobalName(globalName);

            _lockObjGlobal.WaitOne();
            if (_threadGlobalStorage.ContainsKey(globalName))
            {
                _threadGlobalStorage.Remove(globalName);
            }
            _lockObjGlobal.Set();
        }

#endregion

    }
}

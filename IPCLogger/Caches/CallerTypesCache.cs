using IPCLogger.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace IPCLogger.Caches
{
    internal static unsafe class CallerTypesCache
    {
        private static int _callerStackLevel = -1;

        private static readonly Dictionary<int, Dictionary<long, Type>> _cachedTypes =
            new Dictionary<int, Dictionary<long, Type>>();

        public static Type GetCallerType()
        {
            int stackMark;
            long stackAddr = Constants.Is64Bit ? (long)&stackMark : (int)&stackMark;

            Type type = null;
            Dictionary<long, Type> typeDict;
            int currentThread = Thread.CurrentThread.GetHashCode();
            if (!_cachedTypes.TryGetValue(currentThread, out typeDict))
            {
                lock (_cachedTypes)
                {
                    if (!_cachedTypes.TryGetValue(currentThread, out typeDict))
                    {
                        StackTrace stackTrace = new StackTrace();
                        if (_callerStackLevel == -1)
                        {
                            _callerStackLevel = Helpers.FindCallerStackLevel(stackTrace);
                        }
                        MethodBase method = stackTrace.GetFrame(_callerStackLevel).GetMethod();
                        typeDict = new Dictionary<long, Type> {{stackAddr, type = method.DeclaringType}};
                        _cachedTypes.Add(currentThread, typeDict);
                    }
                }
            }
            else
            {
                if (!typeDict.TryGetValue(stackAddr, out type))
                {
                    lock (typeDict)
                    {
                        if (!typeDict.TryGetValue(stackAddr, out type))
                        {
                            StackTrace stackTrace = new StackTrace();
                            if (_callerStackLevel == -1)
                            {
                                _callerStackLevel = Helpers.FindCallerStackLevel(stackTrace);
                            }
                            MethodBase method = stackTrace.GetFrame(_callerStackLevel).GetMethod();
                            typeDict.Add(stackAddr, type = method.DeclaringType);
                        }
                    }
                }
            }
            return type;
        }
    }
}

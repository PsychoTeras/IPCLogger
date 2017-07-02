using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace IPCLogger.Core.Caches
{
    internal static unsafe class CallerTypesCache
    {
        private static readonly bool Is64BitPtr = IntPtr.Size == 8;

        private static readonly Dictionary<int, Dictionary<long, Type>> CachedTypes =
            new Dictionary<int, Dictionary<long, Type>>();

        public static Type GetCallerType()
        {
            int stackMark;
            long stackAddr = Is64BitPtr ? (long)&stackMark : (int)&stackMark;

            Type type = null;
            Dictionary<long, Type> typeDict;
            int currentThread = Thread.CurrentThread.GetHashCode();
            if (!CachedTypes.TryGetValue(currentThread, out typeDict))
            {
                lock (CachedTypes)
                {
                    if (!CachedTypes.TryGetValue(currentThread, out typeDict))
                    {
                        MethodBase method = new StackTrace(3).GetFrame(0).GetMethod();
                        typeDict = new Dictionary<long, Type> {{stackAddr, type = method.DeclaringType}};
                        CachedTypes.Add(currentThread, typeDict);
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
                            MethodBase method = new StackTrace(3).GetFrame(0).GetMethod();
                            typeDict.Add(stackAddr, type = method.DeclaringType);
                        }
                    }
                }
            }
            return type;
        }

        public static void Setup() { }
    }
}

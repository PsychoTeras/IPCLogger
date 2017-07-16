using System;
using System.Collections.Generic;

namespace IPCLogger.Core.Caches
{
    internal static class TLSClosureMembers
    {
        private static readonly Dictionary<object, Delegate> EventNames = new Dictionary<object, Delegate>();

        public static Delegate GetTLSClosureMember(object key, Func<Delegate> onBuild)
        {
            Delegate func;
            if (!EventNames.TryGetValue(key, out func))
            {
                lock (EventNames)
                {
                    if (!EventNames.TryGetValue(key, out func))
                    {
                        func = onBuild();
                        EventNames.Add(key, func);
                    }
                }
            }
            return func;
        }
    }
}

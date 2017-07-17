using System;
using System.Collections.Generic;

namespace IPCLogger.Core.Caches
{
    public class DictionaryCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> Dict = new Dictionary<TKey, TValue>();

        public TValue Get(TKey key, Func<TValue> onBuild)
        {
            TValue func;
            if (!Dict.TryGetValue(key, out func))
            {
                lock (Dict)
                {
                    if (!Dict.TryGetValue(key, out func))
                    {
                        func = onBuild();
                        Dict.Add(key, func);
                    }
                }
            }
            return func;
        }
    }
}

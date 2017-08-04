using System;
using System.Collections.Generic;

namespace IPCLogger.Core.Caches
{
    public class DictionaryCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

        public TValue Get(TKey key, Func<TValue> onBuild)
        {
            TValue func;
            if (!_dict.TryGetValue(key, out func))
            {
                lock (_dict)
                {
                    if (!_dict.TryGetValue(key, out func))
                    {
                        func = onBuild();
                        _dict.Add(key, func);
                    }
                }
            }
            return func;
        }
    }
}

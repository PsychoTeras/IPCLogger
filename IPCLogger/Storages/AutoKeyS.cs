using IPCLogger.Common;
using System.Collections.Generic;

namespace IPCLogger.Storages
{
    internal static class AutoKeyS
    {

#region Private fields

        private static Dictionary<string, AutoKeyItem> _autoKeys = new Dictionary<string, AutoKeyItem>();
        private static readonly LightLock _lockObj = new LightLock();

#endregion

#region Class methods

        internal static void Add(string name, int initValue, int increment, string format)
        {
            _lockObj.WaitOne();
            if (!_autoKeys.ContainsKey(name))
            {
                _autoKeys.Add(name, new AutoKeyItem(initValue, increment, format));
            }
            _lockObj.Set();
        }

        internal static string Pop(string name)
        {
            AutoKeyItem item;
            return _autoKeys.TryGetValue(name, out item) ? item.GetAndIncrease() : null;
        }

        internal static void Remove(string name)
        {
            _lockObj.WaitOne();
            _autoKeys.Remove(name);
            _lockObj.Set();
        }

        public static void Set(string name, int value)
        {
            AutoKeyItem item;
            if (_autoKeys.TryGetValue(name, out item))
            {
                item.Value = value;
            }
        }

        public static void Reset(string name)
        {
            AutoKeyItem item;
            if (_autoKeys.TryGetValue(name, out item))
            {
                item.Reset();
            }
        }

#endregion

    }
}

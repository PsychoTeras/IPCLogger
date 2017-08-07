using System;
using System.Collections.Generic;

namespace IPCLogger.Core.Caches
{
    internal static class EventNamesCache
    {
        private static readonly Dictionary<int, string> _eventNames = new Dictionary<int, string>();

        public static string GetEventName(Enum eventType)
        {
            if (eventType == null) return string.Empty;

            int key = (int)(object)eventType;

            string eventName;
            if (!_eventNames.TryGetValue(key, out eventName))
            {
                lock (_eventNames)
                {
                    if (!_eventNames.TryGetValue(key, out eventName))
                    {
                        eventName = eventType.ToString();
                        _eventNames.Add(key, eventName);
                    }
                }
            }
            return eventName;
        }

        public static string GetEventName(Enum eventType, int key)
        {
            string eventName;
            if (!_eventNames.TryGetValue(key, out eventName))
            {
                if (eventType == null) return string.Empty;
                lock (_eventNames)
                {
                    if (!_eventNames.TryGetValue(key, out eventName))
                    {
                        eventName = eventType.ToString();
                        _eventNames.Add(key, eventName);
                    }
                }
            }
            return eventName;
        }
    }
}
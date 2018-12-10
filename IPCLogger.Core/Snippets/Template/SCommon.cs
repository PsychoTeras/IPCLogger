using IPCLogger.Core.Caches;
using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading;

namespace IPCLogger.Core.Snippets.Template
{
    internal class SCommon : BaseSnippet
    {

#region Declarations

        class DateTimeRecord
        {
            public int LastDateMark;
            public string Value;

            public DateTimeRecord(int lastDateMark, string value)
            {
                LastDateMark = lastDateMark;
                Value = value;
            }
        }

#endregion

#region Constants

        private static readonly int _timeMarkMod = 0;

#endregion

#region Static fields

        private static readonly Dictionary<string, DateTimeRecord> _dateStrings = new Dictionary<string, DateTimeRecord>();
        private static readonly LightLock _lockDateStrings = new LightLock();

        private static volatile int _lastUtcMark;
        private static readonly Dictionary<string, string> _dateUtcStrings = new Dictionary<string, string>();
        private static readonly LightLock _lockDateUtcStrings = new LightLock();

        private static readonly string _machineName = Environment.MachineName;
        private static readonly string _userName = WindowsIdentity.GetCurrent().Name;

        private static readonly Process _process = System.Diagnostics.Process.GetCurrentProcess();
        private static readonly string _processName = _process.ProcessName;
        private static readonly string _processId = _process.Id.ToString();
        private static readonly string _processInfo = $"{_processName} [{_processId}]";

        private static string _appName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        private static string _appDomain = AppDomain.CurrentDomain.FriendlyName;

#endregion

#region Properties

        public override string[] Names
        {
            get
            {
                return new[]
                {
                     "text"
                    ,"data"
                    ,"newline"
                    ,"date"
                    ,"utcdate"
                    ,"ticks"
                    ,"uptime"
                    ,"username"
                    ,"appname"
                    ,"appdomain"
                    ,"thread"
                    ,"event"
                    ,"guid"
                    ,"process"
                    ,"machine"
                };
            }
        }

#endregion

#region Ctor

        public SCommon() : base(SnippetType.Template) {}

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName, byte[] data,
            string text, string @params, PFactory pFactory)
        {
            switch (snippetName)
            {
                case "text":
                    return text;
                case "data":
                    SnippetParams sParams = ParseSnippetParams(@params);
                    int lineLength = sParams.GetValue("lineLength", int.MaxValue);
                    return Helpers.ByteArrayToString(data, lineLength);
                case "newline":
                    return Constants.NewLine;
                case "date":
                {
                    int ticks = Environment.TickCount;
                    int currentTimeMark = _timeMarkMod == 0 ? ticks : ticks - ticks % _timeMarkMod;
                    bool newCacheItem = !_dateStrings.TryGetValue(@params, out DateTimeRecord cachedDate);

                    if (newCacheItem || currentTimeMark != cachedDate.LastDateMark)
                    {
                        _lockDateStrings.WaitOne();

                        if (newCacheItem && _dateStrings.TryGetValue(@params, out cachedDate))
                        {
                            _lockDateStrings.Set();
                            return cachedDate.Value;
                        }

                        string value = DateTime.Now.ToString(@params);
                        if (newCacheItem)
                        {
                            cachedDate = new DateTimeRecord(currentTimeMark, value);
                            _dateStrings.Add(@params, cachedDate);
                        }
                        else
                        {
                            cachedDate.LastDateMark = currentTimeMark;
                            cachedDate.Value = value;
                        }

                        _lockDateStrings.Set();
                    }

                    return cachedDate.Value;
                }
                case "utcdate":
                {
                    int ticks = Environment.TickCount;
                    int currentTimeUtcMark = _timeMarkMod == 0 ? ticks : ticks - ticks % _timeMarkMod;
                    if (currentTimeUtcMark != _lastUtcMark || !_dateUtcStrings.TryGetValue(@params, out var cachedDate))
                    {
                        _lockDateUtcStrings.WaitOne();
                        cachedDate = DateTime.UtcNow.ToString(@params);
                        if (currentTimeUtcMark == _lastUtcMark)
                        {
                            _dateUtcStrings.Add(@params, cachedDate);
                        }
                        else
                        {
                            _dateUtcStrings[@params] = cachedDate;
                        }

                        _lastUtcMark = currentTimeUtcMark;
                        _lockDateUtcStrings.Set();
                    }

                    return cachedDate;
                }
                case "ticks":
                    return Environment.TickCount.ToString(@params);
                case "uptime":
                    return (DateTime.Now - _process.StartTime).Milliseconds.ToString(@params);
                case "username":
                    return _userName;
                case "appname":
                    return _appName;
                case "appdomain":
                    return _appDomain;
                case "thread":
                    Thread thread = Thread.CurrentThread;
                    switch (@params)
                    {
                        case "name":
                            return thread.Name;
                        case "id":
                            return thread.ManagedThreadId.ToString();
                        default:
                            return $"{thread.Name} [{thread.ManagedThreadId}]";
                    }
                case "process":
                    switch (@params)
                    {
                        case "name":
                            return _processName;
                        case "id":
                            return _processId;
                        default:
                            return _processInfo;
                    }
                case "machine":
                    switch (@params)
                    {
                        case "name":
                            return _machineName;
                        default:
                            return _machineName;
                    }
                case "event":
                    if (eventType == null) return null;
                    switch (@params)
                    {
                        case "int":
                            return ((int) (object) eventType).ToString();
                        default:
                            return EventNamesCache.GetEventName(eventType);
                    }
                case "guid":
                    return Guid.NewGuid().ToString(@params);
            }

            return null;
        }

#endregion

    }
}
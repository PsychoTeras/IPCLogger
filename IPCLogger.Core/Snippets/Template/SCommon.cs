using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading;
using IPCLogger.Core.Caches;
using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;

namespace IPCLogger.Core.Snippets.Template
{
    internal sealed class SCommon : BaseSnippet
    {

#region Constants

        private static readonly int TimeMarkMod = 0;

#endregion

#region Static fields

        private static volatile int _lastDateMark;
        private static readonly Dictionary<string, string> _dateStrings = new Dictionary<string, string>();
        private static readonly LightLock _lockDateStrings = new LightLock();


        private static volatile int _lastUTCMark;
        private static readonly Dictionary<string, string> _dateUTCStrings = new Dictionary<string, string>();
        private static readonly LightLock _lockDateUTCStrings = new LightLock();

        private static readonly string _userName = WindowsIdentity.GetCurrent().Name;
        private static readonly Process _process = System.Diagnostics.Process.GetCurrentProcess();
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
                };
            }
        }

#endregion

#region Ctor

        public SCommon() : base(SnippetType.Template) {}

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            switch (snippetName)
            {
                case "text":
                    return text;
                case "newline":
                    return Constants.NewLine;
                case "date":
                {
                    string cachedDate;
                    int ticks = Environment.TickCount;
                    int currentTimeMark = TimeMarkMod == 0 ? ticks : ticks - ticks%TimeMarkMod;
                    if (currentTimeMark != _lastDateMark || !_dateStrings.TryGetValue(@params, out cachedDate))
                    {
                        _lockDateStrings.WaitOne();
                        cachedDate = DateTime.Now.ToString(@params);
                        if (!_dateStrings.ContainsKey(@params))
                        {
                            _dateStrings.Add(@params, cachedDate);
                        }
                        else
                        {
                            _dateStrings[@params] = cachedDate;
                        }
                        _lastDateMark = currentTimeMark;
                        _lockDateStrings.Set();
                    }
                    return cachedDate;
                }
                case "utcdate":
                {
                    string cachedDate;
                    int ticks = Environment.TickCount;
                    int currentTimeUTCMark = TimeMarkMod == 0 ? ticks : ticks - ticks%TimeMarkMod;
                    if (currentTimeUTCMark != _lastUTCMark || !_dateUTCStrings.TryGetValue(@params, out cachedDate))
                    {
                        _lockDateUTCStrings.WaitOne();
                        cachedDate = DateTime.UtcNow.ToString(@params);
                        if (currentTimeUTCMark == _lastUTCMark)
                        {
                            _dateUTCStrings.Add(@params, cachedDate);
                        }
                        else
                        {
                            _dateUTCStrings[@params] = cachedDate;
                        }
                        _lastUTCMark = currentTimeUTCMark;
                        _lockDateUTCStrings.Set();
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
                            return string.Format("{0} [{1}]", thread.Name, thread.ManagedThreadId);
                    }
                case "process":
                    switch (@params)
                    {
                        case "name":
                            return _process.ProcessName;
                        case "id":
                            return _process.Id.ToString();
                        default:
                            return string.Format("{0} [{1}]", _process.ProcessName, _process.Id);
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

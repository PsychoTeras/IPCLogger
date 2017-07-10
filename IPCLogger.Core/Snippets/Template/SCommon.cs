using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static readonly Dictionary<string, string> DateStrings = new Dictionary<string, string>();
        private static readonly LightLock _lockDateStrings = new LightLock();


        private static volatile int _lastUTCMark;
        private static readonly Dictionary<string, string> DateUTCStrings = new Dictionary<string, string>();
        private static readonly LightLock _lockDateUTCStrings = new LightLock();

        private static readonly string UserName = WindowsIdentity.GetCurrent().Name;

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
                    ,"appdomain"
                    ,"thread"
                    ,"event"
                    ,"guid"
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
                    if (currentTimeMark != _lastDateMark || !DateStrings.TryGetValue(@params, out cachedDate))
                    {
                        _lockDateStrings.WaitOne();

                        cachedDate = DateTime.Now.ToString(@params);
                        if (!DateStrings.ContainsKey(@params))
                        {
                            DateStrings.Add(@params, cachedDate);
                        }
                        else
                        {
                            DateStrings[@params] = cachedDate;
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
                    if (currentTimeUTCMark != _lastUTCMark || !DateUTCStrings.TryGetValue(@params, out cachedDate))
                    {
                            _lockDateUTCStrings.WaitOne();

                        cachedDate = DateTime.UtcNow.ToString(@params);
                        if (currentTimeUTCMark == _lastUTCMark)
                        {
                            DateUTCStrings.Add(@params, cachedDate);
                        }
                        else
                        {
                            DateUTCStrings[@params] = cachedDate;
                        }
                        _lastUTCMark = currentTimeUTCMark;

                        _lockDateUTCStrings.Set();
                    }
                    return cachedDate;
                }
                case "ticks":
                    return Environment.TickCount.ToString(@params);
                case "uptime":
                    Process process = System.Diagnostics.Process.GetCurrentProcess();
                    return (DateTime.Now - process.StartTime).Milliseconds.ToString(@params);
                case "username":
                    return UserName;
                case "appdomain":
                    return AppDomain.CurrentDomain.FriendlyName;
                case "thread":
                    Thread thread = Thread.CurrentThread;
                    return string.Format("{0} [{1}]", thread.Name, thread.ManagedThreadId);
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

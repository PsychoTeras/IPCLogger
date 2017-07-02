using System;
using System.Diagnostics;
using IPCLogger.Core.Caches;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LDebug
{
    public sealed class LDebug : BaseLogger<LDebugSettings>
    {

#region ILogger

        protected internal override void Write(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine, bool immediateFlush)
        {
            if (Settings.Trace)
            {
                if (writeLine)
                {
                    Trace.WriteLine(text, EventNamesCache.GetEventName(eventType));
                }
                else
                {
                    Trace.Write(text, EventNamesCache.GetEventName(eventType));
                }
                if (immediateFlush || Settings.ImmediateFlush) Trace.Flush();
            }
            else
            {
                if (writeLine)
                {
                    Debug.WriteLine(text, EventNamesCache.GetEventName(eventType));
                }
                else
                {
                    Debug.Write(text, EventNamesCache.GetEventName(eventType));
                }
                if (immediateFlush || Settings.ImmediateFlush) Debug.Flush();
            }
        }

        public override void Initialize() { }

        public override void Deinitialize() { }

#endregion

    }
}

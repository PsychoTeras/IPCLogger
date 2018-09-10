using System;
using System.Xml;
using IPCLogger.Core.Caches;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Patterns.Base;
using IPCLogger.Core.Snippets;
using IPCLogger.Core.Storages;

namespace IPCLogger.Core.Loggers.Base
{
    public delegate void OnLoggerException(IBaseLogger sender, Exception ex);

    public interface IBaseLogger : IDisposable
    {
        void Initialize();
        void Deinitialize();

        void Write(string text);
        void WriteLine();
        void WriteLine(string text);

        void Write(Enum eventType, string text);
        void WriteLine(Enum eventType);
        void WriteLine(Enum eventType, string text);

        void Write(Enum eventType, Exception ex);
        void Write(Enum eventType, Exception ex, string text);
        void WriteLine(Enum eventType, Exception ex);
        void WriteLine(Enum eventType, Exception ex, string text);

        void WriteData(byte[] data);
        void WriteData(byte[] data, string text);
        void WriteData(Enum eventType, byte[] data);
        void WriteData(Enum eventType, byte[] data, string text);
        void WriteData(Enum eventType, Exception ex, byte[] data);
        void WriteData(Enum eventType, Exception ex, byte[] data, string text);

        bool Suspend();
        bool Resume();
        void Flush();
    }

    internal interface ILoggerSettings<out TSettings>
        where TSettings : BaseSettings
    {
        TSettings Settings { get; }
    }

    public abstract class BaseLoggerInt : IBaseLogger
    {

#region Events

        protected static readonly object LoggerExceptionSyncObj = new object();
        protected static OnLoggerException OnLoggerException;
        internal Func<string, bool> CheckApplicableEvent;

#endregion
        
#region Properties

        internal string UniqueId { get; private set; }
        internal PFactory Patterns { get; private set; }

#endregion

#region Fields

        protected Action PreInitialize;

#endregion

#region Class methods

        protected internal abstract void Write(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine, bool immediateFlush);

        internal void SetPatternsFactory(PFactory pFactory)
        {
            Patterns = pFactory;
        }

        internal void SetUniqueId(string uniqueId)
        {
            UniqueId = uniqueId;
        }

        internal void Setup(XmlNode cfgNode)
        {
            BaseSettings settings = ((ILoggerSettings<BaseSettings>) this).Settings;
            settings.Setup(cfgNode);
            CheckApplicableEvent = settings.CheckApplicableEvent;
        }

#endregion

#region IBaseLogger

        public abstract void Dispose();
        public abstract void Initialize();
        public abstract void Deinitialize();
        public abstract void Write(string text);
        public abstract void WriteLine();
        public abstract void WriteLine(string text);
        public abstract void Write(Enum eventType, string text);
        public abstract void WriteLine(Enum eventType);
        public abstract void WriteLine(Enum eventType, string text);
        public abstract void Write(Enum eventType, Exception ex);
        public abstract void Write(Enum eventType, Exception ex, string text);
        public abstract void WriteLine(Enum eventType, Exception ex);
        public abstract void WriteLine(Enum eventType, Exception ex, string text);
        public abstract void WriteData(byte[] data);
        public abstract void WriteData(byte[] data, string text);
        public abstract void WriteData(Enum eventType, byte[] data);
        public abstract void WriteData(Enum eventType, byte[] data, string text);
        public abstract void WriteData(Enum eventType, Exception ex, byte[] data);
        public abstract void WriteData(Enum eventType, Exception ex, byte[] data, string text);
        public abstract bool Suspend();
        public abstract bool Resume();
        public abstract void Flush();

#endregion

    }

    public abstract class BaseLogger<TSettings> : BaseLoggerInt, ILoggerSettings<TSettings>
        where TSettings : BaseSettings
    {

#region Global events

        public static event OnLoggerException LoggerException
        {
            add
            {
                lock (LoggerExceptionSyncObj)
                {
                    OnLoggerException += value;
                }
            }
            remove
            {
                lock (LoggerExceptionSyncObj)
                {
                    OnLoggerException -= value;
                }
            }
        }

#endregion

#region Properties

        public TSettings Settings { get; private set; }
        public bool ThreadSafetyGuaranteed { get; }

#endregion

#region Ctor

        protected BaseLogger(bool threadSafetyGuaranteed)
        {
            InitSettings();
            SetPatternsFactory(PFactory.Instance);
            ThreadSafetyGuaranteed = threadSafetyGuaranteed;
        }

#endregion

#region Protected methods

        protected virtual void OnSetupSettings() { }

        protected void CatchLoggerException(string message, Exception ex)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ex = new Exception(message, ex);
            }
            OnLoggerException loggerException = OnLoggerException;
            if (loggerException != null)
            {
                loggerException(this, ex);
            }
            else
            {
                throw ex;
            }
        }

#endregion

#region Private methods

        private void InitSettings()
        {
            Settings = (TSettings) Activator.CreateInstance(typeof (TSettings), GetType(), new Action(OnSetupSettings));
        }

        private bool ProcessText(Type callerType, Enum eventType, int eventTypeId, byte[] data, ref string text,
            out string eventName, out bool immediateFlush)
        {
            immediateFlush = false;

            if (Patterns != null && eventType != null)
            {
                eventName = EventNamesCache.GetEventName(eventType, eventTypeId);
                if (!Settings.CheckApplicableEvent(eventName))
                {
                    return false;
                }

                Pattern pattern = Patterns.Get(callerType, eventName);
                if (pattern != null)
                {
                    immediateFlush = pattern.ImmediateFlush;
                    text = SFactory.Process(callerType, eventType, data, text, pattern, Patterns);
                }
            }
            else
            {
                eventName = null;
            }

            return true;
        }

        private void WritePlain(byte[] data, string text, bool writeLine)
        {
            try
            {
                PreInitialize?.Invoke();

                Write(null, null, null, data, text, writeLine, false);
            }
            catch (Exception ex)
            {
                string msgText = !writeLine || text != null
                    ? $", text '{text}'"
                    : null;
                string msg = $"Write{(writeLine ? "Line" : null)} failed for {this}{msgText}";
                CatchLoggerException(msg, ex);
            }
        }

        private void WriteEvent(Enum eventType, byte[] data, string text, bool writeLine)
        {
            try
            {
                PreInitialize?.Invoke();

                int eventTypeId = eventType != null ? (int) (object) eventType : 0;
                Type callerType = CallerTypesCache.GetCallerType();
                string eventName;
                bool immediateFlush;
                if (ProcessText(callerType, eventType, eventTypeId, data, ref text, out eventName, out immediateFlush))
                {
                    Write(callerType, eventType, eventName, data, text, writeLine, immediateFlush);
                }
            }
            catch (Exception ex)
            {
                string msgText = !writeLine || text != null
                    ? $", text '{text}'"
                    : null;
                string msg = $"Write{(writeLine ? "Line" : null)} failed for {this}, event '{eventType?.ToString()}', text '{msgText}'";
                CatchLoggerException(msg, ex);
            }
        }

        private void WriteException(Enum eventType, Exception ex, byte[] data, string text, bool writeLine)
        {
            if (ex == null) return;

            using (LSObject lsObj = LS.Push())
            {
                lsObj.Exception = ex;
                WriteEvent(eventType, data, text, writeLine);
            }
        }

#endregion

#region Public methods

        public override void Write(string text)
        {
            WritePlain(null, text, false);
        }

        public override void WriteLine()
        {
            WritePlain(null, null, true);
        }

        public override void WriteLine(string text)
        {
            WritePlain(null, text, true);
        }

        public override void Write(Enum eventType, string text)
        {
            WriteEvent(eventType, null, text, false);
        }

        public override void WriteLine(Enum eventType)
        {
            WriteEvent(eventType, null, null, true);
        }

        public override void WriteLine(Enum eventType, string text)
        {
            WriteEvent(eventType, null, text, true);
        }

        public override void Write(Enum eventType, Exception ex)
        {
            WriteException(eventType, ex, null, null, false);
        }

        public override void Write(Enum eventType, Exception ex, string text)
        {
            WriteException(eventType, ex, null, text, false);
        }

        public override void WriteLine(Enum eventType, Exception ex)
        {
            WriteException(eventType, ex, null, null, true);
        }

        public override void WriteLine(Enum eventType, Exception ex, string text)
        {
            WriteException(eventType, ex, null, text, true);
        }

        public override void WriteData(byte[] data)
        {
            WritePlain(data, null, false);
        }

        public override void WriteData(byte[] data, string text)
        {
            WritePlain(data, null, false);
        }

        public override void WriteData(Enum eventType, byte[] data)
        {
            WriteEvent(eventType, data, null, false);
        }

        public override void WriteData(Enum eventType, byte[] data, string text)
        {
            WriteEvent(eventType, data, text, false);
        }

        public override void WriteData(Enum eventType, Exception ex, byte[] data)
        {
            WriteException(eventType, ex, data, null, true);
        }

        public override void WriteData(Enum eventType, Exception ex, byte[] data, string text)
        {
            WriteException(eventType, ex, data, text, true);
        }

        public override bool Suspend()
        {
            return true;
        }

        public override bool Resume()
        {
            return true;
        }

        public override void Flush() { }

        public override void Dispose()
        {
            Deinitialize();
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Settings.Name)
                ? $"{GetType().Name} [{Settings.Name}]"
                : $"{GetType().Name}";
        }

#endregion

    }
}

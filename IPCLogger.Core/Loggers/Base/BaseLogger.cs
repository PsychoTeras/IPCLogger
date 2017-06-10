using System;
using System.Xml;
using IPCLogger.Core.Assemblies;
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

#region Ctor

        static BaseLoggerInt()
        {
            EmbeddedAsmsLoader.Init();
        }

#endregion

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

        protected internal abstract void Write(Type callerType, Enum eventType, 
            string eventName, string text, bool writeLine);

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

#endregion

#region Ctor

        protected BaseLogger() : this(false) { }

        protected BaseLogger(bool newInstance)
        {
            InitSettings();
            SetPatternsFactory(newInstance ? PFactory.CreateNewInstance() : PFactory.Instance);
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
            Settings = (TSettings) Activator.CreateInstance(typeof (TSettings), GetType(),
                new Action(OnSetupSettings));
        }

        private bool ProcessText(Type callerType, Enum eventType, int eventTypeId, ref string text,
            out string eventName)
        {
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
                    text = SFactory.Process(callerType, eventType, text, pattern, Patterns);
                }
            }
            else
            {
                eventName = null;
            }
            return true;
        }

        private void WritePlain(string text, bool writeLine)
        {
            try
            {
                if (PreInitialize != null) PreInitialize();
                Write(null, null, null, text, writeLine);
            }
            catch (Exception ex)
            {
                string msgText = !writeLine || text != null
                    ? string.Format(", text '{0}'", text)
                    : null;
                string msg = string.Format("Write{0} failed for {1}{2}",
                    writeLine ? "Line" : null, this, msgText);
                CatchLoggerException(msg, ex);
            }
        }

        private void WriteEvent(Enum eventType, string text, bool writeLine)
        {
            try
            {
                if (PreInitialize != null) PreInitialize();
                int eventTypeId = eventType != null ? (int) (object) eventType : 0;
                Type callerType = CallerTypesCache.GetCallerType();
                string eventName;
                if (ProcessText(callerType, eventType, eventTypeId, ref text, out eventName))
                {
                    Write(callerType, eventType, eventName, text, writeLine);
                }
            }
            catch (Exception ex)
            {
                string msgText = !writeLine || text != null
                    ? string.Format(", text '{0}'", text)
                    : null;
                string msg = string.Format("Write{0} failed for {1}, event '{2}', text '{3}'",
                    writeLine ? "Line" : null, this, eventType != null ? eventType.ToString() : null,
                    msgText);
                CatchLoggerException(msg, ex);
            }
        }

        private void WriteException(Enum eventType, Exception ex, string text, bool writeLine)
        {
            if (ex == null) return;
            using (LSObject lsObj = LS.Push())
            {
                lsObj.Exception = ex;
                WriteEvent(eventType, text, writeLine);
            }
        }

#endregion

#region Public methods

        public IBaseLogger CreateNewInstance()
        {
            return Activator.CreateInstance(GetType(), true) as IBaseLogger;
        }

        public override void Write(string text)
        {
            WritePlain(text, false);
        }

        public override void WriteLine()
        {
            WritePlain(null, true);
        }

        public override void WriteLine(string text)
        {
            WritePlain(text, true);
        }

        public override void Write(Enum eventType, string text)
        {
            WriteEvent(eventType, text, false);
        }

        public override void WriteLine(Enum eventType)
        {
            WriteEvent(eventType, null, true);
        }

        public override void WriteLine(Enum eventType, string text)
        {
            WriteEvent(eventType, text, true);
        }

        public override void Write(Enum eventType, Exception ex)
        {
            WriteException(eventType, ex, null, false);
        }

        public override void Write(Enum eventType, Exception ex, string text)
        {
            WriteException(eventType, ex, text, false);
        }

        public override void WriteLine(Enum eventType, Exception ex)
        {
            WriteException(eventType, ex, null, true);
        }

        public override void WriteLine(Enum eventType, Exception ex, string text)
        {
            WriteException(eventType, ex, text, true);
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
                ? string.Format("'{0}', name '{1}'", GetType().Name, Settings.Name)
                : string.Format("'{0}'", GetType().Name);
        }

#endregion

    }
}

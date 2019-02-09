using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Snippets;
using System;
using System.IO;

namespace IPCLogger.Core.Loggers.LConsole
{
    public sealed class LConsole : BaseLogger<LConsoleSettings>
    {

#region Private fields

        private TextWriter _consoleWriter;

        private bool _eventIsHappend;
        private string _prewEventName;
        private ConsoleColor _prewForeColor;
        private ConsoleColor _prewBackColor;

        private ConsoleColor _defForeColor;
        private ConsoleColor _defBackColor;

        private volatile bool _initialized;

#endregion

#region Ctor

        public LConsole(bool threadSafetyGuaranteed)
            : base(threadSafetyGuaranteed)
        {
        }

#endregion

#region ILogger

        private void SetupSettings()
        {
            if (!_initialized) return;

            if (!string.IsNullOrEmpty(Settings.Title))
            {
                Console.Title = SFactory.Process(Settings.Title, Patterns);
            }

            _consoleWriter = Settings.StreamType == ConsoleStreamType.Error
                ? Console.Error
                : Console.Out;

            LConsoleSettings.HighlightSettings highlights = Settings.Highlights;
            highlights.DefConsoleForeColor = highlights.DefConsoleForeColor ?? _defForeColor;
            highlights.DefConsoleBackColor = highlights.DefConsoleBackColor ?? _defBackColor;
            _prewEventName = null;
            _eventIsHappend = false;
        }

        protected override void OnSetupSettings()
        {
            SetupSettings();
        }

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine, bool immediateFlush)
        {
            if (!_initialized) return;

            SetColors(eventName);

            if (writeLine)
            {
                _consoleWriter.WriteLine(text);
            }
            else
            {
                _consoleWriter.Write(text);
            }
        }

        public override void Initialize()
        {
            try
            {
                _initialized = Console.WindowHeight > 0;
                _defForeColor = Console.ForegroundColor;
                _defBackColor = Console.BackgroundColor;
                SetupSettings();
            }
            catch
            {
                _initialized = false;
            }
        }

        public override void Deinitialize() { }

#endregion

#region Class methods

        private void SetColors(string eventName)
        {
            if (_eventIsHappend && _prewEventName == eventName)
            {
                Console.ForegroundColor = _prewForeColor;
                Console.BackgroundColor = _prewBackColor;
            }

            LConsoleSettings.HighlightSettings highlights = Settings.Highlights;

            if (eventName != null && highlights.ConsoleForeColors.TryGetValue(eventName, out var color))
            {
                Console.ForegroundColor = color;
            }
            else
            {
                color = highlights.DefConsoleForeColor.Value;
                Console.ForegroundColor = color;
            }
            _prewForeColor = Console.ForegroundColor;

            if (eventName != null && highlights.ConsoleBackColors.TryGetValue(eventName, out color))
            {
                Console.BackgroundColor = color;
            }
            else
            {
                color = highlights.DefConsoleBackColor.Value;
                Console.BackgroundColor = color;
            }
            _prewBackColor = Console.BackgroundColor;

            _eventIsHappend = true;
            _prewEventName = eventName;
        }

#endregion

    }
}

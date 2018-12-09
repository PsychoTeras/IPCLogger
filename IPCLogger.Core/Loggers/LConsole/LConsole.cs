using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Snippets;
using System;

namespace IPCLogger.Core.Loggers.LConsole
{
    public sealed class LConsole : BaseLogger<LConsoleSettings>
    {

#region Private fields

        private volatile bool _initialized;
        private ConsoleColor _defForeColor;
        private ConsoleColor _defBackColor;

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
            if (_initialized && !string.IsNullOrEmpty(Settings.Title))
            {
                Console.Title = SFactory.Process(Settings.Title, Patterns);
            }

            LConsoleSettings.HighlightSettings highlights = Settings.Highlights;
            highlights.DefConsoleForeColor = highlights.DefConsoleForeColor ?? _defForeColor;
            highlights.DefConsoleBackColor = highlights.DefConsoleBackColor ?? _defBackColor;
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
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
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
            LConsoleSettings.HighlightSettings highlights = Settings.Highlights;

            if (eventName != null && highlights.ConsoleForeColors.TryGetValue(eventName, out var color))
            {
                Console.ForegroundColor = color;
            }
            else
            {
                color = highlights.DefConsoleForeColor.Value;
                if (highlights.DefConsoleForeColor != Console.ForegroundColor)
                {
                    Console.ForegroundColor = color;
                }
            }

            if (eventName != null && highlights.ConsoleBackColors.TryGetValue(eventName, out color))
            {
                Console.BackgroundColor = color;
            }
            else
            {
                color = highlights.DefConsoleBackColor.Value;
                if (color != Console.BackgroundColor)
                {
                    Console.BackgroundColor = color;
                }
            }
        }

        public int Read()
        {
            return _initialized ? Console.Read() : -1;
        }

        public string ReadLine()
        {
            return _initialized ? Console.ReadLine() : null;
        }

        public ConsoleKeyInfo ReadKey()
        {
            return _initialized ? Console.ReadKey() : default(ConsoleKeyInfo);
        }

#endregion

    }
}

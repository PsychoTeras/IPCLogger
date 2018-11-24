using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Snippets;
using System;

namespace IPCLogger.Core.Loggers.LConsole
{
    public sealed class LConsole : BaseLogger<LConsoleSettings>
    {

#region Private fields

        private volatile bool _initialized;

#endregion

#region Ctor

        public LConsole(bool threadSafetyGuaranteed)
            : base(threadSafetyGuaranteed)
        {
        }

#endregion

#region ILogger

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            byte[] data, string text, bool writeLine, bool immediateFlush)
        {
            if (!_initialized) return;

            bool isColorsSet = SetColors(eventName);

            if (writeLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }

            if (isColorsSet)
            {
                Console.ResetColor();
            }
        }

        public override void Initialize()
        {
            try
            {
                _initialized = Console.WindowHeight > 0;
                if (!string.IsNullOrEmpty(Settings.Title))
                {
                    Console.Title = SFactory.Process(Settings.Title, Patterns);
                }
            }
            catch
            {
                _initialized = false;
            }
        }

        public override void Deinitialize() { }

#endregion

#region Class methods

        private bool SetColors(string eventName)
        {
            ConsoleColor color;
            bool isSet = false;
            if (Settings.ConsoleForeColors != null && eventName != null && Settings.ConsoleForeColors.TryGetValue(eventName, out color))
            {
                Console.ForegroundColor = color;
                isSet = true;
            }
            else if (Settings.DefConsoleForeColor.HasValue)
            {
                Console.ForegroundColor = Settings.DefConsoleForeColor.Value;
                isSet = true;
            }
            if (Settings.ConsoleBackColors != null && eventName != null && Settings.ConsoleBackColors.TryGetValue(eventName, out color))
            {
                Console.BackgroundColor = color;
                isSet = true;
            }
            else if (Settings.DefConsoleBackColor.HasValue)
            {
                Console.BackgroundColor = Settings.DefConsoleBackColor.Value;
                isSet = true;
            }
            return isSet;
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

using System;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Snippets;

namespace IPCLogger.Core.Loggers.LConsole
{
    public sealed class LConsole : SimpleLogger<LConsoleSettings>
    {

#region Ctor

        public LConsole(bool threadSafetyIsGuaranteed)
            : base(threadSafetyIsGuaranteed)
        {
        }

#endregion

#region ILogger

        protected override void WriteSimple(Type callerType, Enum eventType, string eventName,
            string text, bool writeLine)
        {
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

        protected override bool InitializeSimple()
        {
            try
            {
                if (Console.WindowHeight <= 0) return false;

                if (!string.IsNullOrEmpty(Settings.Title))
                {
                    Console.Title = SFactory.Process(Settings.Title, Patterns);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool DeinitializeSimple()
        {
            return true;
        }

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
            return Initialized ? Console.Read() : -1;
        }

        public string ReadLine()
        {
            return Initialized ? Console.ReadLine() : null;
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Initialized ? Console.ReadKey() : default(ConsoleKeyInfo);
        }

#endregion

    }
}

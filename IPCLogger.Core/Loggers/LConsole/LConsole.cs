using System;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LConsole
{
    public sealed class LConsole : BaseLogger<LConsoleSettings>
    {

#region Private fields

        private volatile bool _initialized;

#endregion

#region ILogger

        protected internal override void Write(Type callerType, Enum eventType, string eventName,
            string text, bool writeLine)
        {
            if (!_initialized) return;

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
            }
            catch
            {
                _initialized = false;
            }
        }

        public override void Deinitialize() { }

#endregion

#region Class methods

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

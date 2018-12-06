using IPCLogger.Core.Loggers.Base;
using System;
using System.Collections.Generic;

namespace IPCLogger.Core.Loggers.LConsole
{
    public sealed class LConsoleSettings : BaseSettings
    {

#region Declarations

        public class HighlightSettings
        {
            public ConsoleColor? DefConsoleForeColor;
            public Dictionary<string, ConsoleColor> ConsoleForeColors;

            public ConsoleColor? DefConsoleBackColor;
            public Dictionary<string, ConsoleColor> ConsoleBackColors;
        }

#endregion

#region Constants

        private const string HIGHLIGHT_NODE_NAME = "Highlight";
        private const string FORECOLOR_NODE_NAME = "ForeColor";
        private const string BACKCOLOR_NODE_NAME = "BackColor";

#endregion

#region Properties

        public string Title { get; set; }

        [ConsoleHighlightsConversion]
        public HighlightSettings Highlights { get; set; }

#endregion

#region Ctor

        public LConsoleSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges) { }

#endregion

    }
}

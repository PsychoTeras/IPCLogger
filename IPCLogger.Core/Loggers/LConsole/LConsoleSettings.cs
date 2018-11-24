﻿using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using System;
using System.Collections.Generic;
using System.Xml;

namespace IPCLogger.Core.Loggers.LConsole
{
    // ReSharper disable PossibleNullReferenceException
    public sealed class LConsoleSettings : BaseSettings
    {

#region Constants

        private const string HIGHLIGHT_NODE_NAME = "Highlight";
        private const string FORECOLOR_NODE_NAME = "ForeColor";
        private const string BACKCOLOR_NODE_NAME = "BackColor";

#endregion

#region Internal fields

        internal ConsoleColor? DefConsoleForeColor;
        internal Dictionary<string, ConsoleColor> ConsoleForeColors;

        internal ConsoleColor? DefConsoleBackColor;
        internal Dictionary<string, ConsoleColor> ConsoleBackColors;

#endregion

#region Properties

        public string Title { get; set; }

#endregion

#region Ctor

        public LConsoleSettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges) { }

        protected override void BeginSetup()
        {
            DefConsoleForeColor = null;
            DefConsoleBackColor = null;
        }

        private void ReadAndSetColor(XmlNode highlightNode, string colorType, string @event,
            ref ConsoleColor? defConsoleColor, Dictionary<string, ConsoleColor> consoleColors)
        {
            string sColor;
            XmlNode colorNode = highlightNode.SelectSingleNode(colorType);
            if (colorNode != null && !string.IsNullOrEmpty(sColor = colorNode.InnerText.Trim()))
            {
                if (!Enum.IsDefined(typeof(ConsoleColor), sColor))
                {
                    string msg = $"Invalid {colorType} value '{sColor}'";
                    throw new Exception(msg);
                }
                ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), sColor);
                if (Constants.ApplicableForAllMark == @event)
                {
                    if (defConsoleColor.HasValue)
                    {
                        string msg = $"Duplicated default {colorType} definition";
                        throw new Exception(msg);
                    }
                    defConsoleColor = color;
                }
                else
                {
                    if (consoleColors.ContainsKey(@event))
                    {
                        string msg = $"Duplicated {colorType} definition for event '{@event}'";
                        throw new Exception(msg);
                    }
                    consoleColors.Add(@event, color);
                }
            }
        }

        private void ReadHighlightSettings(XmlNode cfgNode)
        {
            XmlNodeList highlightNodes = cfgNode.SelectNodes(HIGHLIGHT_NODE_NAME);
            if (highlightNodes == null) return;

            ConsoleForeColors = new Dictionary<string, ConsoleColor>();
            ConsoleBackColors = new Dictionary<string, ConsoleColor>();

            foreach (XmlNode highlightNode in highlightNodes)
            {
                XmlAttribute aEvents = highlightNode.Attributes["events"];
                string sEvents = aEvents?.Value;
                string[] events = string.IsNullOrWhiteSpace(sEvents)
                    ? new[] {Constants.ApplicableForAllMark}
                    : sEvents.Split(Constants.Splitter);
                foreach (string s in events)
                {
                    string @event = s.Trim();
                    if (string.IsNullOrEmpty(@event)) continue;

                    ReadAndSetColor(highlightNode, FORECOLOR_NODE_NAME, @event, ref DefConsoleForeColor, ConsoleForeColors);
                    ReadAndSetColor(highlightNode, BACKCOLOR_NODE_NAME, @event, ref DefConsoleBackColor, ConsoleBackColors);
                }
            }
        }

        protected override Dictionary<string, string> GetSettingsDictionary(XmlNode cfgNode)
        {
            ReadHighlightSettings(cfgNode);
            return GetSettingsDictionary(cfgNode, new[] { HIGHLIGHT_NODE_NAME });
        }

#endregion

    }
    // ReSharper restore PossibleNullReferenceException
}

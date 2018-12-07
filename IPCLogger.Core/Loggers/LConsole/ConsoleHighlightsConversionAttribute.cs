using IPCLogger.Core.Attributes;
using IPCLogger.Core.Common;
using System;
using System.Collections.Generic;
using System.Xml;

namespace IPCLogger.Core.Loggers.LConsole
{
    public sealed class ConsoleHighlightsConversionAttribute : XmlNodesConversionAttribute
    {
        private const string HIGHLIGHT_NODE_NAME = "Highlight";
        private const string FORECOLOR_NODE_NAME = "ForeColor";
        private const string BACKCOLOR_NODE_NAME = "BackColor";

        public ConsoleHighlightsConversionAttribute() 
            : base(HIGHLIGHT_NODE_NAME)
        {
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

        private LConsoleSettings.HighlightSettings ReadHighlightSettings(XmlNode[] highlightNodes)
        {
            LConsoleSettings.HighlightSettings colors = new LConsoleSettings.HighlightSettings();

            colors.ConsoleForeColors = new Dictionary<string, ConsoleColor>();
            colors.ConsoleBackColors = new Dictionary<string, ConsoleColor>();

            foreach (XmlNode highlightNode in highlightNodes)
            {
                XmlAttribute aEvents = highlightNode.Attributes?["events"];
                string sEvents = aEvents?.Value;
                string[] events = string.IsNullOrWhiteSpace(sEvents)
                    ? new[] { Constants.ApplicableForAllMark }
                    : sEvents.Split(Constants.Splitter);
                foreach (string s in events)
                {
                    string @event = s.Trim();
                    if (string.IsNullOrEmpty(@event)) continue;

                    ReadAndSetColor(highlightNode, FORECOLOR_NODE_NAME, @event, ref colors.DefConsoleForeColor, colors.ConsoleForeColors);
                    ReadAndSetColor(highlightNode, BACKCOLOR_NODE_NAME, @event, ref colors.DefConsoleBackColor, colors.ConsoleBackColors);
                }
            }

            return colors;
        }

        public override object XmlNodesToValue(XmlNode[] xmlNodes)
        {
            if (xmlNodes?.Length == 0)
            {
                string msg = "XmlNodes cannot be null or empty";
                throw new Exception(msg);
            }

            return ReadHighlightSettings(xmlNodes);
        }

        public override void ValueToXmlNodes(object value, XmlNode[] xmlNodes)
        {
        }
    }
}

using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            LConsoleSettings.HighlightSettings settings = new LConsoleSettings.HighlightSettings();

            settings.ConsoleForeColors = new Dictionary<string, ConsoleColor>();
            settings.ConsoleBackColors = new Dictionary<string, ConsoleColor>();

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

                    ReadAndSetColor(highlightNode, FORECOLOR_NODE_NAME, @event, ref settings.DefConsoleForeColor, settings.ConsoleForeColors);
                    ReadAndSetColor(highlightNode, BACKCOLOR_NODE_NAME, @event, ref settings.DefConsoleBackColor, settings.ConsoleBackColors);
                }
            }

            return settings;
        }

        public override object XmlNodesToValue(XmlNode[] xmlNodes)
        {
            if (xmlNodes == null)
            {
                string msg = "XmlNodes cannot be null or empty";
                throw new Exception(msg);
            }

            return ReadHighlightSettings(xmlNodes);
        }

        public override void ValueToXmlNodes(object value, XmlNode[] xmlNodes)
        {
        }

        public override string ValueToCSString(object value)
        {
            string MakeColorSettings(string mark, ConsoleColor? foreColor, ConsoleColor? backColor, bool @fixed)
            {
                return "{ " +
                       $"\"col1\": \"{mark}\", " +
                       $"\"col2\": \"{(foreColor.HasValue ? foreColor.Value.ToString() : string.Empty)}\", " +
                       $"\"col3\": \"{(backColor.HasValue ? backColor.Value.ToString() : string.Empty)}\", " +
                       $"\"fixed\": \"{@fixed}\"" +
                       " }";
            }

            if (!(value is LConsoleSettings.HighlightSettings settings))
            {
                string msg = "HighlightSettings object is null or has wrong type";
                throw new Exception(msg);
            }

            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("{ \"colsNumber\": 3, \"col1\": \"Applicable for events\", \"col2\": \"ForeColor\", \"col3\": \"BackColor\"");

            List<string> entries = new List<string>();
            string val = MakeColorSettings(Constants.ApplicableForAllMark, 
                settings.DefConsoleForeColor, 
                settings.DefConsoleBackColor, 
                true);
            entries.Add(val);

            IEnumerable<string> events = settings.ConsoleForeColors.
                Select(s => s.Key).
                Concat(settings.ConsoleBackColors.Select(s => s.Key)).
                Distinct();

            foreach (string @event in events)
            {
                val = MakeColorSettings(@event,
                    settings.ConsoleForeColors.TryGetValue(@event, out var foreColor) ? foreColor : (ConsoleColor?) null,
                    settings.ConsoleBackColors.TryGetValue(@event, out var backColor) ? backColor : (ConsoleColor?) null,
                    false);
                entries.Add(val);
            }

            sbJson.Append($", \"values\":[ {string.Join(",", entries)}] }}");
            return sbJson.ToString();
        }
    }
}

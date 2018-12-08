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

        private void SetColor(string sColor, string colorType, string @event,
            ref ConsoleColor? defConsoleColor, Dictionary<string, ConsoleColor> consoleColors)
        {
            if (string.IsNullOrEmpty(sColor))
            {
                return;
            }

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

        private void ReadAndSetColor(XmlNode highlightNode, string colorType, string @event,
            ref ConsoleColor? defConsoleColor, Dictionary<string, ConsoleColor> consoleColors)
        {
            XmlNode colorNode = highlightNode.SelectSingleNode(colorType);
            if (colorNode != null)
            {
                SetColor(colorNode.InnerText.Trim(), colorType, @event, ref defConsoleColor, consoleColors);
            }
        }

        private string[] SplitEvents(string sEvents)
        {
            return string.IsNullOrWhiteSpace(sEvents)
                ? new[] {Constants.ApplicableForAllMark}
                : sEvents.Split(Constants.Splitter).Select(s => s.Trim()).ToArray();
        }

        private LConsoleSettings.HighlightSettings ReadHighlightSettings(XmlNode[] highlightNodes)
        {
            LConsoleSettings.HighlightSettings settings = new LConsoleSettings.HighlightSettings();

            foreach (XmlNode highlightNode in highlightNodes)
            {
                XmlAttribute aEvents = highlightNode.Attributes?["events"];
                string[] events = SplitEvents(aEvents?.Value);

                foreach (string @event in events)
                {
                    if (string.IsNullOrEmpty(@event)) continue;

                    ReadAndSetColor(highlightNode, FORECOLOR_NODE_NAME, @event, ref settings.DefConsoleForeColor,
                        settings.ConsoleForeColors);
                    ReadAndSetColor(highlightNode, BACKCOLOR_NODE_NAME, @event, ref settings.DefConsoleBackColor,
                        settings.ConsoleBackColors);
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

            string values = Helpers.StringListToString(new []{ " " }.Concat(Enum.GetNames(typeof(ConsoleColor))), Constants.Splitter);

            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("{ \"colsNumber\": 3, \"col1\": \"Applicable for events\", \"col2\": \"ForeColor\", \"col3\": \"BackColor\", " +
                          $"\"col2Values\": \"{values}\", \"col3Values\": \"%col2Values%\"");

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

        public override object CSStringToValue(string sValue)
        {
            LConsoleSettings.HighlightSettings settings = new LConsoleSettings.HighlightSettings();

            List<Dictionary<string, string>> jsonObject = sValue?.FromJson<List<Dictionary<string, string>>>();

            foreach (Dictionary<string, string> dict in jsonObject.Select(d => d))
            {
                string sEvents = dict["col1"];
                string[] events = SplitEvents(sEvents);

                foreach (string @event in events)
                {
                    if (string.IsNullOrEmpty(@event)) continue;

                    string sForeColor = dict["col2"];
                    SetColor(sForeColor, FORECOLOR_NODE_NAME, @event, ref settings.DefConsoleForeColor,
                        settings.ConsoleForeColors);

                    string sBackColor = dict["col3"];
                    SetColor(sBackColor, BACKCOLOR_NODE_NAME, @event, ref settings.DefConsoleBackColor,
                        settings.ConsoleBackColors);
                }
            }

            return settings;
        }
    }
}

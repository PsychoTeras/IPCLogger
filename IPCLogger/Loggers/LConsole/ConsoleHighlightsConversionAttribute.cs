using IPCLogger.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace IPCLogger.Loggers.LConsole
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

        private void SetColor(string sColor, string colorType, string eventName,
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
            if (Constants.ApplicableForAllMark == eventName)
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
                if (consoleColors.ContainsKey(eventName))
                {
                    string msg = $"Duplicated {colorType} definition for event '{eventName}'";
                    throw new Exception(msg);
                }
                consoleColors.Add(eventName, color);
            }
        }

        private void ReadAndSetColor(XmlNode highlightNode, string colorType, string eventName,
            ref ConsoleColor? defConsoleColor, Dictionary<string, ConsoleColor> consoleColors)
        {
            XmlNode colorNode = highlightNode.SelectSingleNode(colorType);
            if (colorNode != null)
            {
                SetColor(colorNode.InnerText.Trim(), colorType, eventName, ref defConsoleColor, consoleColors);
            }
        }

        private IEnumerable<string> SplitEvents(string sEvents)
        {
            return string.IsNullOrWhiteSpace(sEvents)
                ? new[] {Constants.ApplicableForAllMark}
                : sEvents.Split(Constants.Splitter).
                          Select(s => s.Trim()).
                          Where(s => s != string.Empty);
        }

        private LConsoleSettings.HighlightSettings ReadHighlightSettings(IEnumerable<XmlNode> highlightNodes)
        {
            LConsoleSettings.HighlightSettings settings = new LConsoleSettings.HighlightSettings();

            foreach (XmlNode highlightNode in highlightNodes)
            {
                XmlAttribute aEvents = highlightNode.Attributes?["events"];
                IEnumerable<string> events = SplitEvents(aEvents?.Value);

                foreach (string eventName in events)
                {
                    ReadAndSetColor(highlightNode, FORECOLOR_NODE_NAME, eventName, ref settings.DefConsoleForeColor,
                        settings.ConsoleForeColors);
                    ReadAndSetColor(highlightNode, BACKCOLOR_NODE_NAME, eventName, ref settings.DefConsoleBackColor,
                        settings.ConsoleBackColors);
                }
            }

            return settings;
        }

        public override object XmlNodesToValue(XmlNode cfgNode)
        {
            if (cfgNode == null)
            {
                string msg = "XmlNodes cannot be null or empty";
                throw new Exception(msg);
            }

            IEnumerable<XmlNode> xmlNodes = cfgNode.ChildNodes.OfType<XmlNode>().
                Where(n => ExclusiveNodeNames.Contains(n.Name));

            return ReadHighlightSettings(xmlNodes);
        }

        private Dictionary<Tuple<ConsoleColor?, ConsoleColor?>, List<string>> GroupEventsAndColors(
            LConsoleSettings.HighlightSettings settings)
        {
            void AddGroupedEvent(Dictionary<Tuple<ConsoleColor?, ConsoleColor?>, List<string>> dict,
                Tuple<ConsoleColor?, ConsoleColor?> item, string eventName)
            {
                if (!dict.TryGetValue(item, out var eventList))
                {
                    eventList = new List<string>();
                    dict.Add(item, eventList);
                }
                eventList.Add(eventName);
            }

            Tuple<ConsoleColor?, ConsoleColor?> MakeColorItem(ConsoleColor? foreColor, ConsoleColor? backColor)
            {
                return new Tuple<ConsoleColor?, ConsoleColor?>(foreColor, backColor);
            }

            var groupedEvents = new Dictionary<Tuple<ConsoleColor?, ConsoleColor?>, List<string>>();

            if (settings.DefConsoleForeColor.HasValue || settings.DefConsoleBackColor.HasValue)
            {
                var item = MakeColorItem(settings.DefConsoleForeColor, settings.DefConsoleBackColor);
                AddGroupedEvent(groupedEvents, item, Constants.ApplicableForAllMark);
            }

            IEnumerable<string> events = settings.ConsoleForeColors.
                Select(s => s.Key).
                Concat(settings.ConsoleBackColors.Select(s => s.Key)).
                Distinct();
            foreach (string eventName in events)
            {
                Tuple<ConsoleColor?, ConsoleColor?> item = MakeColorItem(
                    settings.ConsoleForeColors.TryGetValue(eventName, out var foreColor) ? foreColor : (ConsoleColor?)null,
                    settings.ConsoleBackColors.TryGetValue(eventName, out var backColor) ? backColor : (ConsoleColor?)null);
                AddGroupedEvent(groupedEvents, item, eventName);
            }

            return groupedEvents;
        }

        // ReSharper disable PossibleNullReferenceException
        private void WriteHighlightSettings(Tuple<ConsoleColor?, ConsoleColor?> colorItem, string sEvents, XmlNode cfgNode)
        {
            XmlDocument xmlDoc = cfgNode.OwnerDocument;

            void AppendColorNode(string colorType, ConsoleColor? color, XmlNode node)
            {
                if (!color.HasValue) return;

                XmlNode colorNode = xmlDoc.CreateNode(XmlNodeType.Element, colorType, xmlDoc.NamespaceURI);
                colorNode.InnerText = color.Value.ToString();
                node.AppendChild(colorNode);
            }

            XmlNode valNode = xmlDoc.CreateNode(XmlNodeType.Element, HIGHLIGHT_NODE_NAME, xmlDoc.NamespaceURI);
            cfgNode.AppendChild(valNode);

            if (sEvents != Constants.ApplicableForAllMark)
            {
                XmlAttribute valAttribute = xmlDoc.CreateAttribute("events");
                valNode.Attributes.Append(valAttribute);
                valAttribute.InnerText = sEvents;
            }

            AppendColorNode(FORECOLOR_NODE_NAME, colorItem.Item1, valNode);
            AppendColorNode(BACKCOLOR_NODE_NAME, colorItem.Item2, valNode);
        }
        // ReSharper restore PossibleNullReferenceException

        public override void ValueToXmlNodes(object value, XmlNode cfgNode)
        {
            XmlNode[] xmlNodes = cfgNode.ChildNodes.
                OfType<XmlNode>().
                Where(n => ExclusiveNodeNames.Contains(n.Name)).
                ToArray();
            foreach (XmlNode node in xmlNodes)
            {
                cfgNode.RemoveChild(node);
            }

            if (!(value is LConsoleSettings.HighlightSettings settings))
            {
                return;
            }

            Dictionary<Tuple<ConsoleColor?, ConsoleColor?>, List<string>> groupedEvents = GroupEventsAndColors(settings);

            foreach (var groupedEvent in groupedEvents)
            {
                Tuple<ConsoleColor?, ConsoleColor?> colorItem = groupedEvent.Key;
                string sEvents = Helpers.StringListToString(groupedEvents[colorItem], Constants.SplitterString);
                WriteHighlightSettings(colorItem, sEvents, cfgNode);
            }
        }

        public override string ValueToCSString(object value)
        {
            string MakeColorSettings(string mark, ConsoleColor? foreColor, ConsoleColor? backColor)
            {
                return "{ " +
                       $"\"col1\": \"{mark}\", " +
                       $"\"col2\": \"{(foreColor.HasValue ? foreColor.Value.ToString() : string.Empty)}\", " +
                       $"\"col3\": \"{(backColor.HasValue ? backColor.Value.ToString() : string.Empty)}\"" +
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

            if (settings.DefConsoleForeColor.HasValue || settings.DefConsoleForeColor.HasValue)
            {
                string val = MakeColorSettings(Constants.ApplicableForAllMark,
                    settings.DefConsoleForeColor,
                    settings.DefConsoleBackColor);
                entries.Add(val);
            }

            IEnumerable<string> events = settings.ConsoleForeColors.
                Select(s => s.Key).
                Concat(settings.ConsoleBackColors.Select(s => s.Key)).
                Distinct();
            foreach (string eventName in events)
            {
                string val = MakeColorSettings(eventName,
                    settings.ConsoleForeColors.TryGetValue(eventName, out var foreColor) ? foreColor : (ConsoleColor?) null,
                    settings.ConsoleBackColors.TryGetValue(eventName, out var backColor) ? backColor : (ConsoleColor?) null);
                entries.Add(val);
            }

            sbJson.Append($", \"values\":[ {string.Join(",", entries)}] }}");
            return sbJson.ToString();
        }

        public override object CSStringToValue(string sValue)
        {
            LConsoleSettings.HighlightSettings settings = new LConsoleSettings.HighlightSettings();

            var jsonObject = sValue?.FromJson<List<Dictionary<string, string>>>();

            foreach (Dictionary<string, string> dict in jsonObject.Select(d => d))
            {
                string sEvents = dict["col1"];
                IEnumerable<string> events = SplitEvents(sEvents);
                foreach (string eventName in events)
                {
                    string sForeColor = dict["col2"];
                    SetColor(sForeColor, FORECOLOR_NODE_NAME, eventName, ref settings.DefConsoleForeColor,
                        settings.ConsoleForeColors);

                    string sBackColor = dict["col3"];
                    SetColor(sBackColor, BACKCOLOR_NODE_NAME, eventName, ref settings.DefConsoleBackColor,
                        settings.ConsoleBackColors);
                }
            }

            return settings;
        }
    }
}

using System;
using System.Linq;
using System.Xml;

namespace IPCLogger.Core.Attributes
{
    public abstract class XmlNodesConversionAttribute : CustomConversionAttribute
    {
        public string[] ExclusiveNodeNames { get; }

        public override sealed ConversionSource SourceType { get => ConversionSource.XmlNodes; }

        public XmlNodesConversionAttribute(string exclusiveNodeName)
        {
            if (string.IsNullOrWhiteSpace(exclusiveNodeName))
            {
                string msg = "Exclusive node name cannot be empty";
                throw new Exception(msg);
            }
            ExclusiveNodeNames = new[] { exclusiveNodeName };
        }

        public XmlNodesConversionAttribute(string[] exclusiveNodeNames)
        {
            exclusiveNodeNames = exclusiveNodeNames?.Length == 0
                ? exclusiveNodeNames.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray()
                : null;

            if (exclusiveNodeNames?.Length == 0)
            {
                string msg = "Exclusive node names cannot be empty";
                throw new Exception(msg);
            }

            ExclusiveNodeNames = exclusiveNodeNames;
        }

        public abstract object RootXmlNodeToValue(XmlNode rootXmlNode);

        public abstract void ValueToRootXmlNode(object value, XmlNode rootXmlNode);

        public override string ValueToCSString(object value)
        {
            XmlNode xmlNode = new XmlDocument().CreateElement("_");
            ValueToRootXmlNode(value, xmlNode);
            return xmlNode.InnerXml;
        }

        public override object CSStringToValue(string sValue)
        {
            XmlNode xmlNode = new XmlDocument().CreateElement("_");
            xmlNode.InnerXml = sValue;
            return RootXmlNodeToValue(xmlNode);
        }
    }
}

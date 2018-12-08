using System;
using System.Linq;
using System.Xml;

namespace IPCLogger.Core.Attributes.CustomConversionAttributes.Base
{
    public abstract class XmlNodesConversionAttribute : CustomConversionAttribute
    {
        public string[] ExclusiveNodeNames { get; }

        public sealed override ConversionSource SourceType { get => ConversionSource.XmlNodes; }

        protected XmlNodesConversionAttribute(string exclusiveNodeName)
        {
            if (string.IsNullOrWhiteSpace(exclusiveNodeName))
            {
                string msg = "Exclusive node name cannot be empty";
                throw new Exception(msg);
            }
            ExclusiveNodeNames = new[] { exclusiveNodeName };
        }

        protected XmlNodesConversionAttribute(string[] exclusiveNodeNames)
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

        public abstract object XmlNodesToValue(XmlNode[] xmlNodes);

        public abstract void ValueToXmlNodes(object value, XmlNode[] xmlNodes);

        public override string ValueToCSString(object value)
        {
            XmlDocument xml = new XmlDocument();
            XmlNode[] xmlNodes = ExclusiveNodeNames.Select(s => xml.CreateElement(s)).OfType<XmlNode>().ToArray();
            ValueToXmlNodes(value, xmlNodes);
            return xml.InnerXml;
        }

        public override object CSStringToValue(string sValue)
        {
            XmlDocument xml = new XmlDocument();
            xml.InnerXml = sValue;
            return XmlNodesToValue(xml.ChildNodes.OfType<XmlNode>().ToArray());
        }
    }
}

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

        public abstract object XmlNodesToValue(XmlNode cfgNode);

        public abstract void ValueToXmlNodes(object value, XmlNode cfgNode);
    }
}

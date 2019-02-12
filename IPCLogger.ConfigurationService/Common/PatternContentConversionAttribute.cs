using IPCLogger.Core.Attributes.CustomConversionAttributes;
using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Core.Patterns;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;

namespace IPCLogger.ConfigurationService.Common
{
    public class PatternContentConversionAttribute : XmlNodesConversionAttribute
    {
        KeyValueConversionAttribute _baseAtr;

        public PatternContentConversionAttribute(Type dataType, string keyName, string valueName)
            : base("Content")
        {
            _baseAtr = new KeyValueConversionAttribute(dataType, keyName, valueName);
        }

        public override object CSStringToValue(string sValue)
        {
            return _baseAtr.CSStringToValue(sValue);
        }

        public override string ValueToCSString(object value)
        {
            return _baseAtr.ValueToCSString(value);
        }

        public override void ValueToXmlNodes(object value, XmlNode cfgNode)
        {
            cfgNode.InnerXml = string.Empty;
            List<KeyValuePair<string, string>> kvList = value as List<KeyValuePair<string, string>>;
            if (kvList?.Count > 0)
            {
                if (kvList.Any(kv => string.IsNullOrEmpty(kv.Value)))
                {
                    string msg = "Pattern content cannot be empty";
                    throw new Exception(msg);
                }

                XmlDocument xmlDoc = cfgNode.OwnerDocument;

                foreach (KeyValuePair<string, string> item in kvList)
                {
                    string applicableFor = item.Key.Trim();
                    string content = item.Value.Trim();
                    XmlNode valNode = xmlDoc.CreateNode(XmlNodeType.Element, "Content", xmlDoc.NamespaceURI);
                    cfgNode.AppendChild(valNode);
                    valNode.InnerText = content;

                    if (!string.IsNullOrEmpty(applicableFor))
                    {
                        XmlAttribute valAttribute = xmlDoc.CreateAttribute(PFactory.PropertyAttributes["ApplicableFor"]);
                        valAttribute.Value = applicableFor;
                        valNode.Attributes.Append(valAttribute);
                    }
                }
            }
        }

        public override object XmlNodesToValue(XmlNode cfgNode)
        {
            return null;
        }
    }
}

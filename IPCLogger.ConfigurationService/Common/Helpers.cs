using System.Security.Cryptography;
using System.Text;
using System.Xml;
using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;

namespace IPCLogger.ConfigurationService.Common
{
    // ReSharper disable PossibleNullReferenceException
    public static class Helpers
    {
        public static string CalculateMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return CalculateMD5(bytes);
        }

        public static string CalculateMD5(byte[] bytes)
        {
            if (bytes?.Length == 0) return null;

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte t in hashBytes)
                {
                    sb.Append($"{t:x2}");
                }
                return sb.ToString();
            }
        }


        public static XmlNode AppendCfgXmlNode(XmlNode cfgNode, string nodeName, string nodePath = null)
        {
            XmlNode valNode = cfgNode.SelectSingleNode(nodePath ?? nodeName);
            if (valNode == null)
            {
                XmlDocument xmlDoc = cfgNode.OwnerDocument;
                valNode = xmlDoc.CreateNode(XmlNodeType.Element, nodeName, xmlDoc.NamespaceURI);
                cfgNode.AppendChild(valNode);
            }
            return valNode;
        }

        public static void SetCfgNodeData(XmlNode cfgNode, string nodeName, object value,
            XmlNodeConversionAttribute xmlnAttr)
        {
            XmlNode valNode = AppendCfgXmlNode(cfgNode, nodeName);
            xmlnAttr.ValueToXmlNode(value, valNode);
        }

        public static void SetExclusiveCfgNodeData(XmlNode cfgNode, object value,
            XmlNodesConversionAttribute xmlnsAttr)
        {
            xmlnsAttr.ValueToXmlNodes(value, cfgNode);
        }

        public static void SetCfgNodeValue(XmlNode cfgNode, string nodeName, object value)
        {
            XmlNode valNode = cfgNode.SelectSingleNode(nodeName);
            if (valNode == null)
            {
                XmlDocument xmlDoc = cfgNode.OwnerDocument;
                valNode = xmlDoc.CreateNode(XmlNodeType.Element, nodeName, xmlDoc.NamespaceURI);
                cfgNode.AppendChild(valNode);
            }

            valNode.InnerText = value?.ToString() ?? string.Empty;
        }

        public static void SetCfgAttributeValue(XmlNode cfgNode, string attributeName, object value)
        {
            XmlAttribute valAttribute = cfgNode.Attributes[attributeName];
            if (valAttribute == null)
            {
                XmlDocument xmlDoc = cfgNode.OwnerDocument;
                valAttribute = xmlDoc.CreateAttribute(attributeName);
                cfgNode.Attributes.Append(valAttribute);
            }

            valAttribute.InnerText = value?.ToString() ?? string.Empty;
        }
    }
    // ReSharper restore PossibleNullReferenceException
}

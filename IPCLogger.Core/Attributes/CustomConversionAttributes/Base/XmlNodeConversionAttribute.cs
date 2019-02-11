using System.Xml;

namespace IPCLogger.Core.Attributes.CustomConversionAttributes.Base
{
    public abstract class XmlNodeConversionAttribute : CustomConversionAttribute
    {
        public sealed override ConversionSource SourceType
        {
            get { return ConversionSource.XmlNode; }
        }

        public abstract object XmlNodeToValue(XmlNode xmlNode);

        public abstract void ValueToXmlNode(object value, XmlNode xmlNode);

        public override string ValueToCSString(object value)
        {
            XmlNode xmlNode = new XmlDocument().CreateElement("_");
            ValueToXmlNode(value, xmlNode);
            return xmlNode.InnerXml;
        }

        public override object CSStringToValue(string sValue)
        {
            XmlNode xmlNode = new XmlDocument().CreateElement("_");
            xmlNode.InnerXml = sValue;
            return XmlNodeToValue(xmlNode);
        }
    }
}

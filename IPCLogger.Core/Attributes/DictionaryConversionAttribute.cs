using IPCLogger.Core.Common;
using System.Collections.Generic;
using System.Xml;

namespace IPCLogger.Core.Attributes
{
    public sealed class DictionaryConversionAttribute : CustomConversionAttribute
    {
        public DictionaryConversionAttribute() 
            : base(ConversionSource.XmlNode)
        {
        }

        public override object XmlNodeToValue(XmlNode node)
        {
            return Helpers.XmlNodeToDictionary(node);
        }

        public override void ValueToXmlNode(object value, XmlNode node)
        {
            Helpers.DictionaryToXmlNode(value as Dictionary<string, string>, node);
        }

        public override string ValueToCSString(object value)
        {
            return Helpers.DictionaryToJson(value as Dictionary<string, string>);
        }
    }
}

using IPCLogger.Core.Common;
using System;
using System.Xml;

namespace IPCLogger.Core.Attributes
{
    public sealed class KeyValueConversionAttribute : CustomConversionAttribute
    {
        private Type _dataType;

        public KeyValueConversionAttribute(Type dataType) 
            : base(ConversionSource.XmlNode)
        {
            _dataType = dataType;
        }

        public override object XmlNodeToValue(XmlNode node)
        {
            return Helpers.XmlNodeToKeyValue(_dataType, node);
        }

        public override void ValueToXmlNode(object value, XmlNode node)
        {
            Helpers.KeyValueToXmlNode(_dataType, value, node);
        }

        public override string ValueToCSString(object value)
        {
            return Helpers.KeyValueToJson(_dataType, value);
        }
    }
}

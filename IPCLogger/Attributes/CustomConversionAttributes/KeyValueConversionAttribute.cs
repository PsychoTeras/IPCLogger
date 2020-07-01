using IPCLogger.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Common;
using System;
using System.Xml;

namespace IPCLogger.Attributes.CustomConversionAttributes
{
    public class KeyValueConversionAttribute : XmlNodeConversionAttribute
    {
        private Type _dataType;
        private string _keyName;
        private string _valueName;

        public KeyValueConversionAttribute(Type dataType, string keyName = "Key", string valueName = "Value")
        {
            _dataType = dataType;
            _keyName = keyName;
            _valueName = valueName;
        }

        public override object XmlNodeToValue(XmlNode xmlNode)
        {
            return Helpers.XmlNodeToKeyValue(_dataType, xmlNode);
        }

        public override void ValueToXmlNode(object value, XmlNode xmlNode)
        {
            Helpers.KeyValueToXmlNode(_dataType, _keyName, _valueName, value, xmlNode);
        }

        public override string ValueToCSString(object value)
        {
            return Helpers.KeyValueToJson(_dataType, _keyName, _valueName, value);
        }

        public override object CSStringToValue(string sValue)
        {
            return Helpers.JsonToKeyValue(_dataType, sValue);
        }
    }  
}

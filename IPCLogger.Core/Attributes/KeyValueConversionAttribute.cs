﻿using IPCLogger.Core.Common;
using System;
using System.Xml;

namespace IPCLogger.Core.Attributes
{
    public sealed class KeyValueConversionAttribute : CustomConversionAttribute
    {
        private Type _dataType;
        private string _keyName;
        private string _valueName;

        public KeyValueConversionAttribute(Type dataType, string keyName = "Key", string valueName = "Value") 
            : base(ConversionSource.XmlNode)
        {
            _dataType = dataType;
            _keyName = keyName;
            _valueName = valueName;
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
            return Helpers.KeyValueToJson(_dataType, _keyName, _valueName, value);
        }
    }
}

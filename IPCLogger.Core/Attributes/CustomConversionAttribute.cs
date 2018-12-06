using System;
using System.Xml;

namespace IPCLogger.Core.Attributes
{
    public enum ConversionSource
    {
        Value,
        XmlNode
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class CustomConversionAttribute : Attribute
    {
        public ConversionSource SourceType { get; }

        public string ExclusiveNodeName { get; }

        protected CustomConversionAttribute(ConversionSource sourceType)
        {
            SourceType = sourceType;
        }

        protected CustomConversionAttribute(ConversionSource sourceType, string exclusiveNodeName)
        {
            SourceType = sourceType;
            ExclusiveNodeName = exclusiveNodeName;
        }

        public virtual object StringToValue(string sValue)
        {
            throw new NotImplementedException($"Method {nameof(StringToValue)} needs to be implemented for source type {SourceType}");
        }

        public virtual string ValueToString(object value)
        {
            throw new NotImplementedException($"Method {nameof(ValueToString)} needs to be implemented for source type {SourceType}");
        }

        public virtual object XmlNodeToValue(XmlNode xmlNode)
        {
            throw new NotImplementedException($"Method {nameof(XmlNodeToValue)} needs to be implemented for source type {SourceType}");
        }

        public virtual void ValueToXmlNode(object value, XmlNode xmlNode)
        {
            throw new NotImplementedException($"Method {nameof(ValueToXmlNode)} needs to be implemented for source type {SourceType}");
        }

        public virtual string ValueToCSString(object value)
        {
            return ValueToString(value);
        }

        public virtual object CSStringToValue(string sValue)
        {
            return StringToValue(sValue);
        }
    }
}

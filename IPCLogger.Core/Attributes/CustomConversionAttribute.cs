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


        protected CustomConversionAttribute(ConversionSource sourceType)
        {
            SourceType = sourceType;
        }

        public virtual object StringToValue(string sValue)
        {
            throw new NotImplementedException();
        }

        public virtual string ValueToString(object value)
        {
            throw new NotImplementedException();
        }

        public virtual object XmlNodeToValue(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public virtual void ValueToXmlNode(object value, XmlNode node)
        {
            throw new NotImplementedException();
        }

        public virtual string ValueToCSString(object value)
        {
            return ValueToString(value);
        }
    }
}

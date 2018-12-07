using System;

namespace IPCLogger.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class CustomConversionAttribute : Attribute
    {
        public enum ConversionSource
        {
            Value,
            XmlNode,
            XmlNodes
        }

        public abstract ConversionSource SourceType { get; }

        public abstract string ValueToCSString(object value);

        public abstract object CSStringToValue(string sValue);
    }
}

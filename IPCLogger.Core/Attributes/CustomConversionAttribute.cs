using System;

namespace IPCLogger.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class CustomConversionAttribute : Attribute
    {
        public abstract object StringToValue(string sValue);

        public abstract string ValueToString(object value);

        public virtual string ValueToCSString(object value)
        {
            return ValueToString(value);
        }
    }
}

using System;

namespace IPCLogger.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class CustomConversionAttribute : Attribute
    {
        public abstract object ConvertValue(string sValue);

        public abstract string UnconvertValue(object value);        
    }
}

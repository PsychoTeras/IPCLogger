using IPCLogger.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Common;
using System;

namespace IPCLogger.Attributes.CustomConversionAttributes
{
    public class TimeSpanStringConversionAttribute : ValueConversionAttribute
    {
        public override object StringToValue(string sValue)
        {
            return Helpers.TimeStringToTimeSpan(sValue);
        }

        public override string ValueToString(object value)
        {
            return value is TimeSpan timeSpan ? Helpers.TimeSpanToTimeString(timeSpan) : string.Empty;
        }

        public override string ValueToCSString(object value)
        {
            return value?.ToString();
        }
    }
}
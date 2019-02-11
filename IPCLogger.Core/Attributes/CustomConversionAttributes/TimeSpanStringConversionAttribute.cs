using System;
using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Attributes.CustomConversionAttributes
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
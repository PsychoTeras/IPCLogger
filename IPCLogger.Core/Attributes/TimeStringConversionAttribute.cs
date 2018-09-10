using IPCLogger.Core.Common;
using System;

namespace IPCLogger.Core.Attributes
{
    public sealed class TimeStringConversionAttribute : CustomConversionAttribute
    {
        public override object ConvertValue(string sValue)
        {
            return Helpers.TimeStringToTimeSpan(sValue);
        }

        public override string UnconvertValue(object value)
        {
            return value is TimeSpan timeSpan ? Helpers.TimeSpanToTimeString(timeSpan) : string.Empty;
        }
    }
}
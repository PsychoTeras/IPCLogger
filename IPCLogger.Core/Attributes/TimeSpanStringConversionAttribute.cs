using IPCLogger.Core.Common;
using System;

namespace IPCLogger.Core.Attributes
{
    public sealed class TimeSpanStringConversionAttribute : CustomConversionAttribute
    {
        public TimeSpanStringConversionAttribute()
            : base(ConversionSource.Value)
        {
        }

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
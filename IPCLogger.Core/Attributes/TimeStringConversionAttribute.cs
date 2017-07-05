using IPCLogger.Core.Common;

namespace IPCLogger.Core.Attributes
{
    public sealed class TimeStringConversionAttribute : CustomConversionAttribute
    {
        public override object ConvertValue(string sValue)
        {
            return Helpers.TimeStringToTimeSpan(sValue);
        }
    }
}

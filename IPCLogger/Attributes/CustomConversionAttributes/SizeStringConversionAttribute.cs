using IPCLogger.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Common;

namespace IPCLogger.Attributes.CustomConversionAttributes
{
    public class SizeStringConversionAttribute : ValueConversionAttribute
    {
        public override object StringToValue(string sValue)
        {
            return Helpers.BytesStringToSize(sValue);
        }

        public override string ValueToString(object value)
        {
            return value is long size ? Helpers.SizeToBytesString(size) : string.Empty;
        }

        public override string ValueToCSString(object value)
        {
            return value?.ToString();
        }
    }
}
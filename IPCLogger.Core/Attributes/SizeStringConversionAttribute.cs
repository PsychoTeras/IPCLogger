using IPCLogger.Core.Common;

namespace IPCLogger.Core.Attributes
{
    public sealed class SizeStringConversionAttribute : CustomConversionAttribute
    {
        public override object ConvertValue(string sValue)
        {
            return Helpers.BytesStringToSize(sValue);
        }

        public override string UnconvertValue(object value)
        {
            return value is long size ? Helpers.SizeToBytesString(size) : string.Empty;
        }
    }
}
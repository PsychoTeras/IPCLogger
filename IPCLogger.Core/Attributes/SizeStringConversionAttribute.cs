using IPCLogger.Core.Common;

namespace IPCLogger.Core.Attributes
{
    public sealed class SizeStringConversionAttribute : CustomConversionAttribute
    {
        public SizeStringConversionAttribute()
            : base(ConversionSource.Value)
        {
        }

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
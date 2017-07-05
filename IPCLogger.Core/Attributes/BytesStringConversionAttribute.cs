using IPCLogger.Core.Common;

namespace IPCLogger.Core.Attributes
{
    public sealed class BytesStringConversionAttribute : CustomConversionAttribute
    {
        public override object ConvertValue(string sValue)
        {
            return Helpers.BytesStringToSize(sValue);
        }
    }
}

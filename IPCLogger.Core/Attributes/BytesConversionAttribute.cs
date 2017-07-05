using System;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Attributes
{
    public class BytesConversionAttribute : CustomConversionAttribute
    {
        public override object ConvertValue(string sValue)
        {
            return Helpers.BytesStringToSize(sValue);
        }
    }
}

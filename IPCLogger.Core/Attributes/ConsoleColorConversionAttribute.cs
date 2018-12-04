using IPCLogger.Core.Common;

namespace IPCLogger.Core.Attributes
{
    public sealed class ConsoleColorConversionAttribute : CustomConversionAttribute
    {
        public ConsoleColorConversionAttribute()
            : base(ConversionSource.XmlNode)
        {
        }
    }
}
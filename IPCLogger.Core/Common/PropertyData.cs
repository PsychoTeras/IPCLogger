using System.Reflection;
using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;

namespace IPCLogger.Core.Common
{
    internal class PropertyData
    {
        public PropertyInfo PropertyInfo { get; }

        public CustomConversionAttribute ConversionAttribute { get; }

        public bool IsRequired { get; }

        public bool IsFormattable { get; }

        public PropertyData(PropertyInfo propertyInfo, CustomConversionAttribute conversionAttribute,
            bool isRequired, bool isFormattable)
        {
            PropertyInfo = propertyInfo;
            ConversionAttribute = conversionAttribute;
            IsRequired = isRequired;
            IsFormattable = isFormattable;
        }
    }
}

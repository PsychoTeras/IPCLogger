using IPCLogger.Attributes.CustomConversionAttributes.Base;
using System.Reflection;

namespace IPCLogger.Common
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

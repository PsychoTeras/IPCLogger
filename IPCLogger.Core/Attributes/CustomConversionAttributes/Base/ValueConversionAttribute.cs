namespace IPCLogger.Core.Attributes.CustomConversionAttributes.Base
{
    public abstract class ValueConversionAttribute : CustomConversionAttribute
    {
        public sealed override ConversionSource SourceType
        {
            get { return ConversionSource.Value; }
        }

        public abstract object StringToValue(string sValue);

        public abstract string ValueToString(object value);

        public override string ValueToCSString(object value)
        {
            return ValueToString(value);
        }

        public override object CSStringToValue(string sValue)
        {
            return StringToValue(sValue);
        }
    }
}

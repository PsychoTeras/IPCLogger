using System;
using IPCLogger.Core.Common;
using System.Collections.Generic;

namespace IPCLogger.Core.Attributes
{
    public sealed class StringListConversionAttribute : CustomConversionAttribute
    {
        private Type _dataType;
        private bool _removeEmpty;
        private string _separator;

        public StringListConversionAttribute(Type dataType, bool removeEmpty = true, string separator = ",")
            : base(ConversionSource.Value)
        {
            _dataType = dataType;
            _removeEmpty = removeEmpty;
            _separator = separator;
        }

        public override object StringToValue(string sValue)
        {
            return Helpers.StringToStringList(_dataType, sValue, _removeEmpty, _separator);
        }

        public override string ValueToString(object value)
        {
            return Helpers.StringListToString(value as IEnumerable<string>, _separator);
        }
    }
}

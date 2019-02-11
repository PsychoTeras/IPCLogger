using System;
using System.Collections.Generic;
using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Attributes.CustomConversionAttributes
{
    public class StringListConversionAttribute : ValueConversionAttribute
    {
        private Type _dataType;
        private bool _removeEmpty;

        public StringListConversionAttribute(Type dataType, bool removeEmpty = true)
        {
            _dataType = dataType;
            _removeEmpty = removeEmpty;
        }

        public override object StringToValue(string sValue)
        {
            return Helpers.StringToStringList(_dataType, sValue, _removeEmpty, Constants.Splitter);
        }

        public override string ValueToString(object value)
        {
            return Helpers.StringListToString(value as IEnumerable<string>, Constants.SplitterString);
        }
    }
}

using System;
using IPCLogger.Core.Common;
using System.Collections.Generic;

namespace IPCLogger.Core.Attributes
{
    public sealed class StringListConversionAttribute : CustomConversionAttribute
    {
        private Type _listType;
        private bool _removeEmpty;
        private string _separator;

        public StringListConversionAttribute(Type listType, bool removeEmpty = true, string separator = ",")
        {
            _listType = listType;
            _removeEmpty = removeEmpty;
            _separator = separator;
        }

        public override object ConvertValue(string sValue)
        {
            return Helpers.StringToStringList(_listType, sValue, _removeEmpty, _separator);
        }

        public override string UnconvertValue(object value)
        {
            return Helpers.StringListToString(value as IEnumerable<string>, _separator);
        }
    }
}

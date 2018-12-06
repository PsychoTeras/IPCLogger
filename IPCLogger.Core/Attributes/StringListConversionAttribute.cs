using System;
using IPCLogger.Core.Common;
using System.Collections.Generic;

namespace IPCLogger.Core.Attributes
{
    public sealed class StringListConversionAttribute : CustomConversionAttribute
    {
        private Type _dataType;
        private bool _removeEmpty;

        public StringListConversionAttribute(Type dataType, bool removeEmpty = true)
            : base(ConversionSource.Value)
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
            return Helpers.StringListToString(value as IEnumerable<string>, Constants.Splitter);
        }
    }
}

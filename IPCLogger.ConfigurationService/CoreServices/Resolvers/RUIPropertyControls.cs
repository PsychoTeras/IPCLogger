using IPCLogger.Attributes.CustomConversionAttributes;
using IPCLogger.ConfigurationService.Common;
using IPCLogger.Loggers.LConsole;
using IPCLogger.Resolvers;
using IPCLogger.Resolvers.Base;
using System;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.CoreServices.Resolvers
{
    public class RUIPropertyControls : BaseResolver
    {

#region Constants

        private const string PROPERTY_STRING = "ui-property-string";
        private const string PROPERTY_BOOL = "ui-property-boolean";
        private const string PROPERTY_ENUM = "ui-property-combobox";
        private const string PROPERTY_NUMERIC = "ui-property-numeric";
        private const string PROPERTY_TIMESPAN = "ui-property-timespan";
        private const string PROPERTY_SIZE = "ui-property-size";
        private const string PROPERTY_TABLE = "ui-property-table";

#endregion

#region Types dictionary

        private static readonly Dictionary<Type, string> _types = new Dictionary<Type, string>
        {
            { typeof(char), PROPERTY_STRING },
            { typeof(string), PROPERTY_STRING },
            { typeof(bool), PROPERTY_BOOL },
            { typeof(Enum), PROPERTY_ENUM },
            { typeof(sbyte), PROPERTY_NUMERIC },
            { typeof(byte), PROPERTY_NUMERIC },
            { typeof(short), PROPERTY_NUMERIC },
            { typeof(ushort), PROPERTY_NUMERIC },
            { typeof(int), PROPERTY_NUMERIC },
            { typeof(uint), PROPERTY_NUMERIC },
            { typeof(long), PROPERTY_NUMERIC },
            { typeof(ulong), PROPERTY_NUMERIC },
            { typeof(TimeSpanStringConversionAttribute), PROPERTY_TIMESPAN },
            { typeof(SizeStringConversionAttribute), PROPERTY_SIZE },
            { typeof(StringListConversionAttribute), PROPERTY_STRING },
            { typeof(KeyValueConversionAttribute), PROPERTY_TABLE },
            { typeof(PatternContentConversionAttribute), PROPERTY_TABLE },
            { typeof(ConsoleHighlightsConversionAttribute), PROPERTY_TABLE }
        };

#endregion

#region Properties

        public override ResolverType Type => ResolverType.UI_PropertyControls;

#endregion

#region Class methods

        public override object Resolve(object key)
        {
            if (!_types.TryGetValue(key as Type, out var controlType))
            {
                throw new Exception($"Unknown property type '{key}'");
            }
            return controlType;
        }

#endregion

    }
}

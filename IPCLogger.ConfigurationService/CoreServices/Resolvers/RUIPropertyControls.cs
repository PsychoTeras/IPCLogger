﻿using IPCLogger.ConfigurationService.Common;
using IPCLogger.Core.Attributes.CustomConversionAttributes;
using IPCLogger.Core.Loggers.LConsole;
using IPCLogger.Core.Resolvers;
using IPCLogger.Core.Resolvers.Base;
using System;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.CoreServices.Resolvers
{
    public class RUIPropertyControls : BaseResolver<ResolverType>
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

#region Class methods

        public RUIPropertyControls() : base(ResolverType.CS_UI_PropertyControl)
        {
        }

        public override object Resolve(object key)
        {
            if (!_types.TryGetValue(key as Type, out var controlType))
            {
                throw new Exception($"Unknown property type '{key}'");
            }
            return controlType;
        }

        public override IEnumerable<T> GetKeys<T>()
        {
            return null;
        }

        public override IEnumerable<T> GetValues<T>()
        {
            return null;
        }

#endregion
    }
}

using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Attributes.CustomConversionAttributes;
using IPCLogger.Core.Loggers.LConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace IPCLogger.ConfigurationService.CoreServices
{
    public static class ControlsService
    {

#region Constants

        private const string BASE_CLASS = "div";
        private const string SEPARATOR_CLASS = "hr";

        private const string PROPERTY_ATTR_NAME = "name";
        private const string PROPERTY_ATTR_VALUE = "value";
        private const string PROPERTY_ATTR_VALUES = "values";
        private const string PROPERTY_ATTR_COMMON = "common";
        private const string PROPERTY_ATTR_REQUIRED = "required";
        private const string PROPERTY_ATTR_FORMATTABLE = "formattable";

        private const string PROPERTY_CONTROL = "form-control";
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
            { typeof(ConsoleHighlightsConversionAttribute), PROPERTY_TABLE }
        };

#endregion

        public static string ControlByPropertyModel(PropertyModel propertyModel)
        {
            Type GetBasePropertyType(Type type)
            {
                if (type.IsEnum)
                {
                    type = typeof(Enum);
                }
                return type;
            }

            Type basePropertyType = GetBasePropertyType(propertyModel.Converter ?? propertyModel.Type);

            if (!_types.TryGetValue(basePropertyType, out var controlType))
            {
                throw new Exception($"Unknown property type '{basePropertyType.Name}'");
            }

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter html = new HtmlTextWriter(stringWriter))
            {
                html.AddAttribute(HtmlTextWriterAttribute.Class, $"{PROPERTY_CONTROL} {controlType}");

                html.AddAttribute(PROPERTY_ATTR_NAME, propertyModel.Name);

                html.AddAttribute(PROPERTY_ATTR_VALUE, propertyModel.Value ?? string.Empty);

                if (!string.IsNullOrEmpty(propertyModel.Values))
                {
                    html.AddAttribute(PROPERTY_ATTR_VALUES, propertyModel.Values);
                }

                if (propertyModel.IsCommon)
                {
                    html.AddAttribute(PROPERTY_ATTR_COMMON, string.Empty);
                }

                if (propertyModel.IsRequired)
                {
                    html.AddAttribute(PROPERTY_ATTR_REQUIRED, string.Empty);
                }

                if (propertyModel.IsFormattable)
                {
                    html.AddAttribute(PROPERTY_ATTR_FORMATTABLE, string.Empty);
                }

                html.RenderBeginTag(BASE_CLASS);

                html.RenderEndTag();

                if (propertyModel.IsCommon && propertyModel.Name == "Enabled")
                {
                    html.RenderBeginTag(SEPARATOR_CLASS);

                    html.RenderEndTag();
                }
            }

            return stringWriter.ToString();
        }
    }
}

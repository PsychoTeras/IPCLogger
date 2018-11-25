using IPCLogger.ConfigurationService.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace IPCLogger.ConfigurationService.CoreServices
{
    public static class ComponentService
    {

#region Constants

        private const string BASE_CLASS = "div";

        private const string PROPERTY_ATTR_VALUE = "value";
        private const string PROPERTY_ATTR_REQUIRED = "required";
        private const string PROPERTY_ATTR_CONVERTER = "converter";

        private const string PROPERTY_CONTROL = "form-control";
        private const string PROPERTY_STRING = "ui-property-string";
        private const string PROPERTY_BOOL = "ui-property-boolean";
        private const string PROPERTY_ENUM = "ui-property-combo";
        private const string PROPERTY_NUMERIC = "ui-property-numeric";
        private const string PROPERTY_TIME = "ui-property-time";

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
            { typeof(TimeSpan), PROPERTY_TIME }
        };

#endregion

        public static string ComponentByPropertyModel(PropertyModel propertyModel)
        {
            if (!_types.TryGetValue(propertyModel.Type, out var controlType))
            {
                throw new Exception($"Unknown property type '{propertyModel.Type.Name}'");
            }

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter html = new HtmlTextWriter(stringWriter))
            {
                html.AddAttribute(HtmlTextWriterAttribute.Class, $"{PROPERTY_CONTROL} {controlType}");

                html.AddAttribute(PROPERTY_ATTR_VALUE, (propertyModel.Value ?? string.Empty).ToString());

                if (propertyModel.IsRequired)
                {
                    html.AddAttribute(PROPERTY_ATTR_REQUIRED, null);
                }

                if (propertyModel.Converter != null)
                {
                    html.AddAttribute(PROPERTY_ATTR_CONVERTER, propertyModel.Converter);
                }

                html.RenderBeginTag(BASE_CLASS);

                html.RenderEndTag();
            }
            return stringWriter.ToString();
        }
    }
}

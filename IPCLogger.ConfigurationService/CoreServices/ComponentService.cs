using IPCLogger.ConfigurationService.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace IPCLogger.ConfigurationService.CoreServices
{
    internal static class ComponentService
    {

#region Constants

        private const string BASE_CLASS = "div";

        private const string PROPERTY_ATTR_VALUE = "value";
        private const string PROPERTY_ATTR_REQUIRED = "required";
        private const string PROPERTY_ATTR_VALIDATOR = "validator";

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
            string controlType;
            if (!_types.TryGetValue(propertyModel.Type, out controlType))
            {
                throw new Exception(string.Format("Unknown property type '{0}'", propertyModel.Type.Name));
            }

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter html = new HtmlTextWriter(stringWriter))
            {
                html.RenderBeginTag(BASE_CLASS);

                html.AddAttribute(HtmlTextWriterAttribute.Class, controlType);

                html.AddAttribute(PROPERTY_ATTR_VALUE, (propertyModel.Value ?? string.Empty).ToString());

                if (propertyModel.IsRequired)
                {
                    html.AddAttribute(PROPERTY_ATTR_REQUIRED, string.Empty);
                }

                if (propertyModel.Extended != null)
                {
                    html.AddAttribute(PROPERTY_ATTR_VALIDATOR, propertyModel.Extended.GetType().FullName);
                }

                html.RenderEndTag();

            }
            return stringWriter.ToString();
        }
    }
}

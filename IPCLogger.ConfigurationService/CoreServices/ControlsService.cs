using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Resolvers;
using System;
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

#endregion

        public static string GetLoggerDescription(string loggerType)
        {
            DocItemModel doc = DocsService.Instance.GetLoggerDoc(loggerType);
            return doc?.Description;
        }

        public static string GetLoggerPropertyDisplayName(string loggerType, PropertyModel propertyModel)
        {
            DocItemModel doc = DocsService.Instance.GetLoggerPropertyDoc(loggerType, propertyModel.Name);
            return doc != null && doc.DisplayName != null ? doc.DisplayName : propertyModel.DisplayName;
        }

        public static string GetLoggerPropertyDescription(string loggerType, PropertyModel propertyModel)
        {
            DocItemModel doc = DocsService.Instance.GetLoggerPropertyDoc(loggerType, propertyModel.Name);
            return doc?.Description;
        }

        public static string GetPatternPropertyDisplayName(PropertyModel propertyModel)
        {
            DocItemModel doc = DocsService.Instance.GetPatternPropertyDoc(propertyModel.Name);
            return doc != null && doc.DisplayName != null ? doc.DisplayName : propertyModel.DisplayName;
        }

        public static string GetPatternPropertyDescription(PropertyModel propertyModel)
        {
            DocItemModel doc = DocsService.Instance.GetPatternPropertyDoc(propertyModel.Name);
            return doc?.Description;
        }

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
            string controlType = RFactory.Get(ResolverType.CS_UI_PropertyControl).AsString(basePropertyType);

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

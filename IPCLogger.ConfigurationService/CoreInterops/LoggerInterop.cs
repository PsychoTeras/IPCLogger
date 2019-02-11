using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace IPCLogger.ConfigurationService.CoreInterops
{
    using CoreHelpers = Core.Common.Helpers;

    // ReSharper disable PossibleNullReferenceException
    internal static class LoggerInterop
    {
        private const string ValidationErrorMessage = "{0} is required";

        public static IEnumerable<PropertyData> GetCommonProperties(this BaseSettings settings)
        {
            return settings.CommonProperties.Values;
        }

        public static IEnumerable<PropertyData> GetProperties(this BaseSettings settings)
        {
            return settings.Properties.Values;
        }

        public static string GetPropertyValue(this BaseSettings settings, PropertyInfo property,
            CustomConversionAttribute converter)
        {
            return converter != null
                ? converter.ValueToCSString(property.GetValue(settings, null)) ?? string.Empty
                : property.GetValue(settings, null)?.ToString() ?? string.Empty;
        }

        public static string GetPropertyValues(PropertyInfo property)
        {
            string[] names;
            Type type = property.PropertyType;
            return type.IsEnum && (names = Enum.GetNames(type)).Any()
                ? names.Aggregate((current, next) => current + "," + next)
                : null;
        }

        public static PropertyValidationResult ValidatePropertyValue(this BaseSettings settings, 
            string propertyName, string sValue, bool isCommon, bool isChanged)
        {
            object ConvertValue(string value, Type type)
            {
                return type.IsEnum
                    ? Enum.Parse(type, value)
                    : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }

            bool IsDefaultValue(object value, Type type)
            {
                object defValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                return type == typeof(string) ? string.IsNullOrEmpty((string)value) : value.Equals(defValue);
            }

            try
            {
                Dictionary<string, PropertyData> dictProps = isCommon
                    ? settings.CommonProperties
                    : settings.Properties;

                if (dictProps.TryGetValue(propertyName, out var data))
                {
                    object value = data.ConversionAttribute != null
                        ? data.ConversionAttribute.CSStringToValue(sValue)
                        : ConvertValue(sValue, data.PropertyInfo.PropertyType);

                    if (value == null || data.IsRequired && IsDefaultValue(value, data.PropertyInfo.PropertyType))
                    {
                        string errorMessage = string.Format(ValidationErrorMessage, propertyName);
                        return PropertyValidationResult.Invalid(propertyName, isCommon, errorMessage);
                    }

                    return PropertyValidationResult.Valid(propertyName, value, isCommon);
                }
            }
            catch (Exception ex)
            {
                return PropertyValidationResult.Invalid(propertyName, isCommon, ex.Message);
            }

            throw new Exception($"Invalid property name '{propertyName}'");
        }

        public static void UpdatePropertyValue(this BaseSettings settings, XmlNode cfgNode, string propertyName, 
            object value, bool isCommon)
        {
            Dictionary<string, PropertyData> dictProps = isCommon
                ? settings.CommonProperties
                : settings.Properties;

            Dictionary<string, string> commonPropertiesNames = settings.CommonPropertiesNames;

            if (dictProps.TryGetValue(propertyName, out var data))
            {
                data.PropertyInfo.SetValue(settings, value, null);

                switch (data.ConversionAttribute)
                {
                    case ValueConversionAttribute vcAttr:
                        value = vcAttr.ValueToString(value);
                        if (isCommon)
                        {
                            string attrName = commonPropertiesNames[propertyName];
                            CoreHelpers.SetCfgAttributeValue(cfgNode, attrName, value);
                        }
                        else
                        {
                            CoreHelpers.SetCfgNodeValue(cfgNode, propertyName, value);
                        }
                        break;
                    case XmlNodeConversionAttribute xmlnAttr:
                        CoreHelpers.SetCfgNodeData(cfgNode, propertyName, value, xmlnAttr);
                        break;
                    case XmlNodesConversionAttribute xmlnsAttr:
                        CoreHelpers.SetExclusiveCfgNodeData(cfgNode, value, xmlnsAttr);
                        break;
                    default:
                        if (isCommon)
                        {
                            string attrName = commonPropertiesNames[propertyName];
                            CoreHelpers.SetCfgAttributeValue(cfgNode, attrName, value);
                        }
                        else
                        {
                            CoreHelpers.SetCfgNodeValue(cfgNode, propertyName, value);
                        }
                        break;
                }
            }
        }

        internal static DeclaredLogger GetDeclaredFactoryLogger(XmlDocument xmlCfg)
        {
            string loggersXPath = $"{Constants.RootAppCfgPath}";
            XmlNode cfgNode = xmlCfg.SelectSingleNode(loggersXPath);
            return cfgNode != null ? new DeclaredLogger(cfgNode) : null;
        }

        internal static DeclaredLogger AppendFactoryLogger(XmlDocument xmlCfg)
        {
            // Append config root node
            XmlNode rootCfgNode = CoreHelpers.AppendCfgXmlNode(xmlCfg.SelectSingleNode("/"), "configuration");

            // Append emtpy logger config handler
            XmlNode rootCSectionsNode = CoreHelpers.AppendCfgXmlNode(rootCfgNode, "configSections");
            XmlNode loggerCSectionNode = CoreHelpers.AppendCfgXmlNode(rootCSectionsNode, "section",
                $"section[@name='{Constants.LoggerName}']");
            CoreHelpers.SetCfgAttributeValue(loggerCSectionNode, "name", Constants.LoggerName);
            CoreHelpers.SetCfgAttributeValue(loggerCSectionNode, "type", "System.Configuration.IgnoreSectionHandler");

            // Create logger section
            XmlNode loggerNode = CoreHelpers.AppendCfgXmlNode(rootCfgNode, Constants.LoggerName);
            CoreHelpers.AppendCfgXmlNode(loggerNode, "Patterns");
            CoreHelpers.AppendCfgXmlNode(loggerNode, "Loggers");

            // Initialize settings
            LFactorySettings settings = new LFactorySettings(typeof(LFactory), null);
            ((dynamic)settings).ApplyCommonSettings(loggerNode);

            // Save XML settings
            CoreHelpers.SetCfgAttributeValue(loggerNode, "enabled", settings.Enabled);
            CoreHelpers.SetCfgAttributeValue(loggerNode, "no-lock", settings.NoLock);
            CoreHelpers.SetCfgAttributeValue(loggerNode, "auto-reload", settings.AutoReload);

            return new DeclaredLogger(loggerNode);
        }

        internal static XmlNode AppendConfigurationNode(XmlDocument xmlCfg, XmlNode cfgNode)
        {
            XmlNode rootNode = xmlCfg.SelectSingleNode(Constants.RootLoggersCfgPath);
            XmlNode newNode = xmlCfg.ImportNode(cfgNode, true);
            rootNode.AppendChild(newNode);
            return newNode;
        }
    }
    // ReSharper restore PossibleNullReferenceException
}

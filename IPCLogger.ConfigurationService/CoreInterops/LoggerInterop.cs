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
    using Common;
    using System.Collections;

    // ReSharper disable PossibleNullReferenceException
    internal static class LoggerInterop
    {
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
                switch (value)
                {
                    case ICollection c:
                        return c.Count == 0;
                    case string s:
                        return string.IsNullOrEmpty(s);
                    default:
                        object defValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                        return value.Equals(defValue);
                }
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

                    if (value == null || 
                        data.IsRequired && IsDefaultValue(value, data.PropertyInfo.PropertyType))
                    {
                        string msg = string.Format($"{propertyName} is required");
                        return PropertyValidationResult.Invalid(propertyName, isCommon, msg);
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

        public static void UpdatePropertyValue(this BaseSettings settings, XmlNode cfgNode, 
            string propertyName, object value, bool isCommon)
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
                            Helpers.SetCfgAttributeValue(cfgNode, attrName, value);
                        }
                        else
                        {
                            Helpers.SetCfgNodeValue(cfgNode, propertyName, value);
                        }
                        break;
                    case XmlNodeConversionAttribute xmlnAttr:
                        Helpers.SetCfgNodeData(cfgNode, propertyName, value, xmlnAttr);
                        break;
                    case XmlNodesConversionAttribute xmlnsAttr:
                        Helpers.SetExclusiveCfgNodeData(cfgNode, value, xmlnsAttr);
                        break;
                    default:
                        if (isCommon)
                        {
                            string attrName = commonPropertiesNames[propertyName];
                            Helpers.SetCfgAttributeValue(cfgNode, attrName, value);
                        }
                        else
                        {
                            Helpers.SetCfgNodeValue(cfgNode, propertyName, value);
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
            XmlNode rootCfgNode = Helpers.AppendCfgXmlNode(xmlCfg.SelectSingleNode("/"), "configuration");

            // Append emtpy logger config handler
            XmlNode rootCSectionsNode = Helpers.AppendCfgXmlNode(rootCfgNode, "configSections");
            XmlNode loggerCSectionNode = Helpers.AppendCfgXmlNode(rootCSectionsNode, "section",
                $"section[@name='{Constants.LoggerName}']");
            Helpers.SetCfgAttributeValue(loggerCSectionNode, "name", Constants.LoggerName);
            Helpers.SetCfgAttributeValue(loggerCSectionNode, "type", "System.Configuration.IgnoreSectionHandler");

            // Create logger section
            XmlNode loggerNode = Helpers.AppendCfgXmlNode(rootCfgNode, Constants.LoggerName);
            Helpers.AppendCfgXmlNode(loggerNode, "Patterns");
            Helpers.AppendCfgXmlNode(loggerNode, "Loggers");

            // Initialize settings
            LFactorySettings settings = new LFactorySettings(typeof(LFactory), null);
            settings.ApplyCommonSettings(loggerNode);

            // Save XML settings
            Helpers.SetCfgAttributeValue(loggerNode, "enabled", settings.Enabled);
            Helpers.SetCfgAttributeValue(loggerNode, "no-lock", settings.NoLock);
            Helpers.SetCfgAttributeValue(loggerNode, "auto-reload", settings.AutoReload);

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

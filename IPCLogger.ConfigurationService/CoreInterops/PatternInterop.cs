using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace IPCLogger.ConfigurationService.CoreInterops
{
    using Common;
    using System.Collections;
    using System.Globalization;

    // ReSharper disable PossibleNullReferenceException
    internal static class PatternInterop
    {
        public const string PROP_CONTENT_NAME = "Content";

        public static string GetPropertyValue(this DeclaredPatternModel model, PropertyInfo property,
            CustomConversionAttribute converter)
        {
            return converter != null
                ? converter.ValueToCSString(property.GetValue(model, null)) ?? string.Empty
                : property.GetValue(model, null)?.ToString() ?? string.Empty;
        }

        public static string GetPropertyValues(PropertyInfo property)
        {
            string[] names;
            Type type = property.PropertyType;
            return type.IsEnum && (names = Enum.GetNames(type)).Any()
                ? names.Aggregate((current, next) => current + "," + next)
                : null;
        }

        public static XmlNode AppendConfigurationNode(XmlDocument xmlCfg, XmlNode cfgNode)
        {
            XmlNode rootNode = xmlCfg.SelectSingleNode(Constants.RootPatternsCfgPath);
            XmlNode newNode = xmlCfg.ImportNode(cfgNode, true);
            rootNode.AppendChild(newNode);
            return newNode;
        }

        public static string GetPropertyAttributeName(string propertyName)
        {
            return PFactory.PropertyAttributes[propertyName];
        }

        public static void UpdatePropertyValue(this DeclaredPatternModel model, XmlNode cfgNode, 
            string propertyName, object value, bool isCommon)
        {
            IEnumerable<PropertyData> dictProps = isCommon
                ? model.CommonPropertyDatas
                : model.PropertyDatas;

            PropertyData data = dictProps.FirstOrDefault(p => p.PropertyInfo.Name == propertyName);

            if (data != null)
            {
                data.PropertyInfo.SetValue(model, value, null);

                switch (data.ConversionAttribute)
                {
                    case ValueConversionAttribute vcAttr:
                        value = vcAttr.ValueToString(value);
                        string attrName = PFactory.PropertyAttributes[propertyName];
                        Helpers.SetCfgAttributeValue(cfgNode, attrName, value);
                        break;
                    case XmlNodeConversionAttribute xmlnAttr:
                        Helpers.SetCfgNodeData(cfgNode, propertyName, value, xmlnAttr);
                        break;
                    case XmlNodesConversionAttribute xmlnsAttr:
                        Helpers.SetExclusiveCfgNodeData(cfgNode, value, xmlnsAttr);
                        break;
                    default:
                        attrName = PFactory.PropertyAttributes[propertyName];
                        Helpers.SetCfgAttributeValue(cfgNode, attrName, value);
                        break;
                }
            }
        }

        public static PropertyValidationResult ValidatePropertyValue(this DeclaredPatternModel model,
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
                IEnumerable<PropertyData> dictProps = isCommon
                    ? model.CommonPropertyDatas
                    : model.PropertyDatas;

                PropertyData data = dictProps.FirstOrDefault(p => p.PropertyInfo.Name == propertyName);

                if (data != null)
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
    }
    // ReSharper restore PossibleNullReferenceException
}

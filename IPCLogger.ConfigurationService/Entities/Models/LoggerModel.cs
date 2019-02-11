using IPCLogger.ConfigurationService.CoreInterops;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Helpers;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class LoggerModel
    {
        private PropertyModel[] _properties;

        protected BaseSettings BaseSettings;

        public string Id { get; protected set; }

        public Type Type { get; protected set; }

        public string TypeName { get; protected set; }

        public string Namespace { get; protected set; }

        public string Description { get; protected set; }

        public IEnumerable<PropertyModel> CommonProperties
        {
            get { return _properties.Where(p => p.IsCommon); }
        }

        public IEnumerable<PropertyModel> Properties
        {
            get { return _properties.Where(p => !p.IsCommon); }
        }

        public bool HasProperties
        {
            get { return _properties.Any(p => !p.IsCommon); }
        }

        public XmlNode RootXmlNode { get; set; }

        public void ReloadProperties()
        {
            PropertyModel PropertyDataToModel(PropertyData data, bool isCommon)
            {
                return new PropertyModel
                (
                    data.PropertyInfo.Name,
                    data.PropertyInfo.PropertyType,
                    data.ConversionAttribute?.GetType(),
                    BaseSettings.GetPropertyValue(data.PropertyInfo, data.ConversionAttribute),
                    LoggerInterop.GetPropertyValues(data.PropertyInfo),
                    isCommon,
                    data.IsRequired,
                    data.IsFormattable
                );
            }

            IEnumerable<PropertyModel> commonProperties = BaseSettings.GetCommonProperties().
                Select(data => PropertyDataToModel(data, true));
            _properties = BaseSettings.GetProperties().
                Select(data => PropertyDataToModel(data, false)).
                Concat(commonProperties).
                ToArray();
        }

        // ReSharper disable PossibleNullReferenceException
        protected void InstLoggerSettings(XmlNode cfgNode)
        {
            bool isNew = cfgNode == null;
            RootXmlNode = cfgNode = cfgNode ?? new XmlDocument().CreateElement(TypeName);
            if (isNew && !string.IsNullOrEmpty(Namespace))
            {
                XmlDocument xmlDoc = cfgNode.OwnerDocument;
                XmlAttribute valAttribute = xmlDoc.CreateAttribute("namespace");
                valAttribute.InnerText = Namespace;
                cfgNode.Attributes.Append(valAttribute);
            }
            Type bsType = ((TypeInfo) Type).ImplementedInterfaces
                .Select(i => i.GenericTypeArguments.FirstOrDefault(gt => gt.IsSubclassOf(typeof(BaseSettings))))
                .First(i => i != null);
            BaseSettings = (BaseSettings) Activator.CreateInstance(bsType, Type, null);
            BaseSettings.Setup(cfgNode);
        }
        // ReSharper restore PossibleNullReferenceException

        protected virtual void RecalculateId()
        {
            Id = BaseHelpers.CalculateMD5($"^{TypeName}%{Namespace}^");
        }

        public void ReinitializeSettings()
        {
            InitializeSettings(RootXmlNode);
        }

        protected void InitializeSettings(XmlNode cfgNode = null)
        {
            InstLoggerSettings(cfgNode);
            RecalculateId();
            ReloadProperties();
        }

        protected void CloneCSProperties(LoggerModel source)
        {
            _properties = new PropertyModel[source._properties.Length];
            Array.Copy(source._properties, _properties, source._properties.Length);
        }

        internal static LoggerModel FromType(Type loggerType)
        {
            LoggerModel model = new LoggerModel();
            model.Type = loggerType;
            model.TypeName = loggerType.Name;
            if (loggerType.Module.Name != "IPCLogger.Core.dll")
            {
                model.Namespace = loggerType.Namespace;
            }
            model.InitializeSettings();
            return model;
        }

        internal PropertyValidationResult[] ValidateProperties(PropertyObjectDTO[] properties)
        {
            return properties.Select(p => BaseSettings.ValidatePropertyValue(p.Name, p.Value, p.IsCommon, p.IsChanged)).ToArray();
        }

        internal bool UpdateSettings(PropertyValidationResult[] validationResult, PropertyObjectDTO[] propertyObjs)
        {
            bool wasUpdated = false, hasError = false;
            string bakRootXmlNode = RootXmlNode.InnerXml;
            foreach (PropertyValidationResult result in validationResult)
            {
                PropertyObjectDTO dtoPropObj = propertyObjs.First(p => p.Name == result.Name);
                if (dtoPropObj.IsChanged)
                {
                    try
                    {
                        BaseSettings.UpdatePropertyValue(RootXmlNode, result.Name, result.Value, result.IsCommon);
                    }
                    catch (Exception ex)
                    {
                        result.SetInvalid(ex.Message);
                        hasError = true;
                    }
                    wasUpdated = true;
                }
            }

            if (hasError)
            {
                RootXmlNode.InnerXml = bakRootXmlNode;
            }

            return wasUpdated && !hasError;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Namespace) ? TypeName : $"{TypeName} [{Namespace}]";
        }
    }
}

using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Helpers;
using IPCLogger.Core.Loggers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using IPCLogger.Core.Attributes;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    using PropertyValidationResult = BaseSettings.PropertyValidationResult;
    using PropertyData = Tuple<PropertyInfo, CustomConversionAttribute, bool>;

    public class LoggerModel
    {
        private BaseSettings _baseSettings;
        public PropertyModel[] _properties;

        public string Id { get; protected set; }

        public Type Type { get; protected set; }

        public string TypeName { get; protected set; }

        public string Namespace { get; protected set; }

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

        public XmlNode RootXmlNode { get; private set; }

        public void ReloadProperties()
        {
            PropertyModel PropertyDataToModel(PropertyData data, bool isCommon)
            {
                return new PropertyModel
                (
                    data.Item1.Name,
                    data.Item1.PropertyType,
                    data.Item2?.GetType(),
                    _baseSettings.GetPropertyValue(data.Item1, data.Item2),
                    _baseSettings.GetPropertyValues(data.Item1),
                    isCommon,
                    data.Item3
                );
            }

            var commonProperties = _baseSettings.GetCommonProperties().
                Select(data => PropertyDataToModel(data, true));
            _properties = _baseSettings.GetProperties().
                Select(data => PropertyDataToModel(data, false)).
                Concat(commonProperties).
                ToArray();
        }

        protected BaseSettings InstLoggerSettings(XmlNode cfgNode)
        {
            RootXmlNode = cfgNode = cfgNode ?? new Func<XmlNode>(() => new XmlDocument().CreateElement("_"))();
            Type bsType = ((TypeInfo) Type).ImplementedInterfaces
                .Select(i => i.GenericTypeArguments.FirstOrDefault(gt => gt.IsSubclassOf(typeof(BaseSettings))))
                .First(i => i != null);
            _baseSettings = (BaseSettings) Activator.CreateInstance(bsType, Type, null);
            _baseSettings.Setup(cfgNode);
            return _baseSettings;
        }

        protected void InitializeSettings(XmlNode cfgNode = null)
        {
            _baseSettings = InstLoggerSettings(cfgNode);
            Id = BaseHelpers.CalculateMD5(_baseSettings.Hash);
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

        internal IEnumerable<PropertyValidationResult> ValidateProperties(PropertyObjectDTO[] properties)
        {
            return properties.Select(p =>_baseSettings.ValidatePropertyValue(p.Name, p.Value, p.IsCommon, p.IsChanged));
        }

        internal bool UpdateSettings(IEnumerable<PropertyValidationResult> validationResult, PropertyObjectDTO[] propertyObjs)
        {
            bool wasUpdated = false;
            foreach (PropertyValidationResult result in validationResult)
            {
                PropertyObjectDTO dtoPropObj = propertyObjs.First(p => p.Name == result.Name);
                if (dtoPropObj.IsChanged)
                {
                    _baseSettings.UpdatePropertyValue(RootXmlNode, result.Name, result.Value, result.IsCommon);
                    wasUpdated = true;
                }
            }
            return wasUpdated;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Namespace) ? TypeName : $"{TypeName} [{Namespace}]";
        }
    }
}

using IPCLogger.ConfigurationService.CoreInterops;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.Core.Attributes;
using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    using Common;
    using Core.Attributes.CustomConversionAttributes.Base;

    public class DeclaredPatternModel
    {
        private PropertyData[] _commonPropertyDatas;
        private PropertyData[] _propertyDatas;
        private PropertyModel[] _properties;

        [NonSetting]
        public string Id { get; set; }

        public string Description { get; set; }

        public string Events { get; set; }

        public bool ImmediateFlush { get; set; }

        [RequiredSetting, PatternContentConversion(typeof(List<KeyValuePair<string, string>>),
            "Applicable for class name (regex is allowed)", "Pattern string")]
        public List<KeyValuePair<string, string>> Content { get; set; }

        [NonSetting]
        public XmlNode RootXmlNode { get; set; }

        public string DisplayContent
        {
            get { return string.Join(", ", Content.Select(kv => kv.Value)); }
        }

        internal IEnumerable<PropertyData> CommonPropertyDatas
        {
            get { return _commonPropertyDatas; }
        }

        internal IEnumerable<PropertyData> PropertyDatas
        {
            get { return _propertyDatas; }
        }

        public IEnumerable<PropertyModel> CommonProperties
        {
            get { return _properties.Where(p => p.IsCommon); }
        }

        public IEnumerable<PropertyModel> Properties
        {
            get { return _properties.Where(p => !p.IsCommon); }
        }

        private DeclaredPatternModel() {}

        public void ReloadProperties()
        {
            PropertyModel PropertyDataToModel(PropertyData data)
            {
                return new PropertyModel
                (
                    data.PropertyInfo.Name,
                    data.PropertyInfo.PropertyType,
                    data.ConversionAttribute?.GetType(),
                    this.GetPropertyValue(data.PropertyInfo, data.ConversionAttribute),
                    PatternInterop.GetPropertyValues(data.PropertyInfo),
                    data.PropertyInfo.Name != PatternInterop.PROP_CONTENT_NAME,
                    data.IsRequired,
                    data.IsFormattable
                );
            }

            IEnumerable<PropertyData> propertyDatas = GetType().GetProperties
            (
                BindingFlags.Public | BindingFlags.Instance
            ).Where
            (
                p => p.CanRead && p.CanWrite && !p.IsDefined<NonSettingAttribute>()
            ).Select
            (
                p => new PropertyData
                (
                    p,
                    p.GetAttribute<CustomConversionAttribute>(),
                    p.IsDefined<RequiredSettingAttribute>(),
                    p.IsDefined<FormattableSettingAttribute>()
                )
            );

            _commonPropertyDatas = propertyDatas.
                Where(p => p.PropertyInfo.Name != PatternInterop.PROP_CONTENT_NAME).
                ToArray();

            _propertyDatas = propertyDatas.
                Where(p => p.PropertyInfo.Name == PatternInterop.PROP_CONTENT_NAME).
                ToArray();

            _properties = propertyDatas.Select(PropertyDataToModel).ToArray();
        }

        public void ReloadContent()
        {
            Content = PFactory.GetPatternContent(RootXmlNode).
                Select(c => new KeyValuePair<string, string>(c.ApplicableFor, c.Content)).
                ToList();
        }

        private void RecalculateId()
        {
            int uniqueId = 0;
            XmlNode prewNode = RootXmlNode;
            do
            {
                uniqueId += prewNode.InnerXml.Length;
                prewNode = prewNode.PreviousSibling;
            } while (prewNode != null);
            Id = Helpers.CalculateMD5(uniqueId.ToString());
        }

        private void InitializeSettings()
        {
            RecalculateId();
            ReloadContent();
            ReloadProperties();
        }

        public void ReinitializeSettings()
        {
            InitializeSettings();
        }

        internal PropertyValidationResult[] ValidateProperties(PropertyObjectDTO[] properties)
        {
            return properties.Select(p => this.ValidatePropertyValue(p.Name, p.Value, p.IsCommon, p.IsChanged)).ToArray();
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
                        this.UpdatePropertyValue(RootXmlNode, result.Name, result.Value, result.IsCommon);
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

        internal static DeclaredPatternModel FromDeclaredPattern(DeclaredPattern source)
        {
            DeclaredPatternModel model = new DeclaredPatternModel();
            model.Description = source.Description;
            model.Events = source.Events;
            model.ImmediateFlush = source.ImmediateFlush;
            model.RootXmlNode = source.CfgNode;
            model.InitializeSettings();
            return model;
        }

        internal static DeclaredPatternModel CreateNew()
        {
            DeclaredPatternModel model = new DeclaredPatternModel();
            model.RootXmlNode = new XmlDocument().CreateElement("Pattern");
            model.InitializeSettings();
            return model;
        }

        public override string ToString()
        {
            return Description ?? "[Pattern]";
        }
    }
}

using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Helpers;
using IPCLogger.Core.Attributes;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    using PropertyData = BaseSettings.PropertyData;
    using PropertyValidationResult = BaseSettings.PropertyValidationResult;

    public class DeclaredPatternModel
    {
        private PropertyModel[] _properties;

        [NonSetting]
        public string Id { get; protected set; }

        public string Description { get; protected set; }

        public string Events { get; protected set; }

        public bool ImmediateFlush { get; protected set; }

        [NonSetting]
        public XmlNode RootXmlNode { get; set; }

        [NonSetting]
        public IEnumerable<PropertyModel> Properties
        {
            get { return _properties; }
        }

        private DeclaredPatternModel()
        {
        }

        public void ReloadProperties()
        {
            PropertyModel PropertyDataToModel(PropertyData data, bool isCommon)
            {
                return new PropertyModel
                (
                    data.PropertyInfo.Name,
                    data.PropertyInfo.PropertyType,
                    data.PropertyInfo.GetValue(this, null)?.ToString() ?? string.Empty
                );
            }

            _properties = GetType().GetProperties
            (
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance
            ).Where
            (
                p => p.CanRead && p.CanWrite && !p.IsDefined<NonSettingAttribute>()
            ).Select
            (
                p => new PropertyData(p)
            ).Select(data => PropertyDataToModel(data, false)).ToArray();
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
            Id = BaseHelpers.CalculateMD5(uniqueId.ToString());
        }

        private void InitializeSettings()
        {
            RecalculateId();
            ReloadProperties();
        }

        internal PropertyValidationResult[] ValidateProperties(PropertyObjectDTO[] properties)
        {
            return null;
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
                        //BaseSettings.UpdatePropertyValue(RootXmlNode, result.Name, result.Value, result.IsCommon);
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

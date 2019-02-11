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

    public class PatternContentModel
    {
        public string ApplicableFor { get; set; }

        public string Content { get; set; }

        internal static PatternContentModel FromPatternContent(PatternContent source)
        {
            return new PatternContentModel
            {
                ApplicableFor = source.ApplicableFor,
                Content = source.Content
            };
        }
    }

    public class DeclaredPatternModel
    {
        private PropertyModel[] _properties;
        private List<PatternContentModel> _content;

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

        public IEnumerable<PatternContentModel> Content
        {
            get { return _content; }
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
                    data.PropertyInfo.GetValue(this, null)?.ToString() ?? string.Empty
                );
            }

            _properties = GetType().GetProperties
            (
                BindingFlags.Public | BindingFlags.Instance
            ).Where
            (
                p => p.CanRead && p.CanWrite && !p.IsDefined<NonSettingAttribute>()
            ).Select
            (
                p => new PropertyData(p)
            ).Select(PropertyDataToModel).ToArray();
        }

        public void ReloadContent()
        {
            _content = PFactory.GetPatternContent(RootXmlNode).
                Select(PatternContentModel.FromPatternContent).
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
            ReloadProperties();
            ReloadContent();
        }

        public void ReinitializeSettings()
        {
            InitializeSettings();
        }

        internal PropertyValidationResult[] ValidateProperties(PropertyObjectDTO[] properties)
        {
            List<PropertyValidationResult> result = new List<PropertyValidationResult>();
            PropertyInfo[] propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
            foreach (PropertyObjectDTO dto in properties)
            {
                if (dto.Name != "Content")
                {
                    PropertyInfo pi = propertyInfos.FirstOrDefault(p => p.Name == dto.Name);
                    if (pi == null)
                    {
                        throw new Exception($"Invlid property name '{dto.Name}'");
                    }

                    result.Add(PropertyValidationResult.Valid
                    (
                        dto.Name,
                        Convert.ChangeType(dto.Value, pi.PropertyType),
                        false
                    ));
                }
            }

            return result.ToArray();
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
                        if (result.Name != "Content")
                        {
                            string attributeName = PatternInterop.GetPropertyAttributeName(result.Name);
                            Helpers.SetCfgAttributeValue(RootXmlNode, attributeName, result.Value);
                        }
                        else
                        {

                        }
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

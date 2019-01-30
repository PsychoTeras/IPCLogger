using IPCLogger.Core.Attributes;
using IPCLogger.Core.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using IPCLogger.Core.Attributes.CustomConversionAttributes;
using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;

namespace IPCLogger.Core.Loggers.Base
{
    using PropertyData = Tuple<PropertyInfo, CustomConversionAttribute, bool>;

    // ReSharper disable PossibleNullReferenceException
    public abstract class BaseSettings
    {

#region Definitions

        protected internal class PropertyValidationResult
        {
            public string Name;
            public object Value;
            public bool IsCommon;
            public bool IsValid;
            public string ErrorMessage;

            public void SetInvalid(string errorMessage)
            {
                Value = null;
                IsValid = false;
                ErrorMessage = errorMessage;
            }

            public static PropertyValidationResult Valid(string name, object value, bool isCommon)
            {
                return new PropertyValidationResult
                {
                    Name = name,
                    Value = value,
                    IsCommon = isCommon,
                    IsValid = true
                };
            }

            public static PropertyValidationResult Invalid(string name, bool isCommon, string errorMessage)
            {
                return new PropertyValidationResult
                {
                    Name = name,
                    IsCommon = isCommon,
                    ErrorMessage = errorMessage
                };
            }
        }

#endregion

#region Constants

        protected const string RootLoggersCfgPath = Constants.RootLoggerCfgPath + "/Loggers";
        protected const string ValidationErrorMessage = "{0} is required";

        private static readonly Func<string, bool> _defCheckApplicableEvent = s => true;

#endregion

#region Private fields

        private Type _loggerType;
        private Action _onApplyChanges;
        private HashSet<string> _allowEvents;
        private HashSet<string> _denyEvents;

        private Dictionary<string, PropertyData> _commonProperties;
        private Dictionary<string, PropertyData> _properties;
        private HashSet<string> _exclusivePropertyNodeNames;

#endregion

#region Protected internal fields

        protected internal Func<string, bool> CheckApplicableEvent = _defCheckApplicableEvent;

#endregion

#region Properties

        [NonSetting]
        public bool Enabled { get; protected set; }

        [NonSetting]
        public string Name { get; set; }

        [NonSetting]
        public byte[] Hash { get; protected set; }

        [NonSetting, StringListConversion(typeof(HashSet<string>))]
        public HashSet<string> AllowEvents
        {
            get { return _allowEvents; }
            protected set { _allowEvents = value; }
        }

        [NonSetting, StringListConversion(typeof(HashSet<string>))]
        public HashSet<string> DenyEvents
        {
            get { return _denyEvents; }
            protected set { _denyEvents = value; }
        }

#endregion

#region Ctor

        protected BaseSettings(Type loggerType, Action onApplyChanges)
        {
            _onApplyChanges = onApplyChanges;
            _loggerType = loggerType;
        }

#endregion

#region Public methods

        public void Setup()
        {
            Setup(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        public void Setup(string configurationFile, string loggerName = null)
        {
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                string msg = $"Configuration file '{configurationFile}' is invalid or doesn't exist";
                throw new ArgumentException(msg);
            }

            XmlDocument xmlCfg = new XmlDocument();
            xmlCfg.Load(configurationFile);
            Setup(xmlCfg, loggerName);
        }

        public void Setup(XmlDocument xmlCfg, string loggerName = null)
        {
            XmlNode cfgNode = GetLoggerSettingsNode(xmlCfg, loggerName);
            if (cfgNode == null)
            {
                string msg = $"Settings for logger '{_loggerType.Name}', name '{loggerName}' not found";
                throw new Exception(msg);
            }

            Setup(cfgNode);
        }

        public void Setup(XmlNode cfgNode)
        {
            byte[] newHash = CalculateHash(cfgNode);
            if (newHash != null && !Helpers.ByteArrayEquals(newHash, Hash)) //Setup if changed
            {
                BeginSetup();

                ApplyCommonSettings(cfgNode);

                Dictionary<string, XmlNode> settingsDict = GetSettingsDictionary(cfgNode);

                Dictionary<string, object> valuesDict = GetSettingsValues(settingsDict);

                Dictionary<string, object> exclusiveValuesDict = GetExclusiveSettingsValues(cfgNode);

                Dictionary<string, object> mergedValuesDict = MergeDictionaries(valuesDict, exclusiveValuesDict);

                VerifySettingsValues(mergedValuesDict);

                ApplySettingsValues(mergedValuesDict);

                RecalculateHash(cfgNode);

                FinalizeSetup();

                ApplyChanges();
            }
        }

#endregion

#region Initialization methods

        protected virtual Dictionary<string, string> GetCommonPropertiesSet()
        {
            return new Dictionary<string, string>
            {
                { "Enabled", "enabled" },
                { "Name", "name" },
                { "AllowEvents", "allow-events" },
                { "DenyEvents", "deny-events" }
            };
        }

        protected virtual void BeginSetup()
        {
            Dictionary<string, string> commonPropertiesSet = GetCommonPropertiesSet();

            _commonProperties = GetType().
                GetProperties
                (
                    BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance
                ).Where
                (
                    p => commonPropertiesSet.ContainsKey(p.Name)
                ).OrderByDescending
                (
                    p => p.DeclaringType.MetadataToken
                ).ToDictionary
                (
                    p => p.Name,
                    p => new PropertyData
                    (
                        p,
                        p.GetAttribute<CustomConversionAttribute>(),
                        p.IsDefined<RequiredSettingAttribute>()
                    )
                );

            _properties = GetType().
                GetProperties
                (
                    BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
                ).Where
                (
                    p => p.CanRead && p.CanWrite && !p.IsDefined<NonSettingAttribute>()
                ).ToDictionary
                (
                    p => p.Name,
                    p => new PropertyData
                    (
                        p,
                        p.GetAttribute<CustomConversionAttribute>(),
                        p.IsDefined<RequiredSettingAttribute>()
                    )
                );

            _exclusivePropertyNodeNames = new HashSet<string>
            (
                _properties.Concat(_commonProperties).
                    Select(d => d.Value.Item2).
                    OfType<XmlNodesConversionAttribute>().
                    SelectMany(p => p.ExclusiveNodeNames).
                    Distinct()
            );
        }

        protected virtual string GetLoggerSettingsNodeName(string loggerName = null)
        {
            loggerName = !string.IsNullOrEmpty(loggerName) ? $"[@name='{loggerName}']" : string.Empty;
            return $"{RootLoggersCfgPath}/{_loggerType.Name}{loggerName}";
        }

        protected virtual XmlNode GetLoggerSettingsNode(XmlDocument xmlCfg, string loggerName = null)
        {
            string loggerXPath = GetLoggerSettingsNodeName(loggerName);
            return xmlCfg.SelectSingleNode(loggerXPath);
        }

        protected virtual void ApplyCommonSettings(XmlNode cfgNode)
        {
            XmlAttribute aEnabled = cfgNode.Attributes["enabled"];
            Enabled = aEnabled == null || !bool.TryParse(aEnabled.Value, out var enabled) || enabled;

            XmlAttribute aName = cfgNode.Attributes["name"];
            Name = aName?.InnerText.Trim();

            LoadEventsApplicableSet(cfgNode, "allow-events", out _allowEvents);
            LoadEventsApplicableSet(cfgNode, "deny-events", out _denyEvents);

            if (_allowEvents != null)
            {
                CheckApplicableEvent = s => _allowEvents.Contains(s);
            }
            else if (_denyEvents != null)
            {
                CheckApplicableEvent = s => !_denyEvents.Contains(s);
            }
            else
            {
                CheckApplicableEvent = _defCheckApplicableEvent;
            }
        }

        protected virtual Dictionary<string, XmlNode> GetSettingsDictionary(XmlNode cfgNode)
        {
            return GetSettingsDictionary(cfgNode, null);
        }

        protected virtual Dictionary<string, XmlNode> GetSettingsDictionary(XmlNode cfgNode, ICollection<string> excludes)
        {
            Dictionary<string, XmlNode> settingsDict = new Dictionary<string, XmlNode>();

            IEnumerable<XmlNode> nodes = cfgNode.
                OfType<XmlNode>().
                Where(n => n.NodeType != XmlNodeType.Comment && n.NodeType != XmlNodeType.Whitespace);
            if (excludes != null)
            {
                nodes = nodes.Where(n => !excludes.Contains(n.Name, StringComparer.InvariantCultureIgnoreCase));
            }

            foreach (XmlNode settingNode in nodes)
            {
                if (_exclusivePropertyNodeNames.Contains(settingNode.Name))
                {
                    continue;
                }

                if (!_properties.ContainsKey(settingNode.Name))
                {
                    string msg = $"Undefined settings node '{settingNode.Name}'";
                    throw new Exception(msg);
                }

                if (settingsDict.ContainsKey(settingNode.Name))
                {
                    string msg = $"Duplicated settings definition '{settingNode.Name}'";
                    throw new Exception(msg);
                }

                settingsDict.Add(settingNode.Name, settingNode);
            }

            return settingsDict;
        }

        protected virtual Dictionary<string, object> GetSettingsValues(Dictionary<string, XmlNode> settingsDict)
        {
            Dictionary<string, object> valuesDict = new Dictionary<string, object>(settingsDict.Count);
            foreach (KeyValuePair<string, XmlNode> setting in settingsDict)
            {
                PropertyData data = _properties[setting.Key];
                Type propertyType = data.Item1.PropertyType;

                try
                {
                    object value;
                    switch (data.Item2)
                    {
                        case ValueConversionAttribute vcAttr:
                            value = vcAttr.StringToValue(setting.Value.InnerText.Trim());
                            break;
                        case XmlNodeConversionAttribute xmlnAttr:
                            value = xmlnAttr.XmlNodeToValue(setting.Value);
                            break;
                        default:
                            string sValue = setting.Value.InnerText.Trim();
                            if (propertyType.IsEnum)
                            {
                                try
                                {
                                    value = Enum.Parse(propertyType, sValue);
                                }
                                catch
                                {
                                    string names = string.Join(", ", Enum.GetNames(propertyType));
                                    string msg = $"Possible values: {names}";
                                    throw new Exception(msg);
                                }
                            }
                            else
                            {
                                value = Convert.ChangeType(sValue, propertyType, CultureInfo.InvariantCulture);
                            }
                            break;
                    }

                    if (value == null)
                    {
                        throw new Exception();
                    }

                    valuesDict.Add(setting.Key, value);
                }
                catch (Exception ex)
                {
                    string msg = $"Invalid setting value '{setting.Value}' for setting '{setting.Key}' type '{propertyType.Name}'";
                    if (!string.IsNullOrEmpty(ex.Message))
                    {
                        msg += $". {ex.Message}";
                    }
                    throw new Exception(msg);
                }
            }
            return valuesDict;
        }

        protected virtual Dictionary<string, object> GetExclusiveSettingsValues(XmlNode cfgNode)
        {
            Dictionary<string, object> valuesDict = new Dictionary<string, object>();
            IEnumerable<PropertyData> exclusiveProperties = _properties.Values.
                Where(p => p.Item2 is XmlNodesConversionAttribute);

            foreach (PropertyData data in exclusiveProperties)
            {
                string propertyName = data.Item1.Name;
                Type propertyType = data.Item1.PropertyType;
                XmlNodesConversionAttribute xmlnsAttr = data.Item2 as XmlNodesConversionAttribute;

                try
                {
                    object value = xmlnsAttr.XmlNodesToValue(cfgNode);
                    if (value == null)
                    {
                        throw new Exception();
                    }

                    valuesDict.Add(data.Item1.Name, value);
                }
                catch (Exception ex)
                {
                    string msg = $"Invalid exclusive setting value for setting '{propertyName}' type '{propertyType.Name}'";
                    if (!string.IsNullOrEmpty(ex.Message))
                    {
                        msg += $". {ex.Message}";
                    }
                    throw new Exception(msg);
                }
            }

            return valuesDict;
        }

        protected virtual void ApplySettingsValues(Dictionary<string, object> valuesDict)
        {
            foreach (KeyValuePair<string, object> value in valuesDict)
            {
                PropertyInfo property = _properties[value.Key].Item1;
                property.SetValue(this, value.Value, null);
            }
        }

        protected virtual void VerifySettingsValues(Dictionary<string, object> valuesDict) { }

        protected virtual void FinalizeSetup() { }

        protected virtual void Save(XmlNode cfgNode)
        {
            foreach (PropertyData data in _properties.Values)
            {
                PropertyInfo property = data.Item1;
                object value = property.GetValue(this, null);

                switch (data.Item2)
                {
                    case ValueConversionAttribute vcAttr:
                        value = vcAttr.ValueToString(value);
                        SetCfgNodeValue(cfgNode, property.Name, value);
                        break;
                    case XmlNodeConversionAttribute xmlnAttr:
                        SetCfgNodeData(cfgNode, property.Name, value, xmlnAttr);
                        break;
                    case XmlNodesConversionAttribute xmlnsAttr:
                        SetExclusiveCfgNodeData(cfgNode, value, xmlnsAttr);
                        break;
                    default:
                        SetCfgNodeValue(cfgNode, property.Name, value);
                        break;
                }
            }
        }

#endregion

#region Configuration Service methods

        protected internal virtual IEnumerable<PropertyData> GetCommonProperties()
        {
            return _commonProperties.Values;
        }

        protected internal virtual IEnumerable<PropertyData> GetProperties()
        {
            return _properties.Values;
        }

        protected internal virtual string GetPropertyValue(PropertyInfo property, CustomConversionAttribute converter)
        {
            return converter != null
                ? converter.ValueToCSString(property.GetValue(this, null)) ?? string.Empty
                : property.GetValue(this, null)?.ToString() ?? string.Empty;
        }

        protected internal virtual string GetPropertyValues(PropertyInfo property)
        {
            string[] names;
            Type type = property.PropertyType;
            return type.IsEnum && (names = Enum.GetNames(type)).Any()
                ? names.Aggregate((current, next) => current + "," + next)
                : null;
        }

        protected internal virtual PropertyValidationResult ValidatePropertyValue(string propertyName, string sValue,
            bool isCommon, bool isChanged)
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
                return type == typeof(string) ? string.IsNullOrEmpty((string) value) : value.Equals(defValue);
            }

            try
            {
                Dictionary<string, PropertyData> dictProps = isCommon
                    ? _commonProperties
                    : _properties;

                if (dictProps.TryGetValue(propertyName, out var data))
                {
                    object value = data.Item2 != null
                        ? data.Item2.CSStringToValue(sValue)
                        : ConvertValue(sValue, data.Item1.PropertyType);

                    if (value == null || data.Item3 && IsDefaultValue(value, data.Item1.PropertyType))
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

        protected internal virtual void UpdatePropertyValue(XmlNode cfgNode, string propertyName, object value,
            bool isCommon)
        {
            Dictionary<string, PropertyData> dictProps = isCommon
                ? _commonProperties
                : _properties;

            Dictionary<string, string> commonPropertiesSet = GetCommonPropertiesSet();

            if (dictProps.TryGetValue(propertyName, out var data))
            {
                data.Item1.SetValue(this, value, null);

                switch (data.Item2)
                {
                    case ValueConversionAttribute vcAttr:
                        value = vcAttr.ValueToString(value);
                        if (isCommon)
                        {
                            string attrName = commonPropertiesSet[propertyName];
                            SetCfgAttributeValue(cfgNode, attrName, value);
                        }
                        else
                        {
                            SetCfgNodeValue(cfgNode, propertyName, value);
                        }
                        break;
                    case XmlNodeConversionAttribute xmlnAttr:
                        SetCfgNodeData(cfgNode, propertyName, value, xmlnAttr);
                        break;
                    case XmlNodesConversionAttribute xmlnsAttr:
                        SetExclusiveCfgNodeData(cfgNode, value, xmlnsAttr);
                        break;
                    default:
                        if (isCommon)
                        {
                            string attrName = commonPropertiesSet[propertyName];
                            SetCfgAttributeValue(cfgNode, attrName, value);
                        }
                        else
                        {
                            SetCfgNodeValue(cfgNode, propertyName, value);
                        }
                        break;
                }
            }
        }

#endregion

#region Private methods

        private void ApplyChanges()
        {
            _onApplyChanges?.Invoke();
        }

        private Dictionary<TK, TV> MergeDictionaries<TK, TV>(Dictionary<TK, TV> dict1, Dictionary<TK, TV> dict2)
        {
            return dict1.Union(dict2).ToDictionary(k => k.Key, v => v.Value);
        }

        private void LoadEventsApplicableSet(XmlNode cfgNode, string attributeName, out HashSet<string> set)
        {
            set = null;
            XmlAttribute aAllowEvents = cfgNode.Attributes[attributeName];
            if (aAllowEvents != null)
            {
                string[] allowedEvents = aAllowEvents.InnerText.Split
                    (
                        new[] {Constants.Splitter}, StringSplitOptions.RemoveEmptyEntries
                    );
                set = new HashSet<string>
                    (
                        allowedEvents.Select(s => s.Trim()).Where(s => s != string.Empty).Distinct()
                    );
                if (set.Count == 0)
                {
                    set = null;
                }
            }
        }

        private void RecalculateHash(XmlNode cfgNode)
        {
            Hash = CalculateHash(cfgNode);
        }

#endregion

#region Helpers

        protected byte[] CalculateHash(XmlNode cfgNode)
        {
            byte[] bXmlData = Encoding.ASCII.GetBytes(cfgNode.OuterXml);
            return new MD5CryptoServiceProvider().ComputeHash(bXmlData);
        }

        private XmlNode AppendCfgXmlNode(XmlNode cfgNode, string nodeName)
        {
            XmlNode valNode = cfgNode.SelectSingleNode(nodeName);
            if (valNode == null)
            {
                XmlDocument xmlDoc = cfgNode.OwnerDocument;
                valNode = xmlDoc.CreateNode(XmlNodeType.Element, nodeName, xmlDoc.NamespaceURI);
                cfgNode.AppendChild(valNode);
            }
            return valNode;
        }

        protected void SetCfgNodeData(XmlNode cfgNode, string nodeName, object value,
            XmlNodeConversionAttribute xmlnAttr)
        {
            XmlNode valNode = AppendCfgXmlNode(cfgNode, nodeName);
            xmlnAttr.ValueToXmlNode(value, valNode);
        }

        protected void SetExclusiveCfgNodeData(XmlNode cfgNode, object value,
            XmlNodesConversionAttribute xmlnsAttr)
        {
            xmlnsAttr.ValueToXmlNodes(value, cfgNode);
        }

        protected void SetCfgNodeValue(XmlNode cfgNode, string nodeName, object value)
        {
            XmlNode valNode = cfgNode.SelectSingleNode(nodeName);
            if (valNode == null)
            {
                XmlDocument xmlDoc = cfgNode.OwnerDocument;
                valNode = xmlDoc.CreateNode(XmlNodeType.Element, nodeName, xmlDoc.NamespaceURI);
                cfgNode.AppendChild(valNode);
            }

            valNode.InnerText = value?.ToString() ?? string.Empty;
        }

        protected void SetCfgAttributeValue(XmlNode cfgNode, string attributeName, object value)
        {
            XmlAttribute valAttribute = cfgNode.Attributes[attributeName];
            if (valAttribute == null)
            {
                XmlDocument xmlDoc = cfgNode.OwnerDocument;
                valAttribute = xmlDoc.CreateAttribute(attributeName);
                cfgNode.Attributes.Append(valAttribute);
            }

            valAttribute.InnerText = value?.ToString() ?? string.Empty;
        }

#endregion

    }

    // ReSharper restore PossibleNullReferenceException
}

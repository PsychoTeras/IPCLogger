using IPCLogger.Core.Attributes;
using IPCLogger.Core.Attributes.CustomConversionAttributes;
using IPCLogger.Core.Attributes.CustomConversionAttributes.Base;
using IPCLogger.Core.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace IPCLogger.Core.Loggers.Base
{
    // ReSharper disable PossibleNullReferenceException
    public abstract class BaseSettings
    {

#region Constants

        private static readonly Func<string, bool> _defCheckApplicableEvent = s => true;

#endregion

#region Private fields

        private Type _loggerType;
        private Action _onApplyChanges;
        private HashSet<string> _allowEvents;
        private HashSet<string> _denyEvents;

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

        internal Dictionary<string, PropertyData> CommonProperties { get; private set; }

        internal Dictionary<string, PropertyData> Properties { get; private set; }

        internal Dictionary<string, string> CommonPropertiesNames
        {
            get { return GetCommonPropertiesNames(); }
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
            byte[] newHash = Helpers.CalculateHash(cfgNode);
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

        protected virtual Dictionary<string, string> GetCommonPropertiesNames()
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
            Dictionary<string, string> commonPropertiesNames = GetCommonPropertiesNames();

            CommonProperties = GetType().
                GetProperties
                (
                    BindingFlags.Public | BindingFlags.Instance
                ).Where
                (
                    p => commonPropertiesNames.ContainsKey(p.Name)
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
                        p.IsDefined<RequiredSettingAttribute>(),
                        p.IsDefined<FormattableSettingAttribute>()
                    )
                );

            Properties = GetType().
                GetProperties
                (
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
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
                        p.IsDefined<RequiredSettingAttribute>(),
                        p.IsDefined<FormattableSettingAttribute>()
                    )
                );

            _exclusivePropertyNodeNames = new HashSet<string>
            (
                Properties.Concat(CommonProperties).
                    Select(d => d.Value.ConversionAttribute).
                    OfType<XmlNodesConversionAttribute>().
                    SelectMany(p => p.ExclusiveNodeNames).
                    Distinct()
            );
        }

        protected virtual string GetLoggerSettingsNodeName(string loggerName = null)
        {
            loggerName = !string.IsNullOrEmpty(loggerName) ? $"[@name='{loggerName}']" : string.Empty;
            return $"{Constants.RootLoggersCfgPath}/{_loggerType.Name}{loggerName}";
        }

        protected virtual XmlNode GetLoggerSettingsNode(XmlDocument xmlCfg, string loggerName = null)
        {
            string loggerXPath = GetLoggerSettingsNodeName(loggerName);
            return xmlCfg.SelectSingleNode(loggerXPath);
        }

        protected internal virtual void ApplyCommonSettings(XmlNode cfgNode)
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

                if (!Properties.ContainsKey(settingNode.Name))
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
                PropertyData data = Properties[setting.Key];
                Type propertyType = data.PropertyInfo.PropertyType;

                try
                {
                    object value;
                    switch (data.ConversionAttribute)
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
            IEnumerable<PropertyData> exclusiveProperties = Properties.Values.
                Where(p => p.ConversionAttribute is XmlNodesConversionAttribute);

            foreach (PropertyData data in exclusiveProperties)
            {
                string propertyName = data.PropertyInfo.Name;
                Type propertyType = data.PropertyInfo.PropertyType;
                XmlNodesConversionAttribute xmlnsAttr = data.ConversionAttribute as XmlNodesConversionAttribute;

                try
                {
                    object value = xmlnsAttr.XmlNodesToValue(cfgNode);
                    if (value == null)
                    {
                        throw new Exception();
                    }

                    valuesDict.Add(data.PropertyInfo.Name, value);
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
                PropertyInfo property = Properties[value.Key].PropertyInfo;
                property.SetValue(this, value.Value, null);
            }
        }

        protected virtual void VerifySettingsValues(Dictionary<string, object> valuesDict) { }

        protected virtual void FinalizeSetup() { }

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
            Hash = Helpers.CalculateHash(cfgNode);
        }

#endregion

    }
    // ReSharper restore PossibleNullReferenceException
}

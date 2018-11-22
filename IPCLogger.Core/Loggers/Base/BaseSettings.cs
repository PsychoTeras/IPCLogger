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

namespace IPCLogger.Core.Loggers.Base
{
    // ReSharper disable PossibleNullReferenceException
    public abstract class BaseSettings
    {

#region Constants

        private static readonly Func<string, bool> _defCheckApplicableEvent = s => true;
        protected const string RootLoggersCfgPath = Constants.RootLoggerCfgPath + "/Loggers";

#endregion

#region Private fields

        private Type _loggerType;
        private Action _onApplyChanges;
        private HashSet<string> _allowEvents;
        private HashSet<string> _denyEvents;

#endregion

#region Protected fields

        protected byte[] SettingsHash;

#endregion

#region Internal fields

        internal Func<string, bool> CheckApplicableEvent = _defCheckApplicableEvent;
        internal Dictionary<string, KeyValuePair<PropertyInfo, CustomConversionAttribute>> Properties;

#endregion

#region Properties

        [NonSetting]
        public string Name { get; set; }

        [NonSetting]
        public HashSet<string> AllowEvents
        {
            get { return _allowEvents; }
        }

        [NonSetting]
        public HashSet<string> DenyEvents
        {
            get { return _denyEvents; }
        }

#endregion

#region Ctor

        protected BaseSettings(Type loggerType, Action onApplyChanges)
        {
            _onApplyChanges = onApplyChanges;
            _loggerType = loggerType;
            Properties = GetType().
                GetProperties
                (
                    BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
                ).Where
                (
                    p => p.CanRead && p.CanWrite && p.GetCustomAttributes(typeof(NonSettingAttribute), true).Length == 0
                ).ToDictionary
                (
                    p => p.Name, 
                    p => new KeyValuePair<PropertyInfo, CustomConversionAttribute>
                        (
                            p, 
                            p.GetCustomAttributes(typeof(CustomConversionAttribute), true).FirstOrDefault() as CustomConversionAttribute
                        )
                );
        }

#endregion

#region Class methods

        public void ApplyChanges()
        {
            _onApplyChanges?.Invoke();
        }

        public virtual void Setup()
        {
            Setup(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        public virtual void Setup(string configurationFile, string loggerName = null)
        {
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                string msg = $"Configuration file '{configurationFile}' is invalid or doesn't exists";
                throw new ArgumentException(msg);
            }

            XmlDocument xmlCfg = new XmlDocument();
            xmlCfg.Load(configurationFile);
            Setup(xmlCfg, loggerName);
        }

        public virtual void Setup(XmlDocument xmlCfg, string loggerName = null)
        {
            XmlNode cfgNode = GetLoggerSettingsNode(xmlCfg, loggerName);
            if (cfgNode == null)
            {
                string msg = $"Settings for logger '{_loggerType.Name}', name '{loggerName}' haven't found";
                throw new Exception(msg);
            }

            Setup(cfgNode);
        }

        public virtual void Setup(XmlNode cfgNode)
        {
            byte[] newHash = CalculateHash(cfgNode);
            if (newHash != null && !Helpers.ByteArrayEquals(newHash, SettingsHash)) //Setup if changed
            {
                BeginSetup();

                ApplyCommonSettings(cfgNode);

                Dictionary<string, string> settingsDict = GetSettingsDictionary(cfgNode);

                Dictionary<string, object> valuesDict = GetSettingsValues(settingsDict);

                VerifySettingsValues(valuesDict);

                ApplySettingsValues(valuesDict);

                RecalculateHash(cfgNode);

                FinalizeSetup();

                ApplyChanges();
            }
        }

        protected virtual void BeginSetup() { }

        protected virtual string GetLoggerSettingsNodeName(string loggerName = null)
        {
            loggerName = !string.IsNullOrEmpty(loggerName) ? $"[@name='{loggerName}']" : string.Empty;
            return $"{RootLoggersCfgPath}/{_loggerType.Name}{loggerName}";
        }

        protected internal XmlNode GetLoggerSettingsNode(XmlDocument xmlCfg, string loggerName = null)
        {
            string loggerXPath = GetLoggerSettingsNodeName(loggerName);
            return xmlCfg.SelectSingleNode(loggerXPath);
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

        protected virtual void ApplyCommonSettings(XmlNode cfgNode)
        {
            XmlAttribute aName = cfgNode.Attributes["name"];
            Name = aName?.InnerText.Trim();

            LoadEventsApplicableSet(cfgNode, "allow-events", out _allowEvents);
            if (_allowEvents != null)
            {
                CheckApplicableEvent = s => _allowEvents.Contains(s);
                return;
            }

            LoadEventsApplicableSet(cfgNode, "deny-events", out _denyEvents);
            if (_denyEvents != null)
            {
                CheckApplicableEvent = s => !_denyEvents.Contains(s);
                return;
            }

            CheckApplicableEvent = _defCheckApplicableEvent;
        }

        protected virtual Dictionary<string, string> GetSettingsDictionary(XmlNode cfgNode)
        {
            return GetSettingsDictionary(cfgNode, null);
        }

        protected virtual Dictionary<string, string> GetSettingsDictionary(XmlNode cfgNode, ICollection<string> excludes)
        {
            Dictionary<string, string> settingsDict = new Dictionary<string, string>();
            IEnumerable<XmlNode> nodes = cfgNode.OfType<XmlNode>().Where(n => n.NodeType != XmlNodeType.Comment);
            if (excludes != null)
            {
                nodes = nodes.Where(n => !excludes.Contains(n.Name, StringComparer.InvariantCultureIgnoreCase));
            }
            foreach (XmlNode settingNode in nodes)
            {
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
                settingsDict.Add(settingNode.Name, settingNode.InnerText);
            }
            return settingsDict;
        }

        protected virtual Dictionary<string, object> GetSettingsValues(Dictionary<string, string> settingsDict)
        {
            Dictionary<string, object> valuesDict = new Dictionary<string, object>(settingsDict.Count);
            foreach (KeyValuePair<string, string> setting in settingsDict)
            {
                KeyValuePair<PropertyInfo, CustomConversionAttribute> item = Properties[setting.Key];
                Type propertyType = item.Key.PropertyType;
                try
                {
                    object value;
                    string sValue = setting.Value.Trim();
                    if (item.Value != null)
                    {
                        value = item.Value.ConvertValue(sValue);
                    }
                    else
                    {
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

        protected virtual void ApplySettingsValues(Dictionary<string, object> valuesDict)
        {
            foreach (KeyValuePair<string, object> value in valuesDict)
            {
                PropertyInfo property = Properties[value.Key].Key;
                property.SetValue(this, value.Value, null);
            }
        }

        protected virtual void VerifySettingsValues(Dictionary<string, object> valuesDict) { }

        protected virtual byte[] CalculateHash(XmlNode cfgNode)
        {
            byte[] bXmlData = Encoding.ASCII.GetBytes(cfgNode.OuterXml);
            return new MD5CryptoServiceProvider().ComputeHash(bXmlData);
        }

        private void RecalculateHash(XmlNode cfgNode)
        {
            SettingsHash = CalculateHash(cfgNode);
        }

        protected virtual void FinalizeSetup() { }

        public virtual void Save(XmlNode cfgNode)
        {
            foreach (KeyValuePair<PropertyInfo, CustomConversionAttribute> item in Properties.Values)
            {
                PropertyInfo property = item.Key;
                object value = property.GetValue(this, null);

                if (item.Value != null)
                {
                    value = item.Value.UnconvertValue(value);
                }

                SetCfgNodeValue(cfgNode, property.Name, value);
            }
        }

#endregion

#region Helpers

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

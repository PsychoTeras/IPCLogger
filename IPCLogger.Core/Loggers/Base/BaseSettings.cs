using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using IPCLogger.Core.Attributes;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Loggers.Base
{
    // ReSharper disable PossibleNullReferenceException
    public abstract class BaseSettings
    {

#region Constants

        private static readonly Func<string, bool> DefCheckApplicableEvent = s => true;
        protected const string ROOT_LOGGERS_CFG_PATH = Constants.ROOT_LOGGER_CFG_PATH + "/Loggers";

#endregion

#region Private fields

        private Action _onApplyChanges;
        private Type _loggerType;

        private Dictionary<string, PropertyInfo> _properties;

        private HashSet<string> _allowEvents;
        private HashSet<string> _denyEvents;

#endregion

#region Protected fields

        protected byte[] SettingsHash;

#endregion

#region Internal fields

        internal Func<string, bool> CheckApplicableEvent = DefCheckApplicableEvent;

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
            _properties = GetType().GetProperties(
                BindingFlags.GetProperty | BindingFlags.Public | 
                BindingFlags.Instance | BindingFlags.FlattenHierarchy).
                Where
                (
                    p => p.CanRead && p.CanWrite && 
                         p.GetCustomAttributes(typeof(NonSettingAttribute), true).Length == 0
                ).ToDictionary(p => p.Name, p => p);
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
                string msg = string.Format("Configuration file '{0}' is invalid or doesn't exists",
                    configurationFile);
                throw new ArgumentException(msg);
            }

            XmlDocument xmlCfg = new XmlDocument();
            xmlCfg.Load(configurationFile);
            Setup(xmlCfg, loggerName);
        }

        public virtual void Setup(XmlDocument xmlCfg, string loggerName = null)
        {
            string loggerXPath = GetLoggerSettingsNodeName(loggerName);
            XmlNode cfgNode = xmlCfg.SelectSingleNode(loggerXPath);
            if (cfgNode == null)
            {
                string msg = string.Format("Settings for logger '{0}', name '{1}' haven't found",
                    _loggerType.Name, loggerName);
                throw new Exception(msg);
            }

            Setup(cfgNode);
        }

        public virtual void Setup(XmlNode cfgNode)
        {
            ApplyCommonSettings(cfgNode);

            Dictionary<string, string> settingsDict = GetSettingsDictionary(cfgNode);

            Dictionary<string, object> valuesDict = GetSettingsValues(settingsDict);

            VerifySettingsValues(valuesDict);

            ApplySettingsValues(valuesDict);

            RecalculateHash(cfgNode);

            ApplyChanges();
        }

        public void AllowAllEvents()
        {
            CheckApplicableEvent = DefCheckApplicableEvent;
        }

        public void AllowOrDenyEvents(ICollection<string> allowEvents, 
            ICollection<string> denyEvents)
        {
            _allowEvents = allowEvents != null && allowEvents.Count > 0
                ? new HashSet<string>(allowEvents)
                : null;
            if (_allowEvents != null)
            {
                CheckApplicableEvent = s => _allowEvents.Contains(s);
                return;
            }

            _denyEvents = denyEvents != null && denyEvents.Count > 0
                ? new HashSet<string>(denyEvents)
                : null;
            if (_denyEvents != null)
            {
                CheckApplicableEvent = s => !_denyEvents.Contains(s);
                return;
            }

            CheckApplicableEvent = DefCheckApplicableEvent;
        }

        protected virtual string GetLoggerSettingsNodeName(string loggerName = null)
        {
            loggerName = !string.IsNullOrEmpty(loggerName) ? string.Format("[@name='{0}']", loggerName) : string.Empty;
            return string.Format("{0}/{1}{2}", ROOT_LOGGERS_CFG_PATH, _loggerType.Name, loggerName);
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
            Name = aName != null ? aName.InnerText.Trim() : null;

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

            CheckApplicableEvent = DefCheckApplicableEvent;
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
                HashSet<string> hExcludes = new HashSet<string>(excludes.Select(s => s.ToLower()));
                nodes = nodes.Where(n => !hExcludes.Contains(n.Name.ToLower()));
            }
            foreach (XmlNode settingNode in nodes)
            {
                if (!_properties.ContainsKey(settingNode.Name))
                {
                    string msg = string.Format("Undefined settings node '{0}'", settingNode.Name);
                    throw new Exception(msg);
                }
                if (settingsDict.ContainsKey(settingNode.Name))
                {
                    string msg = string.Format("Duplicated settings definition '{0}'", settingNode.Name);
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
                Type propertyType = _properties[setting.Key].PropertyType;
                try
                {
                    object value = Convert.ChangeType(setting.Value.Trim(), propertyType, 
                                                      CultureInfo.InvariantCulture);
                    if (value == null)
                    {
                        throw new Exception();
                    }
                    valuesDict.Add(setting.Key, value);
                }
                catch
                {
                    string msg = string.Format("Invalid setting value '{0}' for setting '{1}' type '{2}'",
                        setting.Value, setting.Key, propertyType.Name);
                    throw new Exception(msg);
                }
            }
            return valuesDict;
        }

        protected virtual void ApplySettingsValues(Dictionary<string, object> valuesDict)
        {
            foreach (KeyValuePair<string, object> value in valuesDict)
            {
                PropertyInfo property = _properties[value.Key];
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

        internal virtual void SetupIfHasBeenChanged(XmlNode cfgNode)
        {
            byte[] newHash = CalculateHash(cfgNode);
            if (newHash != null && !Helpers.ByteArrayEquals(newHash, SettingsHash))
            {
                Setup(cfgNode);
            }
        }

#endregion

    }
    // ReSharper restore PossibleNullReferenceException
}

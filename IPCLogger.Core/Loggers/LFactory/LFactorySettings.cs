using IPCLogger.Core.Attributes;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace IPCLogger.Core.Loggers.LFactory
{
    // ReSharper disable PossibleNullReferenceException
    public sealed class LFactorySettings : BaseSettings
    {

#region Private fields

        private bool _noLock;
        private bool _autoReload;

#endregion

#region Properties

        [NonSetting]
        public bool NoLock
        {
            get { return _noLock; }
            set { _noLock = value; }
        }

        [NonSetting]
        public bool AutoReload
        {
            get { return _autoReload; }
            set { _autoReload = value; }
        }

        [NonSetting]
        internal bool ShouldLock
        {
            get { return !_noLock || _autoReload; }
        }

#endregion

#region Ctor

        public LFactorySettings(Type loggerType, Action onApplyChanges)
            : base(loggerType, onApplyChanges) { }

#endregion

#region Static methods

        internal static List<DeclaredLogger> GetDeclaredLoggers(string configurationFile, bool includeDisabled = false)
        {
            List<DeclaredLogger> loggers = new List<DeclaredLogger>();
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                return loggers;
            }

            XmlDocument xmlCfg = new XmlDocument();
            try
            {
                xmlCfg.Load(configurationFile);
            }
            catch
            {
                return loggers;
            }

            return GetDeclaredLoggers(xmlCfg, includeDisabled);
        }

        internal static List<DeclaredLogger> GetDeclaredLoggers(XmlDocument xmlCfg, bool includeDisabled = false)
        {
            List<DeclaredLogger> loggers = new List<DeclaredLogger>();

            string loggersXPath = $"{Constants.RootLoggersCfgPath}/*";
            XmlNodeList cfgNodes = xmlCfg.SelectNodes(loggersXPath);
            foreach (XmlNode cfgNode in cfgNodes.OfType<XmlNode>())
            {
                DeclaredLogger declaredLogger = new DeclaredLogger(cfgNode);
                if (includeDisabled || declaredLogger.Enabled)
                {
                    loggers.Add(declaredLogger);
                }
            }

            return loggers;
        }

        internal static DeclaredLogger GetDeclaredFactoryLogger(XmlDocument xmlCfg)
        {
            string loggersXPath = $"{Constants.RootAppCfgPath}";
            XmlNode cfgNode = xmlCfg.SelectSingleNode(loggersXPath);
            return cfgNode != null ? new DeclaredLogger(cfgNode) : null;
        }

        internal static DeclaredLogger CreateFactoryLogger(XmlDocument xmlCfg)
        {
            XmlNode AppendXmlNode(XmlNode parentNode, string nodeName, string nodePath = null)
            {
                XmlNode valNode = parentNode.SelectSingleNode(nodePath ?? nodeName);
                if (valNode == null)
                {
                    XmlDocument xmlDoc = parentNode.OwnerDocument;
                    valNode = xmlDoc.CreateNode(XmlNodeType.Element, nodeName, xmlDoc.NamespaceURI);
                    parentNode.AppendChild(valNode);
                }
                return valNode;
            }

            void AppendXmlAttribute(XmlNode parentNode, string attributeName, string value)
            {
                XmlAttribute valAttribute = parentNode.Attributes[attributeName];
                if (valAttribute == null)
                {
                    XmlDocument xmlDoc = parentNode.OwnerDocument;
                    valAttribute = xmlDoc.CreateAttribute(attributeName);
                    parentNode.Attributes.Append(valAttribute);
                }

                valAttribute.InnerText = value;
            }

            // Append config root node
            XmlNode rootCfgNode = AppendXmlNode(xmlCfg.SelectSingleNode("/"), "configuration");

            // Append emtpy logger config handler
            XmlNode rootCSectionsNode = AppendXmlNode(rootCfgNode, "configSections");
            XmlNode loggerCSectionNode = AppendXmlNode(rootCSectionsNode, "section",
                $"section[@name='{Constants.LoggerName}']");
            AppendXmlAttribute(loggerCSectionNode, "name", Constants.LoggerName);
            AppendXmlAttribute(loggerCSectionNode, "type", "System.Configuration.IgnoreSectionHandler");

            // Create logger section
            XmlNode loggerNode = AppendXmlNode(rootCfgNode, Constants.LoggerName);
            AppendXmlNode(loggerNode, "Patterns");
            AppendXmlNode(loggerNode, "Loggers");

            // Initialize settings
            LFactorySettings settings = new LFactorySettings(typeof(LFactory), null);
            settings.ApplyCommonSettings(loggerNode);
            settings.Save(loggerNode);

            return new DeclaredLogger(loggerNode);
        }

        internal static XmlNode AppendConfigurationNode(XmlDocument xmlCfg, XmlNode cfgNode)
        {
            XmlNode rootNode = xmlCfg.SelectSingleNode(Constants.RootLoggersCfgPath);
            XmlNode newNode = xmlCfg.ImportNode(cfgNode, true);
            rootNode.AppendChild(newNode);
            return newNode;
        }

#endregion

#region Class methods

        protected override Dictionary<string, string> GetCommonPropertiesSet()
        {
            return new Dictionary<string, string>
            {
                { "Enabled", "enabled" },
                { "NoLock", "no-lock" },
                { "AutoReload", "auto-reload" }
            };
        }

        protected override string GetLoggerSettingsNodeName(string loggerName = null)
        {
            return Constants.RootAppCfgPath;
        }

        protected override void ApplyCommonSettings(XmlNode cfgNode)
        {
            XmlAttribute aEnabled = cfgNode.Attributes["enabled"];
            Enabled = aEnabled == null || !bool.TryParse(aEnabled.Value, out var enabled) || enabled;
            XmlAttribute aNoLock = cfgNode.Attributes["no-lock"];
            _noLock = aNoLock != null && bool.TryParse(aNoLock.Value, out _noLock) && _noLock;
            XmlAttribute aAutoReload = cfgNode.Attributes["auto-reload"];
            _autoReload = aAutoReload != null && bool.TryParse(aAutoReload.Value, out _autoReload) && _autoReload;
        }

        protected override Dictionary<string, XmlNode> GetSettingsDictionary(XmlNode cfgNode)
        {
            return new Dictionary<string, XmlNode>();
        }

        protected override void Save(XmlNode cfgNode)
        {
            SetCfgAttributeValue(cfgNode, "enabled", Enabled);
            SetCfgAttributeValue(cfgNode, "no-lock", _noLock);
            SetCfgAttributeValue(cfgNode, "auto-reload", _autoReload);
        }

#endregion

    }

    internal class DeclaredLogger
    {
        public string TypeName;
        public string Namespace;
        public bool Enabled;
        public XmlNode CfgNode;
        public string UniqueId;

        public DeclaredLogger(XmlNode cfgNode)
        {
            CfgNode = cfgNode;
            TypeName = cfgNode.Name;
            Namespace = cfgNode.Attributes["namespace"]?.Value;
            XmlAttribute aEnabled = cfgNode.Attributes["enabled"];
            Enabled = aEnabled == null || !bool.TryParse(aEnabled.Value, out Enabled) || Enabled;
            string name = cfgNode.Attributes["name"]?.Value;
            UniqueId = Helpers.CalculateUniqueId(name, TypeName, Namespace);
        }
    }
    // ReSharper restore PossibleNullReferenceException
}

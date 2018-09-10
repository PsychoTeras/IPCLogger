using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using IPCLogger.Core.Attributes;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LFactory
{
    // ReSharper disable PossibleNullReferenceException
    public sealed class LFactorySettings : BaseSettings
    {

#region Private fields

        private bool _enabled;
        private bool _noLock;
        private bool _autoReload;

#endregion

#region Properties

        [NonSetting]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

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

        internal static List<DeclaredLogger> GetDeclaredLoggers(string configurationFile)
        {
            List<DeclaredLogger> loggers = new List<DeclaredLogger>();
            if (string.IsNullOrEmpty(configurationFile) || !System.IO.File.Exists(configurationFile))
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

            string loggersXPath = string.Format("{0}/*", RootLoggersCfgPath);
            XmlNodeList cfgNodes = xmlCfg.SelectNodes(loggersXPath);
            foreach (XmlNode cfgNode in cfgNodes.OfType<XmlNode>())
            {
                DeclaredLogger declaredLogger = new DeclaredLogger(cfgNode);
                if (declaredLogger.Enabled)
                {
                    loggers.Add(declaredLogger);
                }
            }

            return loggers;
        }

#endregion

#region Class methods

        protected override string GetLoggerSettingsNodeName(string loggerName = null)
        {
            return Constants.RootLoggerCfgPath;
        }

        protected override void ApplyCommonSettings(XmlNode cfgNode)
        {
            XmlAttribute aName = cfgNode.Attributes["name"];
            Name = aName != null ? aName.InnerText.Trim() : null;
            XmlAttribute aNoLock = cfgNode.Attributes["no-lock"];
            _noLock = aNoLock != null && bool.TryParse(aNoLock.Value, out _noLock) && _noLock;
            XmlAttribute aEnabled = cfgNode.Attributes["enabled"];
            _enabled = aEnabled == null || !bool.TryParse(aEnabled.Value, out _enabled) || _enabled;
            XmlAttribute aAutoReload = cfgNode.Attributes["auto-reload"];
            _autoReload = aAutoReload != null && bool.TryParse(aAutoReload.Value, out _autoReload) && _autoReload;
        }

        protected override Dictionary<string, string> GetSettingsDictionary(XmlNode cfgNode)
        {
            return new Dictionary<string, string>();
        }

#endregion

    }

    internal struct DeclaredLogger
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
            XmlAttribute aNamespace = cfgNode.Attributes["namespace"];
            Namespace = aNamespace != null ? aNamespace.Value : null;
            XmlAttribute aEnabled = cfgNode.Attributes["enabled"];
            Enabled = aEnabled == null || !bool.TryParse(aEnabled.Value, out Enabled) || Enabled;
            XmlAttribute aName = cfgNode.Attributes["name"];
            string name = aName != null ? aName.Value : null;
            UniqueId = Helpers.CalculateUniqueId(name, TypeName, Namespace);
        }
    }
    // ReSharper restore PossibleNullReferenceException
}

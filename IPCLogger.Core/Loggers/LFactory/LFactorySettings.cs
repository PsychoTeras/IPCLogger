using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LFactory
{
    // ReSharper disable PossibleNullReferenceException
    public class LFactorySettings : BaseSettings
    {

#region Private fields

        private bool _enabled;
        private bool _lightLock;
        private bool _autoReload;

#endregion

#region Properties

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public bool LightLock
        {
            get { return _lightLock; } 
            set { _lightLock = value; }
        }

        public bool AutoReload
        {
            get { return _autoReload; }
            set { _autoReload = value; }
        }

        internal bool ShouldLock
        {
            get { return !_lightLock || _autoReload; }
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

            string loggersXPath = string.Format("{0}/*", ROOT_LOGGERS_CFG_PATH);
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
            return Constants.ROOT_LOGGER_CFG_PATH;
        }

        protected override void ApplyCommonSettings(XmlNode cfgNode)
        {
            XmlAttribute aName = cfgNode.Attributes["name"];
            Name = aName != null ? aName.InnerText.Trim() : null;
        }

        protected override Dictionary<string, string> GetSettingsDictionary(XmlNode cfgNode)
        {
            XmlAttribute aLightLock = cfgNode.Attributes["light-lock"];
            _lightLock = aLightLock != null && bool.TryParse(aLightLock.Value, out _lightLock) && _lightLock;
            XmlAttribute aEnabled = cfgNode.Attributes["enabled"];
            _enabled = aEnabled == null || !bool.TryParse(aEnabled.Value, out _enabled) || _enabled;
            XmlAttribute aAutoReload = cfgNode.Attributes["auto-reload"];
            _autoReload = aAutoReload != null && bool.TryParse(aAutoReload.Value, out _autoReload) && _autoReload;
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

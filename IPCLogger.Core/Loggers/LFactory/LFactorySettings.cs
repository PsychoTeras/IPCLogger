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

#endregion

#region Overrided methods

        protected override Dictionary<string, string> GetCommonPropertiesNames()
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

        public override void ApplyCommonSettings(XmlNode cfgNode)
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

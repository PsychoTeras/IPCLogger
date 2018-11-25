using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Loggers.Base;
using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace IPCLogger.ConfigurationService.CoreServices
{
    internal class CoreService
    {

#region Private fields

#endregion

#region Properties

        public string ConfigurationFile { get; set; }

        public XmlDocument ConfigurationXml { get; set; }

        public LFactorySettings FactorySettings { get; set; }

        public List<LoggerModel> AvailableLoggers { get; set; }

        public List<DeclaredLoggerModel> DeclaredLoggers { get; set; }

#endregion

#region Ctor

        public CoreService(string configurationFile)
        {
            LoadConfiguration(configurationFile);
            ReadLoggers(configurationFile);
            ConfigurationFile = configurationFile;
        }

#endregion

#region Configuration methods

        private void LoadConfiguration(string configurationFile)
        {
            //Validate configuration file
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                string msg = $"Configuration file '{configurationFile}' is invalid or doesn't exists";
                throw new ArgumentException(msg);
            }

            //Load XML
            ConfigurationXml = new XmlDocument();
            ConfigurationXml.Load(configurationFile);

            //Load factory settings
            FactorySettings = new LFactorySettings(typeof(LFactory), null);
            XmlNode cfgNode = FactorySettings.GetLoggerSettingsNode(ConfigurationXml);
            FactorySettings.Setup(cfgNode);
        }

        public void SaveConfiguration()
        {
            ConfigurationXml.Save(ConfigurationFile);
        }

#endregion

#region Applications methods

        private void ReadAvailableLoggers(string configurationFile)
        {
            AvailableLoggers = LFactory.
                GetLoggers().
                Select(LoggerModel.FromType).
                OrderBy(t => t.TypeName).
                ToList();
        }

        private void ReadDeclaredLoggers(string configurationFile)
        {
            IEnumerable<Type> availableTypes = AvailableLoggers.Select(l => l.Type);
            DeclaredLoggers = LFactorySettings.
                GetDeclaredLoggers(configurationFile, true).
                Select(s => DeclaredLoggerModel.FromDeclaredLogger(s, availableTypes)).
                OrderBy(t => t.TypeName).
                ToList();
        }

        private void ReadLoggers(string configurationFile)
        {
            ReadAvailableLoggers(configurationFile);
            ReadDeclaredLoggers(configurationFile);
        }

#endregion

#region Application methods

        public BaseSettings ReadLoggerSettings(LoggerModel loggerModel)
        {
            return null;
        }

#endregion

    }
}

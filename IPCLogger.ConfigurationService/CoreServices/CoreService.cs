using IPCLogger.ConfigurationService.Entities.Models;
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

#region Properties

        public XmlDocument ConfigurationXml { get; set; }

        public List<LoggerModel> AvailableLoggers { get; set; }

        public List<DeclaredLoggerModel> DeclaredLoggers { get; set; }

#endregion

#region Ctor

        public CoreService(string configurationFile)
        {
            LoadConfiguration(configurationFile);
            ReadLoggers(configurationFile);
        }

#endregion

#region Configuration methods

        private void LoadConfiguration(string configurationFile)
        {
            //Validate configuration file
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                string msg = $"Configuration file '{configurationFile}' is invalid or doesn't exist";
                throw new ArgumentException(msg);
            }

            //Load XML
            ConfigurationXml = new XmlDocument();
            ConfigurationXml.Load(configurationFile);
        }

        public void SaveConfiguration()
        {
            ConfigurationXml.Save(new Uri(ConfigurationXml.BaseURI).LocalPath);
        }

#endregion

#region Application methods

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
                GetDeclaredLoggers(ConfigurationXml, true).
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

    }
}

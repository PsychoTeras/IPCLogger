using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using IPCLogger.ConfigurationService.Common.Exceptions;

namespace IPCLogger.ConfigurationService.CoreServices
{
    public class CoreService
    {

#region Private fields

        private int _applicationId;

        private string _configurationFile;

#endregion

#region Properties

        public XmlDocument ConfigurationXml { get; set; }

        public List<LoggerModel> AvailableLoggers { get; set; }

        public List<DeclaredLoggerModel> DeclaredLoggers { get; set; }

#endregion

#region Ctor

        public CoreService(int applicationId, string configurationFile)
        {
            _applicationId = applicationId;
            LoadConfiguration(_configurationFile = configurationFile);
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

        public XmlNode AppendConfigurationNode(XmlNode cfgNode)
        {
            return LFactorySettings.AppendConfigurationNode(ConfigurationXml, cfgNode);
        }

        public void SaveConfiguration()
        {
            ConfigurationXml.Save(_configurationFile);
        }

        public bool IsSameApplication(int applicationId)
        {
            return _applicationId == applicationId;
        }

        public bool IsSameConfiguration(string configurationFile)
        {
            return configurationFile.Equals(_configurationFile, StringComparison.InvariantCultureIgnoreCase);
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
                Select(s => DeclaredLoggerModel.FromCoreDeclaredLogger(s, availableTypes)).
                OrderBy(t => t.TypeName).
                ToList();
        }

        private void ReadLoggers(string configurationFile)
        {
            ReadAvailableLoggers(configurationFile);
            ReadDeclaredLoggers(configurationFile);
        }

        public LoggerModel GetAvailableLogger(string loggerId)
        {
            LoggerModel loggerModel = AvailableLoggers.FirstOrDefault(l => l.Id == loggerId);
            if (loggerModel == null)
            {
                throw new InvalidRequestException();
            }

            return loggerModel;
        }

        public DeclaredLoggerModel GetDeclaredLogger(string loggerId)
        {
            DeclaredLoggerModel loggerModel = DeclaredLoggers.FirstOrDefault(l => l.Id == loggerId);
            if (loggerModel == null)
            {
                throw new InvalidRequestException();
            }

            return loggerModel;
        }

#endregion

    }
}

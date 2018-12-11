using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using IPCLogger.ConfigurationService.Common.Exceptions;
using IPCLogger.ConfigurationService.Entities.DTO;
using static IPCLogger.Core.Loggers.Base.BaseSettings;

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

#region Loggers methods

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

        internal void ValidateLoggerUniqueness(DeclaredLoggerModel loggerModel, PropertyObjectDTO[] propertyObjs,
            ref PropertyValidationResult[] validationResult)
        {
            PropertyValidationResult pvrName = validationResult.FirstOrDefault(r => r.Name == "Name" && r.IsCommon);

            if (DeclaredLoggers.Exists(l => l.Name == (pvrName?.Value ?? loggerModel.Name) as string &&
                    l.TypeName == loggerModel.TypeName && l.Namespace == loggerModel.Namespace))
            {
                string msg = "Such logger with the same name is already registered. Please select another logger name";
                if (pvrName != null)
                {
                    pvrName.SetInvalid(msg);
                }
                else
                {
                    pvrName = PropertyValidationResult.Invalid("Name", true, msg);
                    Array.Resize(ref validationResult, validationResult.Length + 1);
                    validationResult[validationResult.Length - 1] = pvrName;
                }
            }
        }

        public void AppendLogger(DeclaredLoggerModel loggerModel)
        {
            DeclaredLoggers.Add(loggerModel);
        }

#endregion

    }
}

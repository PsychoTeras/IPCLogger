using IPCLogger.ConfigurationService.Common.Exceptions;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Loggers.LFactory;
using IPCLogger.Core.Snippets;
using IPCLogger.Core.Snippets.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using IPCLogger.Core.Patterns;
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

        public DeclaredLoggerModel FactoryLogger { get; set; }

        public List<DeclaredPatternModel> DeclaredPatterns { get; set; }

        public Dictionary<SnippetType, List<BaseSnippet>> Snippets { get; set; }

#endregion

#region Ctor

        public CoreService(int applicationId, string configurationFile)
        {
            _applicationId = applicationId;
            LoadConfigurationFile(_configurationFile = configurationFile);
            ReadLoggers();
            ReadPatterns();
            ReadSnippets();
        }

#endregion

#region Configuration methods

        private void LoadConfigurationFile(string configurationFile)
        {
            //Validate configuration file
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                string msg = $"Configuration file '{configurationFile}' is invalid or doesn't exist";
                throw new ArgumentException(msg);
            }

            //Load XML
            ConfigurationXml = new XmlDocument();
            ConfigurationXml.PreserveWhitespace = true;
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

        private void ReadAvailableLoggers()
        {
            AvailableLoggers = LFactory.
                GetLoggers().
                Select(LoggerModel.FromType).
                OrderBy(t => t.TypeName).
                ToList();
        }

        private void ReadDeclaredLoggers()
        {
            IEnumerable<Type> availableTypes = AvailableLoggers.Select(l => l.Type);
            DeclaredLoggers = LFactorySettings.
                GetDeclaredLoggers(ConfigurationXml, true).
                Select(s => DeclaredLoggerModel.FromDeclaredLogger(s, availableTypes)).
                ToList();

            DeclaredLogger dlFactory = LFactorySettings.GetDeclaredFactoryLogger(ConfigurationXml);
            if (dlFactory == null)
            {
                dlFactory = LFactorySettings.CreateFactoryLogger(ConfigurationXml);
                SaveConfiguration();
            }
            FactoryLogger = DeclaredLoggerModel.FromDeclaredLogger(dlFactory, typeof(LFactory));
        }

        private void ReadLoggers()
        {
            ReadAvailableLoggers();
            ReadDeclaredLoggers();
        }

        public LoggerModel GetAvailableLogger(string loggerId)
        {
            LoggerModel model = AvailableLoggers.FirstOrDefault(l => l.Id == loggerId);
            if (model == null)
            {
                throw new InvalidRequestException();
            }

            return model;
        }

        public DeclaredLoggerModel GetDeclaredLogger(string loggerId)
        {
            DeclaredLoggerModel model = FactoryLogger.Id == loggerId
                ? FactoryLogger
                : DeclaredLoggers.FirstOrDefault(l => l.Id == loggerId);
            if (model == null)
            {
                throw new InvalidRequestException();
            }

            return model;
        }

        internal void ValidateLoggerUniqueness(DeclaredLoggerModel loggerModel, PropertyObjectDTO[] propertyObjs,
            ref PropertyValidationResult[] validationResult)
        {
            PropertyValidationResult pvrName = validationResult.FirstOrDefault(r => r.Name == "Name" && r.IsCommon);

            if (DeclaredLoggers.Exists(l => l.Id != loggerModel.Id &&
                    l.Name == (pvrName?.Value ?? loggerModel.Name) as string &&
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

        public void RemoveLogger(string loggerId)
        {
            DeclaredLoggerModel model = DeclaredLoggers.FirstOrDefault(l => l.Id == loggerId);
            if (model == null)
            {
                throw new InvalidRequestException();
            }

            DeclaredLoggers.Remove(model);
            model.RootXmlNode.ParentNode?.RemoveChild(model.RootXmlNode);
            SaveConfiguration();
        }

#endregion

#region Patterns methods

        private void ReadPatterns()
        {
            DeclaredPatterns = PFactory.
                GetDeclaredPatterns(ConfigurationXml).
                Select(DeclaredPatternModel.FromDeclaredPattern).
                ToList();
        }

        public DeclaredPatternModel GetDeclaredPattern(string patternId)
        {
            DeclaredPatternModel model = DeclaredPatterns.FirstOrDefault(l => l.Id == patternId);
            if (model == null)
            {
                throw new InvalidRequestException();
            }

            return model;
        }

#endregion

#region Snippets methods

        private void ReadSnippets()
        {
            Snippets = SFactory.GetSnippets();
        }

#endregion

    }
}

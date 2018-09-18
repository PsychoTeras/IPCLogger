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

#region Private fields

        private string _configurationFile;
        private XmlDocument _configurationXml;

        private LFactorySettings _factorySettings;
        private List<DeclaredLoggerModel> _declaredLoggers;
        private List<RegisteredLoggerModel> _registeredLoggers;

#endregion

#region Properties

        public List<DeclaredLoggerModel> DeclaredLoggers
        {
            get { return _declaredLoggers; }
        }

        public List<RegisteredLoggerModel> RegisteredLoggers
        {
            get { return _registeredLoggers; }
        }

#endregion

#region Ctor

        public CoreService(string configurationFile)
        {
            LoadConfiguration(configurationFile);
            ReadLoggers(configurationFile);
            _configurationFile = configurationFile;
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
            _configurationXml = new XmlDocument();
            _configurationXml.Load(configurationFile);

            //Load factory settings
            _factorySettings = new LFactorySettings(typeof(LFactory), null);
            XmlNode cfgNode = _factorySettings.GetLoggerSettingsNode(_configurationXml);
            _factorySettings.Setup(cfgNode);
        }

        public void SaveConfiguration()
        {
            _configurationXml.Save(_configurationFile);
        }

#endregion

#region Applications methods

        private void ReadDeclaredLoggers(string configurationFile)
        {
            _declaredLoggers = LFactorySettings.
                GetDeclaredLoggers(configurationFile).
                Select(d => DeclaredLoggerModel.FromDeclaredLogger(d)).
                OrderBy(t => t.TypeName).
                ToList();
        }

        private void ReadRegisteredLoggers(string configurationFile)
        {
            _registeredLoggers = LFactory.
                GetRegisteredLoggers().
                Select(t => RegisteredLoggerModel.FromType(t)).
                OrderBy(t => t.TypeName).
                ToList();
        }

        private void ReadLoggers(string configurationFile)
        {
            ReadRegisteredLoggers(configurationFile);
            ReadDeclaredLoggers(configurationFile);
        }

#endregion

    }
}

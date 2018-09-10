using IPCLogger.Core.Loggers.LFactory;
using System;
using System.IO;
using System.Xml;

namespace IPCLogger.ConfigurationService.CoreServices
{
    internal class CoreService
    {

#region Private fields

        private string _configurationFile;
        private XmlDocument _configurationXml;

        private LFactorySettings _factorySettings;

#endregion

#region Class methods

        public void LoadConfiguration(string configurationFile)
        {
            //Validate configuration file
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                string msg = $"Configuration file '{configurationFile}' is invalid or doesn't exists";
                throw new ArgumentException(msg);
            }

            //Load XML
            _configurationXml = new XmlDocument();
            _configurationXml.Load(_configurationFile = configurationFile);

            //Load factory settings
            _factorySettings = new LFactorySettings(typeof(LFactory), null);
            XmlNode cfgNode = _factorySettings.GetLoggerSettingsNode(_configurationXml);
            _factorySettings.Setup(cfgNode);

            _factorySettings.NoLock = true;

            _factorySettings.Save(cfgNode);
        }

        public void SaveConfiguration()
        {
            _configurationXml.Save(_configurationFile);
        }

#endregion

    }
}

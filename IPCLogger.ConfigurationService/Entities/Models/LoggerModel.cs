using IPCLogger.Core.ConfigurationService;
using System;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class LoggerModel
    {
        private CSProperty[] _properties;

        public Type Type { get; protected set; }

        public string TypeName { get; protected set; }

        public string Namespace { get; protected set; }

        public CSProperty[] Properties
        {
            get
            {
                if (_properties == null)
                {
                    LoadProperties();
                }

                return _properties;
            }
        }

        internal static LoggerModel FromType(Type loggerType)
        {
            LoggerModel model = new LoggerModel();
            model.Type = loggerType;
            model.TypeName = loggerType.Name;
            if (loggerType.Module.Name != "IPCLogger.Core.dll")
            {
                model.Namespace = loggerType.Namespace;
            }
            return model;
        }

        private void LoadProperties()
        {
            List<CSProperty> properties = new List<CSProperty>();
            _properties = properties.ToArray();
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Namespace) ? TypeName : $"{TypeName} [{Namespace}]";
        }
    }
}

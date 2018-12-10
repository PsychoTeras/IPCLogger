using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using IPCLogger.ConfigurationService.Helpers;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DeclaredLoggerModel : LoggerModel
    {
        public string Name
        {
            get { return CommonProperties.FirstOrDefault(p => p.Name == "Name")?.Value; }
        }

        private void SetLoggerType(IEnumerable<Type> availableLoggers)
        {
            Type = availableLoggers.FirstOrDefault
            (
                t => t.Name == TypeName &&
                     (string.IsNullOrEmpty(Namespace) || t.Namespace != null && t.Namespace.Equals(Namespace))
            );
        }

        protected override void RecalculateId()
        {
            Id = BaseHelpers.CalculateMD5(BaseSettings.Hash);
        }

        internal static DeclaredLoggerModel FromLogger(LoggerModel source)
        {
            DeclaredLoggerModel model = new DeclaredLoggerModel();
            model.TypeName = source.TypeName;
            model.Namespace = source.Namespace;
            model.Type = source.Type;
            model.InitializeSettings();
            model.Id = source.Id;
            return model;
        }

        internal static DeclaredLoggerModel FromCoreDeclaredLogger(DeclaredLogger source, IEnumerable<Type> availableLoggers)
        {
            DeclaredLoggerModel model = new DeclaredLoggerModel();
            model.TypeName = source.TypeName;
            model.Namespace = source.Namespace;
            model.SetLoggerType(availableLoggers);
            model.InitializeSettings(source.CfgNode);
            return model;
        }
    }
}

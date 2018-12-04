using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.Linq;

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

        internal static DeclaredLoggerModel FromLogger(LoggerModel source)
        {
            DeclaredLoggerModel model = new DeclaredLoggerModel();
            model.TypeName = source.TypeName;
            model.Namespace = source.Namespace;
            model.Type = source.Type;
            model.CloneCSProperties(source);
            return model;
        }

        internal static DeclaredLoggerModel FromDeclaredLogger(DeclaredLogger source, IEnumerable<Type> availableLoggers)
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

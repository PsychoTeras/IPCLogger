using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DeclaredLoggerModel : LoggerModel
    {
        private void SetLoggerType(IEnumerable<Type> availableLoggers)
        {
            Type = availableLoggers.FirstOrDefault
            (
                t => t.Name == TypeName &&
                     (string.IsNullOrEmpty(Namespace) ||
                      t.Namespace != null && t.Namespace.Equals(Namespace, StringComparison.InvariantCultureIgnoreCase))
            );
        }

        internal static DeclaredLoggerModel FromLogger(LoggerModel source)
        {
            DeclaredLoggerModel model = new DeclaredLoggerModel();
            model.Id = source.Id; //BUG: ID needs to be generated
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

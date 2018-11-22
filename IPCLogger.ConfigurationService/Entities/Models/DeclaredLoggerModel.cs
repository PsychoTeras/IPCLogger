using IPCLogger.Core.ConfigurationService;
using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DeclaredLoggerModel : LoggerModel
    {
        public List<KeyValuePair<PropertyModel, object>> ChangedValues { get; }

        public DeclaredLoggerModel()
        {
            ChangedValues = new List<KeyValuePair<PropertyModel, object>>();
        }

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
            model.SetCSProperties(source.CfgNode);
            return model;
        }
    }
}

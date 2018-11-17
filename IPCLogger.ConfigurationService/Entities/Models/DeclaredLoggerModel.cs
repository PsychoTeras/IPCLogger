using System.Collections.Generic;
using IPCLogger.Core.ConfigurationService;
using IPCLogger.Core.Loggers.LFactory;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DeclaredLoggerModel : LoggerModel
    {
        public List<KeyValuePair<CSProperty, object>> ChangedValues { get; private set; }

        internal static DeclaredLoggerModel FromLogger(LoggerModel source)
        {
            DeclaredLoggerModel model = new DeclaredLoggerModel();
            model.TypeName = source.TypeName;
            model.Namespace = source.Namespace;
            return model;
        }

        internal static DeclaredLoggerModel FromDeclaredLogger(DeclaredLogger source)
        {
            DeclaredLoggerModel model = new DeclaredLoggerModel();
            model.TypeName = source.TypeName;
            model.Namespace = source.Namespace;
            return model;
        }
    }
}

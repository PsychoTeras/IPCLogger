using IPCLogger.Core.Loggers.LFactory;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DeclaredLoggerModel : RegisteredLoggerModel
    {
        internal static DeclaredLoggerModel FromRegisteredLogger(RegisteredLoggerModel source)
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

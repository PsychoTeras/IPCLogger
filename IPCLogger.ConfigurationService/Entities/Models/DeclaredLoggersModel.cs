using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DeclaredLoggersModel
    {
        public List<DeclaredLoggerModel> DeclaredLoggers { get; set; }

        public DeclaredLoggerModel FactoryLogger { get; set; }

        public DeclaredLoggersModel(List<DeclaredLoggerModel> declaredLoggers, DeclaredLoggerModel factoryLogger)
        {
            DeclaredLoggers = declaredLoggers;
            FactoryLogger = factoryLogger;
        }
    }
}

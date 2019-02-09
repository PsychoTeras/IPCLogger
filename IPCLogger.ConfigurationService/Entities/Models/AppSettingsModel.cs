using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class AppSettingsModel
    {
        public List<DeclaredLoggerModel> DeclaredLoggers { get; set; }

        public List<DeclaredPatternModel> DeclaredPatterns { get; set; }

        public DeclaredLoggerModel FactoryLogger { get; set; }

        public AppSettingsModel(List<DeclaredLoggerModel> declaredLoggers, 
            List<DeclaredPatternModel> declaredPatterns,
            DeclaredLoggerModel factoryLogger)
        {
            DeclaredLoggers = declaredLoggers;
            DeclaredPatterns = declaredPatterns;
            FactoryLogger = factoryLogger;
        }
    }
}

using IPCLogger.Core.Loggers.LFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using IPCLogger.ConfigurationService.Common;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DeclaredLoggerModel : LoggerModel
    {
        public string Name
        {
            get { return CommonProperties.FirstOrDefault(p => p.Name == "Name")?.Value; }
        }

        private static Type FindLoggerType(string typeName, string nameSpace, IEnumerable<Type> availableLoggers)
        {
            return availableLoggers.FirstOrDefault
            (
                t => t.Name == typeName &&
                     (string.IsNullOrEmpty(nameSpace) || t.Namespace != null && t.Namespace.Equals(nameSpace))
            );
        }

        protected override void RecalculateId()
        {
            Id = Helpers.CalculateMD5(BaseSettings.Hash);
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

        internal static DeclaredLoggerModel FromDeclaredLogger(DeclaredLogger source, IEnumerable<Type> availableLoggers)
        {
            Type loggerType = FindLoggerType(source.TypeName, source.Namespace, availableLoggers);
            return FromDeclaredLogger(source, loggerType);
        }

        internal static DeclaredLoggerModel FromDeclaredLogger(DeclaredLogger source, Type loggerType)
        {
            DeclaredLoggerModel model = new DeclaredLoggerModel();
            model.TypeName = source.TypeName;
            model.Namespace = source.Namespace;
            model.Type = loggerType;
            model.InitializeSettings(source.CfgNode);
            return model;
        }
    }
}

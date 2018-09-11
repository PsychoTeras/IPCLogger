using System;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class RegisteredLoggerModel
    {
        public string TypeName { get; set; }

        public string Namespace { get; set; }

        internal static RegisteredLoggerModel FromType(Type loggerType)
        {
            RegisteredLoggerModel model = new RegisteredLoggerModel();
            model.TypeName = loggerType.Name;
            if (loggerType.Module.Name != "IPCLogger.Core.dll")
            {
                model.Namespace = loggerType.Namespace;
            }
            return model;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Namespace) ? TypeName : $"{TypeName} [{Namespace}]";
        }
    }
}

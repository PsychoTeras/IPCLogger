using IPCLogger.Core.ConfigurationService;
using IPCLogger.Core.Loggers.Base;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class LoggerModel
    {
        private static int _refId;

        public int Id { get; }

        public Type Type { get; protected set; }

        public string TypeName { get; protected set; }

        public string Namespace { get; protected set; }

        public CSProperty[] Properties { get; private set; }

        public LoggerModel()
        {
            Id = Interlocked.Increment(ref _refId);
        }

        protected BaseSettings InstLoggerSettings(XmlNode cfgNode)
        {
            cfgNode = cfgNode ?? new Func<XmlNode>(() => new XmlDocument().CreateElement("_"))();
            Type bsType = ((TypeInfo) Type).ImplementedInterfaces
                .Select(i => i.GenericTypeArguments.FirstOrDefault(gt => gt.IsSubclassOf(typeof(BaseSettings))))
                .First(i => i != null);
            BaseSettings settings = (BaseSettings) Activator.CreateInstance(bsType, Type, null);
            settings.Setup(cfgNode);
            return settings;
        }

        protected void SetCSProperties(XmlNode cfgNode = null)
        {
            BaseSettings settings = InstLoggerSettings(cfgNode);
            Properties = settings.GetCSProperties();
        }

        protected void CloneCSProperties(LoggerModel source)
        {
            Properties = new CSProperty[source.Properties.Length];
            Array.Copy(source.Properties, Properties, source.Properties.Length);
        }

        internal static LoggerModel FromType(Type loggerType)
        {
            LoggerModel model = new LoggerModel();
            model.Type = loggerType;
            model.TypeName = loggerType.Name;
            if (loggerType.Module.Name != "IPCLogger.Core.dll")
            {
                model.Namespace = loggerType.Namespace;
            }
            model.SetCSProperties();
            return model;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Namespace) ? TypeName : $"{TypeName} [{Namespace}]";
        }
    }
}

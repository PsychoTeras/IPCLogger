using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Helpers;
using IPCLogger.Core.Loggers.Base;
using System;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class LoggerModel
    {
        private BaseSettings _baseSettings;

        public string Id { get; protected set; }

        public Type Type { get; protected set; }

        public string TypeName { get; protected set; }

        public string Namespace { get; protected set; }

        public PropertyModel[] Properties { get; private set; }

        protected BaseSettings InstLoggerSettings(XmlNode cfgNode)
        {
            cfgNode = cfgNode ?? new Func<XmlNode>(() => new XmlDocument().CreateElement("_"))();
            Type bsType = ((TypeInfo) Type).ImplementedInterfaces
                .Select(i => i.GenericTypeArguments.FirstOrDefault(gt => gt.IsSubclassOf(typeof(BaseSettings))))
                .First(i => i != null);
            _baseSettings = (BaseSettings) Activator.CreateInstance(bsType, Type, null);
            _baseSettings.Setup(cfgNode);
            return _baseSettings;
        }

        protected void InitializeSettings(XmlNode cfgNode = null)
        {
            _baseSettings = InstLoggerSettings(cfgNode);
            Id = BaseHelpers.CalculateMD5(_baseSettings.GetHash());
            Properties = _baseSettings.GetProperties().Select
            (
                p => new PropertyModel
                (
                    p.Item1.Name,
                    p.Item1.PropertyType,
                    p.Item2?.GetType(),
                    _baseSettings.GetPropertyValue(p.Item1),
                    _baseSettings.GetPropertyValues(p.Item1),
                    p.Item3
                )
            ).ToArray();
        }

        protected void CloneCSProperties(LoggerModel source)
        {
            Properties = new PropertyModel[source.Properties.Length];
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
            model.InitializeSettings();
            return model;
        }

        public InvalidPropertyValueDTO[] ValidateProperties(PropertyObjectDTO[] properties)
        {
            return properties.Select(p =>
                !_baseSettings.ValidatePropertyValue(p.Name, p.Value, out var errorMessage)
                    ? new InvalidPropertyValueDTO(p.Name, errorMessage)
                    : null).Where(p => p != null).ToArray();
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Namespace) ? TypeName : $"{TypeName} [{Namespace}]";
        }
    }
}

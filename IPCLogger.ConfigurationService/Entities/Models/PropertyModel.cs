using System;
using IPCLogger.Core.Attributes;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class PropertyModel
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public CustomConversionAttribute Extended { get; private set; }

        public object Value { get; private set; }

        public bool IsRequired { get; private set; }

        public PropertyModel(string name, Type type, CustomConversionAttribute extended, object value, bool isRequired)
        {
            Name = name;
            Type = type;
            Extended = extended;
            Value = value;
            IsRequired = isRequired;
        }

        public PropertyModel(string name, Type type, object value, bool isRequired) 
            : this(name, type, null, value, isRequired) { }

        public override string ToString()
        {
            return string.Format("{0} [{1}] = {2}", Name, Type.Name, Value ?? "NULL");
        }
    }
}

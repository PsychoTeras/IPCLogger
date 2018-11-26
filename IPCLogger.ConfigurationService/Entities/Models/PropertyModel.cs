using System;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class PropertyModel
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public Type Converter { get; private set; }

        public object Value { get; private set; }

        public bool IsRequired { get; private set; }

        public PropertyModel(string name, Type type, Type converter, object value, bool isRequired)
        {
            Name = name;
            Type = type;
            Converter = converter;
            Value = value;
            IsRequired = isRequired;
        }

        public PropertyModel(string name, Type type, object value, bool isRequired) 
            : this(name, type, null, value, isRequired) { }

        public override string ToString()
        {
            return $"{Name} [{Type.Name}] = {Value ?? "NULL"}";
        }
    }
}

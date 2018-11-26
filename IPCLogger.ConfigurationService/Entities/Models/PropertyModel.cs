using System;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class PropertyModel
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public Type Converter { get; private set; }

        public string Value { get; private set; }

        public string Values { get; private set; }

        public bool IsRequired { get; private set; }

        public PropertyModel(string name, Type type, Type converter, string value, string values, bool isRequired)
        {
            Name = name;
            Type = type;
            Converter = converter;
            Value = value;
            Values = values;
            IsRequired = isRequired;
        }

        public override string ToString()
        {
            return $"{Name} [{Type.Name}] = {Value ?? "NULL"}";
        }
    }
}

using IPCLogger.Core.Attributes;
using System;
using System.Text;

namespace IPCLogger.Core.ConfigurationService
{
    public class CSProperty
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public CustomConversionAttribute Extended { get; private set; }

        public object Value { get; private set; }

        public CSProperty(string name, Type type, CustomConversionAttribute extended, object value)
        {
            Name = name;
            Type = type;
            Extended = extended;
            Value = value;
        }

        public CSProperty(string name, Type type, object value) 
            : this(name, type, null, value) { }

        public override string ToString()
        {
            return string.Format("{0} [{1}] = {2}", Name, Type.Name, Value ?? "NULL");
        }
    }
}

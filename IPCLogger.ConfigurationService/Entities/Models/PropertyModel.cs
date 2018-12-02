using System;
using System.Text.RegularExpressions;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class PropertyModel
    {
        private static readonly Regex _regexMakeDisplayName = new Regex(@"(?<WORD>(^|[A-Z]|_)([a-z\d]+?)+)");

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public Type Type { get; private set; }

        public Type Converter { get; private set; }

        public string Value { get; private set; }

        public string Values { get; private set; }

        public bool IsCommon { get; private set; }

        public bool IsRequired { get; private set; }

        public PropertyModel(string name, Type type, Type converter, string value, string values, bool isCommon, bool isRequired)
        {
            Name = name;
            Type = type;
            Converter = converter;
            Value = value;
            Values = values;
            IsCommon = isCommon;
            IsRequired = isRequired;

            DisplayName = string.Empty;
            MatchCollection matches = _regexMakeDisplayName.Matches(Name);
            foreach (Match match in matches)
            {
                DisplayName += match.Value[0].ToString().ToUpper() + match.Value.Substring(1) + " ";
            }
            DisplayName = DisplayName.TrimEnd();
        }

        public void UpdateValue(string newValue)
        {
            Value = newValue;
        }

        public override string ToString()
        {
            return $"{Name} [{Type.Name}] = {Value ?? "NULL"}";
        }
    }
}

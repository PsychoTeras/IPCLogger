namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class PropertyObjectDTO
    {
        public string Name { get; }

        public string  Value { get; }

        public bool IsCommon { get; }

        public PropertyObjectDTO(string name, string value, bool isCommon)
        {
            Name = name;
            Value = value;
            IsCommon = isCommon;
        }

        public override string ToString()
        {
            return (IsCommon ? "#" : "") + $"{Name} = {Value}";
        }
    }
}

namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class PropertyObjectDTO
    {
        public string Name { get; }

        public string  Value { get; }

        public PropertyObjectDTO(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}

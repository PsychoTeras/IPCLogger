namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class PropertyObjectDTO
    {
        public string Name { get; set; }

        public string  Value { get; set; }

        public bool IsCommon { get; set; }

        public bool IsChanged { get; set; }

        public override string ToString()
        {
            return (IsCommon ? "#" : "") + $"{Name} = {Value}";
        }
    }
}

namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class InvalidPropertyValueDTO
    {
        public string Name { get; }

        public bool IsCommon { get; }

        public string ErrorMessage { get; }

        public InvalidPropertyValueDTO(string name, bool isCommon, string errorMessage)
        {
            Name = name;
            IsCommon = isCommon;
            ErrorMessage = errorMessage;
        }

        public InvalidPropertyValueDTO(string name, string errorMessage)
            : this(name, false, errorMessage)
        {
        }

        public override string ToString()
        {
            return IsCommon ? "#" + Name : Name;
        }
    }
}

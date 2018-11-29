namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class InvalidPropertyValueDTO
    {
        public string PropertyName { get; }

        public string ErrorMessage { get; }

        public InvalidPropertyValueDTO(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return PropertyName;
        }
    }
}

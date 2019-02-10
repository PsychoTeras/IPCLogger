namespace IPCLogger.Core.Common
{
    internal class PropertyValidationResult
    {
        public string Name;
        public object Value;
        public bool IsCommon;
        public bool IsValid;
        public string ErrorMessage;

        public void SetInvalid(string errorMessage)
        {
            Value = null;
            IsValid = false;
            ErrorMessage = errorMessage;
        }

        public static PropertyValidationResult Valid(string name, object value, bool isCommon)
        {
            return new PropertyValidationResult
            {
                Name = name,
                Value = value,
                IsCommon = isCommon,
                IsValid = true
            };
        }

        public static PropertyValidationResult Invalid(string name, bool isCommon, string errorMessage)
        {
            return new PropertyValidationResult
            {
                Name = name,
                IsCommon = isCommon,
                ErrorMessage = errorMessage
            };
        }
    }
}

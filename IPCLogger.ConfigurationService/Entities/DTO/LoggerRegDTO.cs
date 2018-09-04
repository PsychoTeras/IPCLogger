namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class LoggerRegDTO : DBRecord
    {
        public string ApplicationName { get; set; }

        public string Description { get; set; }

        public string ConfigurationFile { get; set; }
    }
}

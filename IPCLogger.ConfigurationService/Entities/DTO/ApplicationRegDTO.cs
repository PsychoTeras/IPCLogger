namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class ApplicationRegDTO : DBRecord
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ConfigurationFile { get; set; }
    }
}

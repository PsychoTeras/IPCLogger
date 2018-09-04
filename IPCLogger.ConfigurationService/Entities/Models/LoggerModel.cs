namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class LoggerModel : DBRecord
    {
        public string ApplicationName { get; set; }

        public string Description { get; set; }

        public string ConfigurationFile { get; set; }

        public bool Visible { get; set; }

        public override string ToString()
        {
            return ApplicationName;
        }
    }
}

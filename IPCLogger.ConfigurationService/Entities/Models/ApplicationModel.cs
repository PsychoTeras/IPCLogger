namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class ApplicationModel : DBRecord
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ConfigurationFile { get; set; }

        public bool Visible { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

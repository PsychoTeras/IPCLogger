namespace IPCLogger.ConfigurationService.Entities
{
    public class Role : DBRecord
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

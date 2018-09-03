namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class RoleModel : DBRecord
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

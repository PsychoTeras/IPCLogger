namespace IPCLogger.ConfigurationService.Entities
{
    public class UserModel : DBRecord
    {
        public string UserName { get; set; }

        public int RoleId { get; set; }

        public bool Blocked { get; set; }
    }
}

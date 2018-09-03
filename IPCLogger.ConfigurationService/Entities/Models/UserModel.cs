namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class UserModel : DBRecord
    {
        public string UserName { get; set; }

        public int RoleId { get; set; }

        public bool Blocked { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}

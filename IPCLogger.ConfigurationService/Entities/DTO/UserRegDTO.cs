using IPCLogger.ConfigurationService.Helpers;

namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class UserRegDTO : DBRecord
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordHash
        {
            get => BaseHelpers.CreateMD5(Password);
        }

        public int RoleId { get; set; }
    }
}

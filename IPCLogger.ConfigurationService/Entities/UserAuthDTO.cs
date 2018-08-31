using IPCLogger.ConfigurationService.Helpers;

namespace IPCLogger.ConfigurationService.Entities
{
    public class UserAuthDTO
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordHash
        {
            get => BaseHelpers.CreateMD5(Password);
        }

        public bool LoginFailed { get; set; }

        public bool RememberMe { get; set; }

        public UserAuthDTO() { }

        public UserAuthDTO(string userName, bool rememberMe, bool loginFailed)
        {
            UserName = userName;
            RememberMe = rememberMe;
            LoginFailed = loginFailed;
        }

        public UserAuthDTO(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
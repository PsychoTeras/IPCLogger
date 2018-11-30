using IPCLogger.ConfigurationService.Helpers;

namespace IPCLogger.ConfigurationService.Entities.DTO
{
    public class UserAuthDTO
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordHash
        {
            get => BaseHelpers.CalculateMD5(Password);
        }

        public bool LoginFailed { get; set; }

        public bool WrongSession { get; set; }

        public bool RememberMe { get; set; }

        public UserAuthDTO() { }

        public UserAuthDTO(string userName, bool rememberMe, bool loginFailed, bool wrongSession)
        {
            UserName = userName;
            RememberMe = rememberMe;
            LoginFailed = loginFailed;
            WrongSession = wrongSession;
        }

        public UserAuthDTO(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
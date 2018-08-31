using System.Security.Cryptography;
using System.Text;

namespace IPCLogger.ConfigurationService.Entities
{
    public class UserAuthDTO
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordHash
        {
            get => CreateMD5(Password);
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

        private string CreateMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte t in hashBytes)
                {
                    sb.Append($"{t:x2}");
                }
                return sb.ToString();
            }
        }
    }
}
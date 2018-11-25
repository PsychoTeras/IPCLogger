using System.Security.Cryptography;
using System.Text;

namespace IPCLogger.ConfigurationService.Helpers
{
    public static class BaseHelpers
    {
        public static string CreateMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

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

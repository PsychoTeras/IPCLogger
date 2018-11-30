using System.Security.Cryptography;
using System.Text;

namespace IPCLogger.ConfigurationService.Helpers
{
    public static class BaseHelpers
    {
        public static string CalculateMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return CalculateMD5(bytes);
        }

        public static string CalculateMD5(byte[] bytes)
        {
            if (bytes?.Length == 0) return null;

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(bytes);
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

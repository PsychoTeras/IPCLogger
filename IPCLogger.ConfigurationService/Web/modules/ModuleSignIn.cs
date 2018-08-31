using System;
using Nancy;
using Nancy.Security;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using Nancy.Authentication.Forms;
using Nancy.Extensions;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleSignIn : NancyModule
    {
        private static string CreateMD5(string input)
        {
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

        public ModuleSignIn()
        {
            Get["/"] = x =>
            {
                this.RequiresAuthentication();
                return View["index"];
            };

            Get["/signin"] = x =>
            {
                dynamic model = new ExpandoObject();
                model.Errored = Request.Query.error.HasValue;
                return View["signin", model];
            };

            Post["/signin"] = x =>
            {
                UserAuthDTO dto = new UserAuthDTO
                {
                    UserName = (string)Request.Form.userName,
                    PasswordHash = CreateMD5((string)Request.Form.password)
                };
                Guid? userGuid = UserDAL.Instance.Login(dto);

                if (userGuid == null)
                {
                    return Context.GetRedirect("~/signin?error=true&username=" + (string)Request.Form.userName);
                }

                DateTime? expiry = null;
                if (Request.Form.rememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry);
            };
        }
    }
}

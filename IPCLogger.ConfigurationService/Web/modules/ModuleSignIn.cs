using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities.DTO;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nancy.ModelBinding;
using System;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleSignIn : NancyModule
    {
        public ModuleSignIn()
        {
            Get["/signin"] = x =>
            {
                UserAuthDTO model = new UserAuthDTO
                (
                    Request.Query.username,
                    Request.Query.rememberme.HasValue,
                    Request.Query.failed.HasValue
                );
                return View["signin", model];
            };

            Post["/signin"] = x =>
            {
                UserAuthDTO model = this.Bind<UserAuthDTO>();
                Guid? userGuid = UserDAL.Instance.Login(model);

                if (userGuid == null)
                {
                    return Context.GetRedirect("~/signin?username=" + model.UserName + "&failed" +
                        (model.RememberMe ? "&rememberme" : ""));
                }

                DateTime? expiry = null;
                if (Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry);
            };
        }
    }
}

using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities.DTO;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nancy.ModelBinding;
using System;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleLogin : NancyModule
    {
        public ModuleLogin()
        {
            Get["/signin"] = x =>
            {
                UserAuthDTO model = new UserAuthDTO
                (
                    Request.Query.username,
                    Request.Query.rememberme.HasValue,
                    Request.Query.failed.HasValue,
                    Request.Query.wrongsession.HasValue
                );
                return View["signin", model];
            };

            Post["/signin"] = x =>
            {
                UserAuthDTO model = this.Bind<UserAuthDTO>();
                Guid? userGuid = UserDAL.Instance.Login(model);

                if (userGuid == null)
                {
                    string rememberMe = model.RememberMe ? "&rememberme" : "";
                    return Context.GetRedirect("~/signin?username=" + model.UserName + "&failed" + rememberMe);
                }

                DateTime? expiry = null;
                if (Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry);
            };

            Get["/signout"] = x => this.LogoutAndRedirect("/");
        }
    }
}

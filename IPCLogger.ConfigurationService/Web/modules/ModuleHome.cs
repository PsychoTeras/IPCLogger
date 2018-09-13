using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.Models;
using Nancy;
using Nancy.Responses;
using Nancy.Security;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleHome : NancyModule
    {
        public ModuleHome()
        {
            Get["/"] = x => Response.AsRedirect("/loggers", RedirectResponse.RedirectType.Temporary);

            Get["/loggers"] = x =>
            {
                this.RequiresAuthentication();

                List<LoggerModel> loggers = LoggerDAL.Instance.GetLoggers(false);
                return View["index", PageModel.Loggers(loggers)];
            };

            Get["/users"] = x =>
            {
                this.RequiresAuthentication();

                List<UserModel> users = UserDAL.Instance.GetUsers();
                return View["index", PageModel.Users(users)];
            };
        }
    }
}

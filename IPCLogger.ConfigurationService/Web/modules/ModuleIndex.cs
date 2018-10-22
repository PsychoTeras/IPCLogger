using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.Models;
using Nancy;
using Nancy.Responses;
using Nancy.Security;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleIndex : NancyModule
    {
        public ModuleIndex()
        {
            CoreService LoadCoreService(string configurationFile)
            {
                CoreService coreService;
                if (!ViewBag.CoreService.HasValue)
                {
                    coreService = new CoreService(configurationFile);
                    ViewBag.CoreService = coreService;
                }
                else
                {
                    coreService = ViewBag.CoreService;
                }
                return coreService;
            }

            Get["/"] = x => Response.AsRedirect("/loggers", RedirectResponse.RedirectType.Temporary);

            Get["/loggers"] = x =>
            {
                //this.RequiresAuthentication();

                List<LoggerModel> loggers = LoggerDAL.Instance.GetLoggers();
                return View["index", PageModel.Loggers(loggers)];
            };

            Get["/loggers/{id:int}"] = x =>
            {
                //this.RequiresAuthentication();

                int loggerId = int.Parse(x.id);
                LoggerModel logger = LoggerDAL.Instance.GetLogger(loggerId);

                CoreService coreService = LoadCoreService(logger.ConfigurationFile);

                PageModel previousPageModel = Session["PreviousPageModel"] as PageModel;
                return View["index", PageModel.Logger(logger, coreService.DeclaredLoggers, previousPageModel)];
            };

            Get["/users"] = x =>
            {
                //this.RequiresAuthentication();

                List<UserModel> users = UserDAL.Instance.GetUsers();
                return View["index", PageModel.Users(users)];
            };
        }
    }
}

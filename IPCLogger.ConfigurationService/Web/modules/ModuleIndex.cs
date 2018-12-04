using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.Models;
using Nancy;
using Nancy.Responses;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleIndex : ModuleBase
    {
        public ModuleIndex()
        {
            Get["/"] = x => Response.AsRedirect("/applications", RedirectResponse.RedirectType.Temporary);

            Get["/applications"] = x =>
            {
                VerifyAuthentication();

                List<ApplicationModel> applications = ApplicationDAL.Instance.GetApplications();
                PageModel pageModel = SetPageModel(() => PageModel.Applications(applications));
                return View["index", pageModel];
            };

            Get["/applications/{appid:int}"] = x =>
            {
                VerifyAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                ApplicationModel applicationModel = ApplicationDAL.Instance.GetApplication(applicationId);
                CoreService coreService = LoadCoreService(applicationModel.Id, applicationModel);

                PageModel pageModel = SetPageModel(() => PageModel.Loggers(applicationModel, coreService.DeclaredLoggers, PageModel));
                return View["index", pageModel];
            };

            Get["/applications/{appid:int}/loggers/{lid}/settings"] = x =>
            {
                VerifyAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                string loggerId = ViewBag.loggerId = x.lid;

                CoreService coreService = LoadCoreService(applicationId);
                DeclaredLoggerModel loggerModel = coreService.GetDeclaredLogger(loggerId);
                ViewBag.typeName = loggerModel.TypeName;

                PageModel pageModel = SetPageModel(() => PageModel.LoggerSettings(applicationId, loggerModel, PageModel));
                return View["index", pageModel];
            };

            Get["/users"] = x =>
            {
                VerifyAuthentication();

                List<UserModel> users = UserDAL.Instance.GetUsers();
                return View["index", PageModel.Users(users)];
            };
        }
    }
}

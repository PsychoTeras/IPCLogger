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
                PageModel pageModel = SetPageModel(PageModel.Applications(applications));
                return View["index", pageModel];
            };

            Get["/applications/{appid:int}"] = x =>
            {
                VerifyAuthentication();

                ViewBag.isEmbedded = ViewBag.hideHeaders = true;
                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                ApplicationModel applicationModel = ApplicationDAL.Instance.GetApplication(applicationId);
                CoreService coreService = GetCoreService(applicationModel.Id, applicationModel);
                ViewBag.loggerId = coreService.FactoryLogger.Id;

                DeclaredLoggersModel model = new DeclaredLoggersModel(coreService.DeclaredLoggers, coreService.FactoryLogger);
                PageModel pageModel = SetPageModel(PageModel.Loggers(applicationModel, model, PageModel));
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

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
        private PageModel PreviousPageModel
        {
            get { return Session["PreviousPageModel"] as PageModel; }
        }

        private CoreService CoreService
        {
            get { return Session["CoreService"] as CoreService; }
            set { Session["CoreService"] = value; }
        }

        public ModuleIndex()
        {
            CoreService LoadCoreService(string configurationFile)
            {
                CoreService coreService = CoreService;
                if (CoreService == null)
                {
                    coreService = new CoreService(configurationFile);
                    CoreService = coreService;
                }
                return coreService;
            }

            Get["/"] = x => Response.AsRedirect("/applications", RedirectResponse.RedirectType.Temporary);

            Get["/applications"] = x =>
            {
                //this.RequiresAuthentication();

                List<ApplicationModel> applications = ApplicationDAL.Instance.GetApplications();
                PageModel pageModel = PageModel.Applications(applications);
                return View["index", pageModel];
            };

            Get["/applications/{appid:int}"] = x =>
            {
                //this.RequiresAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                ApplicationModel applicationModel = ApplicationDAL.Instance.GetApplication(applicationId);
                CoreService coreService = LoadCoreService(applicationModel.ConfigurationFile);

                PageModel pageModel = PageModel.Loggers(applicationModel, coreService.DeclaredLoggers, PreviousPageModel);
                return View["index", pageModel];
            };

            Get["/applications/{appid:int}/logger/{lid:int}/settings"] = x =>
            {
                //this.RequiresAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                int loggerId = ViewBag.loggerId = int.Parse(x.lid);
                ApplicationModel applicationModel = ApplicationDAL.Instance.GetApplication(applicationId);
                CoreService coreService = LoadCoreService(applicationModel.ConfigurationFile);

                PageModel pageModel = PageModel.Loggers(applicationModel, coreService.DeclaredLoggers, PreviousPageModel);
                return View["index", pageModel];
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

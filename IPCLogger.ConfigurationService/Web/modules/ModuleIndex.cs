using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.Models;
using Nancy;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleIndex : NancyModule
    {
        private PageModel PageModel
        {
            get { return Context.Request.Session["PageModel"] as PageModel; }
            set { Context.Request.Session["PageModel"] = value; }
        }

        private CoreService CoreService
        {
            get { return Context.Request.Session["CoreService"] as CoreService; }
            set { Context.Request.Session["CoreService"] = value; }
        }

        public ModuleIndex()
        {
            CoreService LoadCoreService(int applicationId, ApplicationModel applicationModel = null)
            {
                CoreService coreService = CoreService;
                if (CoreService == null)
                {
                    applicationModel = applicationModel ?? ApplicationDAL.Instance.GetApplication(applicationId);
                    coreService = new CoreService(applicationModel.ConfigurationFile);
                    CoreService = coreService;
                }

                return coreService;
            }

            PageModel SetPageModel(Func<PageModel> funcPageModel)
            {
                PageModel previousPageModel = PageModel;
                PageModel currentPageModel = funcPageModel();
                if (currentPageModel != null && previousPageModel != null)
                {
                    PageModel model = currentPageModel;
                    PageModel existingPageModel = previousPageModel.FirstOrDefault(m => m.PageType == model.PageType);
                    if (existingPageModel != null)
                    {
                        currentPageModel = existingPageModel;
                    }
                }
                PageModel = currentPageModel;
                return currentPageModel;
            }

            Get["/"] = x => Response.AsRedirect("/applications", RedirectResponse.RedirectType.Temporary);

            Get["/applications"] = x =>
            {
                //this.RequiresAuthentication();

                List<ApplicationModel> applications = ApplicationDAL.Instance.GetApplications();
                PageModel pageModel = SetPageModel(() => PageModel.Applications(applications));
                return View["index", pageModel];
            };

            Get["/applications/{appid:int}"] = x =>
            {
                //this.RequiresAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                ApplicationModel applicationModel = ApplicationDAL.Instance.GetApplication(applicationId);
                CoreService coreService = LoadCoreService(applicationModel.Id, applicationModel);

                PageModel pageModel = SetPageModel(() => PageModel.Loggers(applicationModel, coreService.DeclaredLoggers, PageModel));
                return View["index", pageModel];
            };

            Get["/applications/{appid:int}/loggers/{lid:int}/settings"] = x =>
            {
                //this.RequiresAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                int loggerId = ViewBag.loggerId = int.Parse(x.lid);

                CoreService coreService = LoadCoreService(applicationId);
                DeclaredLoggerModel loggerModel = coreService.DeclaredLoggers.First(l => l.Id == loggerId);
                PageModel pageModel = SetPageModel(() => PageModel.LoggerSettings(applicationId, loggerModel, PageModel));
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

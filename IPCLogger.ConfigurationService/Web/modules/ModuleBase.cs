using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.Models;
using Nancy;
using System;
using System.Linq;
using Nancy.Security;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public abstract class ModuleBase : NancyModule
    {
        protected PageModel PageModel
        {
            get { return Context.Request.Session["PageModel"] as PageModel; }
            set { Context.Request.Session["PageModel"] = value; }
        }

        protected CoreService CoreService
        {
            get { return Context.Request.Session["CoreService"] as CoreService; }
            set { Context.Request.Session["CoreService"] = value; }
        }

        protected CoreService LoadCoreService(int applicationId, ApplicationModel applicationModel = null)
        {
            CoreService coreService = CoreService;
            if (CoreService == null || !CoreService.IsSameApplication(applicationId))
            {
                applicationModel = applicationModel ?? ApplicationDAL.Instance.GetApplication(applicationId);
                if (CoreService == null || !CoreService.IsSameConfiguration(applicationModel.ConfigurationFile))
                {
                    coreService = new CoreService(applicationId, applicationModel.ConfigurationFile);
                    CoreService = coreService;
                }
            }
            return coreService;
        }

        protected PageModel SetPageModel(Func<PageModel> funcPageModel)
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

        protected void VerifyAuthentication()
        {
            //this.RequiresAuthentication();
        }
    }
}

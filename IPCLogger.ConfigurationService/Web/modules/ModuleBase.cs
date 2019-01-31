using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.ConfigurationService.Web.modules.common;
using Nancy;
using System.IO;
using System.Linq;
using System.Web;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public abstract class ModuleBase : NancyModule
    {
        protected PageModel PageModel
        {
            get { return Context.Request.Session["PageModel"] as PageModel; }
            set { Context.Request.Session["PageModel"] = value; }
        }

        private CoreService CoreService
        {
            get { return Context.Request.Session["CoreService"] as CoreService; }
            set { Context.Request.Session["CoreService"] = value; }
        }

        private DocsService DocsService
        {
            get { return Context.Request.Session["DocsService"] as DocsService; }
            set { Context.Request.Session["DocsService"] = value; }
        }

        protected CoreService GetCoreService(int applicationId, ApplicationModel applicationModel = null)
        {
            CoreService coreService = CoreService;
            if (coreService == null || !coreService.IsSameApplication(applicationId))
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

        protected DocsService GetDocsService()
        {
            DocsService docsService = DocsService;
            if (docsService == null)
            {
                DefaultRootPathProvider pathProvider = new DefaultRootPathProvider();
                string docsPath = BootstrapperCommon.StaticContentsConventions.
                    First(kv => kv.Key == "docs").
                    Value.Substring(1).
                    Replace("/", "\\");
                docsPath = Path.Combine(pathProvider.GetRootPath(), docsPath);
                docsService = new DocsService(docsPath);
            }
            return docsService;
        }

        protected PageModel SetPageModel(PageModel pageModel)
        {
            PageModel previousPageModel = PageModel;
            PageModel currentPageModel = pageModel;
            if (currentPageModel != null && previousPageModel != null)
            {
                PageModel existingPageModel = previousPageModel.FirstOrDefault(m => m.PageType == currentPageModel.PageType);
                if (existingPageModel != null)
                {
                    currentPageModel = existingPageModel;
                    currentPageModel.Model = pageModel.Model;
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

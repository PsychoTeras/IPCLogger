using IPCLogger.ConfigurationService.Entities;
using Nancy;
using Nancy.Bootstrapper;
using System.Linq;

namespace IPCLogger.ConfigurationService.Web.modules.common
{
    public class RequestStartupCommon : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            pipelines.OnError += (ctx, ex) => throw ex;

            pipelines.BeforeRequest += (ctx) =>
            {
                return null;
            };

            pipelines.AfterRequest += (ctx) => 
            {
                if (ctx.NegotiationContext.ViewName != null)
                {
                    PageModel previousPageModel = ctx.Request.Session["PreviousPageModel"] as PageModel;
                    PageModel currentPageModel = ctx.NegotiationContext.DefaultModel as PageModel;
                    if (currentPageModel != null && previousPageModel != null)
                    {
                        PageModel existingPageModel = previousPageModel.FirstOrDefault(m => m.PageType == currentPageModel.PageType);
                        if (existingPageModel != null)
                        {
                            currentPageModel = existingPageModel;
                        }
                    }
                    ctx.Request.Session["PreviousPageModel"] = currentPageModel;
                }
            };
        }
    }
}

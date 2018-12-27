using IPCLogger.ConfigurationService.Entities;
using Nancy;
using Nancy.Extensions;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleSys : ModuleBase
    {
        public ModuleSys()
        {
            Get["/backurl"] = x => Response.AsText(PageModel?.PreviousPageModel?.PagePath);

            Post["/trackurl"] = x =>
            {
                PageModel pageModel = PageModel;
                string pagePath = Request.Body.AsString();
                if (pageModel != null && !string.IsNullOrWhiteSpace(pagePath))
                {
                    pageModel.PagePath = pagePath;
                }
                return null;
            };
        }
    }
}

using IPCLogger.ConfigurationService.Entities;
using Nancy.Extensions;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleSys : ModuleBase
    {
        public ModuleSys()
        {
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

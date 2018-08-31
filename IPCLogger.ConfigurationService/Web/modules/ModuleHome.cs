using Nancy;
using Nancy.Security;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleHome : NancyModule
    {
        public ModuleHome()
        {
            Get["/"] = x =>
            {
                this.RequiresAuthentication();
                return View["index"];
            };
        }
    }
}

using Nancy;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleSignIn : NancyModule
    {
        public ModuleSignIn()
        {
            Get["/"] = x => View["Web/html/signin.html"];
        }
    }
}

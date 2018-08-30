using Nancy;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleMain : NancyModule
    {
        public ModuleMain()
        {
            Get["/"] = x => View["Web/views/main.html"];
        }
    }
}

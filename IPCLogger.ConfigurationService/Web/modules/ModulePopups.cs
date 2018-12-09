using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.Entities.Models;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModulePopups : ModuleBase
    {
        public ModulePopups()
        {
            Get["/applications/{appid:int}/popupAddLogger"] = x =>
            {
                VerifyAuthentication();

                int applicationId = int.Parse(x.appid);
                CoreService coreService = LoadCoreService(applicationId);
                List<LoggerModel> loggers = coreService.AvailableLoggers;
                return View["popups/popupAddLogger", loggers];
            };            
        }
    }
}

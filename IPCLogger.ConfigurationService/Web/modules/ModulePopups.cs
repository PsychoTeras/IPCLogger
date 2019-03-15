using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Snippets.Base;
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
                CoreService coreService = GetCoreService(applicationId);
                List<LoggerModel> loggers = coreService.AvailableLoggers;
                return View["popups/popupAddLogger", loggers];
            };

            Get["/applications/{appid:int}/popupSnippetsInfo"] = x =>
            {
                VerifyAuthentication();

                int applicationId = int.Parse(x.appid);
                CoreService coreService = GetCoreService(applicationId);
                DocsService docsService = DocsService.Instance;
                SnippetsInfoModel model = new SnippetsInfoModel(null, docsService);
                return View["popups/popupSnippetsInfo", model];
            };
        }
    }
}

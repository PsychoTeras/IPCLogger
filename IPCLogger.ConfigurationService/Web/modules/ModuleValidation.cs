using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleValidation : NancyModule
    {
        private CoreService CoreService
        {
            get { return Context.Request.Session["CoreService"] as CoreService; }
            set { Context.Request.Session["CoreService"] = value; }
        }

        public ModuleValidation()
        {
            CoreService LoadCoreService(int applicationId, ApplicationModel applicationModel = null)
            {
                CoreService coreService = CoreService;
                if (CoreService == null)
                {
                    applicationModel = applicationModel ?? ApplicationDAL.Instance.GetApplication(applicationId);
                    coreService = new CoreService(applicationModel.ConfigurationFile);
                    CoreService = coreService;
                }

                return coreService;
            }

            Post["/validate/properties"] = x =>
            {
                //this.RequiresAuthentication();
                string jsonPropertyObjs = Request.Body.AsString();
                if (string.IsNullOrEmpty(jsonPropertyObjs))
                {
                    return null;
                }

                int loggerId;
                int applicationId;
                PropertyObjectDTO[] propertyObjs;
                try
                {
                    loggerId = int.Parse(Request.Query["lid"]);
                    applicationId = int.Parse(Request.Query["appid"]);
                    propertyObjs = JsonConvert.DeserializeObject<PropertyObjectDTO[]>(jsonPropertyObjs);
                    if (propertyObjs == null) throw new Exception();
                }
                catch
                {
                    return null;
                }

                CoreService coreService = LoadCoreService(applicationId);
                DeclaredLoggerModel loggerModel = coreService.DeclaredLoggers.First(l => l.Id == loggerId);
                InvalidPropertyValueDTO[] validationResult = loggerModel.ValidateProperties(propertyObjs);
                return Response.AsJson(validationResult);
            };
        }
    }
}

using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Loggers.Base;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleSettings : NancyModule
    {
        private CoreService CoreService
        {
            get { return Context.Request.Session["CoreService"] as CoreService; }
            set { Context.Request.Session["CoreService"] = value; }
        }

        public ModuleSettings()
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

            Post["/settings/save"] = x =>
            {
                //this.RequiresAuthentication();

                string jsonPropertyObjs = Request.Body.AsString();
                if (string.IsNullOrEmpty(jsonPropertyObjs))
                {
                    return null;
                }

                string loggerId;
                int applicationId;
                PropertyObjectDTO[] propertyObjs;
                try
                {
                    loggerId = Request.Query["lid"];
                    applicationId = int.Parse(Request.Query["appid"]);
                    propertyObjs = JsonConvert.DeserializeObject<PropertyObjectDTO[]>(jsonPropertyObjs);
                    if (propertyObjs == null) throw new Exception();
                }
                catch
                {
                    return null;
                }

                try
                {
                    CoreService coreService = LoadCoreService(applicationId);
                    DeclaredLoggerModel loggerModel = coreService.DeclaredLoggers.First(l => l.Id == loggerId);
                    PropertyValidationResult[] validationResult = loggerModel.ValidateProperties(propertyObjs);
                    InvalidPropertyValueDTO[] invalidProperties = validationResult.
                        Where(r => !r.IsValid).
                        Select(r => new InvalidPropertyValueDTO(r.Name, r.ErrorMessage)).
                        ToArray();
                    if (!invalidProperties.Any())
                    {
                        loggerModel.UpdateSettings(validationResult, propertyObjs);
                        coreService.SaveConfiguration();
                    }
                    return Response.AsJson(invalidProperties);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            };
        }
    }
}

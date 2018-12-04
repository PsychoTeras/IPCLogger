using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Loggers.Base;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IPCLogger.ConfigurationService.Web.modules
{
    using PropertyValidationResult = BaseSettings.PropertyValidationResult;

    public class ModuleSettings : ModuleBase
    {
        public ModuleSettings()
        {
            Post["/settings/save"] = x =>
            {
                VerifyAuthentication();

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
                    DeclaredLoggerModel loggerModel = coreService.GetDeclaredLogger(loggerId);
                    IEnumerable<PropertyValidationResult> validationResult = loggerModel.ValidateProperties(propertyObjs);
                    IEnumerable<InvalidPropertyValueDTO> invalidProperties = validationResult.
                        Where(r => !r.IsValid).
                        Select(r => new InvalidPropertyValueDTO(r.Name, r.IsCommon, r.ErrorMessage));
                    if (!invalidProperties.Any() && loggerModel.UpdateSettings(validationResult, propertyObjs))
                    {
                        coreService.SaveConfiguration();
                        loggerModel.ReloadProperties();
                    }

                    return Response.AsJson(invalidProperties.ToArray());
                }
                catch (Exception ex)
                {
                    return Response.AsJson(ex.Message, HttpStatusCode.BadRequest);
                }
            };
        }
    }
}

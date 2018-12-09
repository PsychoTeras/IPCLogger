using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.Entities;
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

    public class ModuleLoggerSettings : ModuleBase
    {
        public ModuleLoggerSettings()
        {
            Get["/applications/{appid:int}/loggers/{lid}/settings"] = x =>
            {
                VerifyAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                string loggerId = ViewBag.loggerId = x.lid;

                CoreService coreService = LoadCoreService(applicationId);
                DeclaredLoggerModel loggerModel = coreService.GetDeclaredLogger(loggerId);
                ViewBag.typeName = loggerModel.TypeName;

                PageModel pageModel = SetPageModel(() => PageModel.LoggerSettings(applicationId, loggerModel, PageModel));
                return View["index", pageModel];
            };

            Post["/applications/{appid:int}/loggers/{lid}/settings"] = x =>
            {
                VerifyAuthentication();

                string jsonPropertyObjs = Request.Body.AsString();
                if (string.IsNullOrEmpty(jsonPropertyObjs))
                {
                    return null;
                }

                int applicationId = int.Parse(x.appid);
                string loggerId = x.lid;

                PropertyObjectDTO[] propertyObjs;
                try
                {
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
                    PropertyValidationResult[] validationResult = loggerModel.ValidateProperties(propertyObjs);
                    IEnumerable<InvalidPropertyValueDTO> invalidProperties = validationResult.
                        Where(r => !r.IsValid).
                        Select(r => new InvalidPropertyValueDTO(r.Name, r.IsCommon, r.ErrorMessage));
                    if (!invalidProperties.Any())
                    {
                        if (loggerModel.UpdateSettings(validationResult, propertyObjs))
                        {
                            coreService.SaveConfiguration();
                            loggerModel.ReloadProperties();
                        }
                        else
                        {
                            invalidProperties = validationResult.
                                Where(r => !r.IsValid).
                                Select(r => new InvalidPropertyValueDTO(r.Name, r.IsCommon, r.ErrorMessage));
                        }
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

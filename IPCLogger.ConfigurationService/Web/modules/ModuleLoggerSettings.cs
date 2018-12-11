using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Loggers.Base;
using Nancy;
using Nancy.Extensions;
using Nancy.Responses.Negotiation;
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
            Negotiator NewOrGet(dynamic x, bool isNew)
            {
                VerifyAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                string loggerId = ViewBag.loggerId = x.lid;
                ViewBag.isNew = isNew;

                CoreService coreService = LoadCoreService(applicationId);
                DeclaredLoggerModel loggerModel;
                if (isNew)
                {
                    LoggerModel availableLoggerModel = coreService.GetAvailableLogger(loggerId);
                    loggerModel = DeclaredLoggerModel.FromLogger(availableLoggerModel);
                }
                else
                {
                    loggerModel = coreService.GetDeclaredLogger(loggerId);
                }

                ViewBag.typeName = loggerModel.TypeName;

                PageModel pageModel = SetPageModel(() => PageModel.AddLogger(applicationId, loggerModel, PageModel));
                return View["index", pageModel];
            }

            Response CreateOrUpdate(dynamic x, bool create)
            {
                VerifyAuthentication();

                string jsonPropertyObjs = Request.Body.AsString();
                if (string.IsNullOrEmpty(jsonPropertyObjs))
                {
                    return null;
                }

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
                    int applicationId = int.Parse(x.appid);
                    string loggerId = x.lid;

                    CoreService coreService = LoadCoreService(applicationId);

                    DeclaredLoggerModel loggerModel;
                    if (create)
                    {
                        LoggerModel availableLoggerModel = coreService.GetAvailableLogger(loggerId);
                        loggerModel = DeclaredLoggerModel.FromLogger(availableLoggerModel);
                    }
                    else
                    {
                        loggerModel = coreService.GetDeclaredLogger(loggerId);
                    }

                    PropertyValidationResult[] validationResult = loggerModel.ValidateProperties(propertyObjs);
                    CoreService.ValidateLoggerUniqueness(loggerModel, propertyObjs, ref validationResult);

                    IEnumerable<InvalidPropertyValueDTO> invalidProperties = validationResult.
                        Where(r => !r.IsValid).
                        Select(r => new InvalidPropertyValueDTO(r.Name, r.IsCommon, r.ErrorMessage));
                    if (!invalidProperties.Any())
                    {
                        if (loggerModel.UpdateSettings(validationResult, propertyObjs))
                        {
                            if (create)
                            {
                                loggerModel.RootXmlNode = coreService.AppendConfigurationNode(loggerModel.RootXmlNode);
                                loggerModel.ReinitializeSettings();
                                coreService.AppendLogger(loggerModel);
                            }
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
            }

            Get["/applications/{appid:int}/loggers/{lid}"] = x => NewOrGet(x, true);

            Get["/applications/{appid:int}/loggers/{lid}/settings"] = x => NewOrGet(x, false);

            Post["/applications/{appid:int}/loggers/{lid}"] = x => CreateOrUpdate(x, true);

            Post["/applications/{appid:int}/loggers/{lid}/settings"] = x => CreateOrUpdate(x, false);
        }
    }
}

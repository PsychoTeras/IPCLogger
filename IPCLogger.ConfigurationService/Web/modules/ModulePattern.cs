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

    public class ModulePatternSettings : ModuleBase
    {
        public ModulePatternSettings()
        {
            Negotiator NewOrGet(dynamic x, bool isNew)
            {
                VerifyAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                string patternId = ViewBag.patternId = x.lid;
                ViewBag.isNew = isNew;

                CoreService coreService = GetCoreService(applicationId);
                DeclaredPatternModel model = isNew
                    ? new DeclaredPatternModel()
                    : coreService.GetDeclaredPattern(patternId);

                PageModel pageModel = SetPageModel
                (
                    isNew
                        ? PageModel.AddPattern(applicationId, model, PageModel)
                        : PageModel.PatternSettings(applicationId, model, PageModel)
                );
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

                    CoreService coreService = GetCoreService(applicationId);

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
                    coreService.ValidateLoggerUniqueness(loggerModel, propertyObjs, ref validationResult);

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

            Get["/applications/{appid:int}/patterns/{lid}"] = x => NewOrGet(x, true);

            Get["/applications/{appid:int}/patterns/{lid}/settings"] = x => NewOrGet(x, false);

            Post["/applications/{appid:int}/patterns/{lid}"] = x => CreateOrUpdate(x, true);

            Post["/applications/{appid:int}/patterns/{lid}/settings"] = x => CreateOrUpdate(x, false);

            Delete["/applications/{appid:int}/patterns/{lid}"] = x =>
            {
                VerifyAuthentication();

                try
                {
                    int applicationId = int.Parse(x.appid);
                    string loggerId = x.lid;
                    CoreService coreService = GetCoreService(applicationId);
                    coreService.RemoveLogger(loggerId);
                    return null;
                }
                catch (Exception ex)
                {
                    return Response.AsJson(ex.Message, HttpStatusCode.BadRequest);
                }
            };
        }
    }
}

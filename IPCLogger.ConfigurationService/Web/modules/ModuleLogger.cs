using IPCLogger.ConfigurationService.CoreInterops;
using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using Nancy;
using Nancy.Extensions;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IPCLogger.ConfigurationService.Web.modules
{
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

                CoreService coreService = GetCoreService(applicationId);
                DeclaredLoggerModel model;
                if (isNew)
                {
                    LoggerModel availableLoggerModel = coreService.GetAvailableLogger(loggerId);
                    model = DeclaredLoggerModel.FromLogger(availableLoggerModel);
                }
                else
                {
                    model = coreService.GetDeclaredLogger(loggerId);
                }

                ViewBag.typeName = model.TypeName;

                PageModel pageModel = SetPageModel
                (
                    isNew
                        ? PageModel.AddLogger(applicationId, model, PageModel)
                        : PageModel.LoggerSettings(applicationId, model, PageModel)
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
                    return Response.AsJson("Invalid properties object", HttpStatusCode.BadRequest);
                }

                try
                {
                    int applicationId = int.Parse(x.appid);
                    string loggerId = x.lid;

                    CoreService coreService = GetCoreService(applicationId);

                    DeclaredLoggerModel model;
                    if (create)
                    {
                        LoggerModel availableLoggerModel = coreService.GetAvailableLogger(loggerId);
                        model = DeclaredLoggerModel.FromLogger(availableLoggerModel);
                    }
                    else
                    {
                        model = coreService.GetDeclaredLogger(loggerId);
                    }

                    PropertyValidationResult[] validationResult = model.ValidateProperties(propertyObjs);
                    coreService.ValidateLoggerUniqueness(model, propertyObjs, ref validationResult);

                    IEnumerable<InvalidPropertyValueDTO> invalidProperties = validationResult.
                        Where(r => !r.IsValid).
                        Select(r => new InvalidPropertyValueDTO(r.Name, r.IsCommon, r.ErrorMessage));
                    if (!invalidProperties.Any())
                    {
                        if (model.UpdateSettings(validationResult, propertyObjs))
                        {
                            if (create)
                            {
                                model.RootXmlNode = coreService.AppendLoggerNode(model.RootXmlNode);
                                model.ReinitializeSettings();
                                coreService.AppendLogger(model);
                            }
                            coreService.SaveConfiguration();
                            model.ReloadProperties();
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

            Get["/applications/{appid:int}/loggers/{lid}/new"] = x => NewOrGet(x, true);

            Get["/applications/{appid:int}/loggers/{lid}/settings"] = x => NewOrGet(x, false);

            Post["/applications/{appid:int}/loggers/{lid}/new"] = x => CreateOrUpdate(x, true);

            Post["/applications/{appid:int}/loggers/{lid}/settings"] = x => CreateOrUpdate(x, false);

            Delete["/applications/{appid:int}/loggers/{lid}"] = x =>
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

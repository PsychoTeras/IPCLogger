using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.DTO;
using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Common;
using Nancy;
using Nancy.Extensions;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using IPCLogger.ConfigurationService.CoreInterops;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModulePatternSettings : ModuleBase
    {
        public ModulePatternSettings()
        {
            Negotiator NewOrGet(dynamic x, bool isNew)
            {
                VerifyAuthentication();

                int applicationId = ViewBag.applicationId = int.Parse(x.appid);
                string patternId = ViewBag.patternId = x.pid;
                ViewBag.isNew = isNew;

                CoreService coreService = GetCoreService(applicationId);
                DeclaredPatternModel model = isNew
                    ? DeclaredPatternModel.CreateNew()
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
                    string patternId = x.pid;

                    CoreService coreService = GetCoreService(applicationId);

                    DeclaredPatternModel model = create
                        ? DeclaredPatternModel.CreateNew()
                        : coreService.GetDeclaredPattern(patternId);

                    PropertyValidationResult[] validationResult = model.ValidateProperties(propertyObjs);

                    IEnumerable<InvalidPropertyValueDTO> invalidProperties = validationResult.
                        Where(r => !r.IsValid).
                        Select(r => new InvalidPropertyValueDTO(r.Name, r.ErrorMessage));
                    if (!invalidProperties.Any())
                    {
                        if (model.UpdateSettings(validationResult, propertyObjs))
                        {
                            if (create)
                            {
                                model.RootXmlNode = coreService.AppendPatternNode(model.RootXmlNode);
                                model.ReinitializeSettings();
                                coreService.AppendPattern(model);
                            }
                            else
                            {
                                model.ReloadProperties();
                            }
                            coreService.SaveConfiguration();
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

            Get["/applications/{appid:int}/patterns/new"] = x => NewOrGet(x, true);

            Get["/applications/{appid:int}/patterns/{pid}/settings"] = x => NewOrGet(x, false);

            Post["/applications/{appid:int}/patterns/new"] = x => CreateOrUpdate(x, true);

            Post["/applications/{appid:int}/patterns/{pid}/settings"] = x => CreateOrUpdate(x, false);

            Delete["/applications/{appid:int}/patterns/{pid}"] = x =>
            {
                VerifyAuthentication();

                try
                {
                    int applicationId = int.Parse(x.appid);
                    string patternId = x.pid;
                    CoreService coreService = GetCoreService(applicationId);
                    coreService.RemovePattern(patternId);
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

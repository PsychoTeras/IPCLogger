﻿using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.ConfigurationService.DAL;
using IPCLogger.ConfigurationService.Entities;
using IPCLogger.ConfigurationService.Entities.Models;
using Nancy;
using Nancy.Responses;
using Nancy.Security;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleIndex : NancyModule
    {
        public ModuleIndex()
        {
            Get["/"] = x => Response.AsRedirect("/loggers", RedirectResponse.RedirectType.Temporary);

            Get["/loggers"] = x =>
            {
                //this.RequiresAuthentication();

                List<LoggerModel> loggers = LoggerDAL.Instance.GetLoggers(false);
                return View["index", PageModel.Loggers(loggers)];
            };

            Get["/loggers/{id:int}"] = x =>
            {
                //this.RequiresAuthentication();

                ModuleIndex me = this;

                int loggerId = int.Parse(x.id);
                string configurationFile = LoggerDAL.Instance.GetConfigurationFile(loggerId);
                CoreService coreService;
                if (!me.ViewBag.CoreService.HasValue)
                {
                    coreService = new CoreService(configurationFile);
                    me.ViewBag.CoreService = new CoreService(configurationFile);
                }
                else
                {
                    coreService = me.ViewBag.CoreService;
                }

                return View["index", PageModel.Logger(coreService.DeclaredLoggers)];
            };

            Get["/users"] = x =>
            {
                //this.RequiresAuthentication();

                List<UserModel> users = UserDAL.Instance.GetUsers();
                return View["index", PageModel.Users(users)];
            };
        }
    }
}

using IPCLogger.ConfigurationService.DAL;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Session.InProc;
using Nancy.TinyIoc;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Web.modules.common
{
    public class BootstrapperCommon : DefaultNancyBootstrapper
    {
        private static readonly FormsAuthenticationConfiguration _formsAuthConfiguration = new FormsAuthenticationConfiguration
        {
            RedirectUrl = "~/signin"
        };

        public static readonly KeyValuePair<string, string>[] StaticContentsConventions =
        {
            new KeyValuePair<string, string>("views", "/Web/views"),
            new KeyValuePair<string, string>("css", "/Web/css"),
            new KeyValuePair<string, string>("js", "/Web/js"),
            new KeyValuePair<string, string>("assets", "/Web/assets"),
            new KeyValuePair<string, string>("docs", "/Web/docs"),
        };

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            Conventions.ViewLocationConventions.Add((viewName, model, context) => "Web/views/" + viewName);
            pipelines.EnableInProcSessions(InProcSessionsConfiguration.Default);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            //We don't call "base" here to prevent auto-discovery of types/dependencies
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<IUserMapper, UserDAL>(UserDAL.Instance);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Clear();
            foreach (KeyValuePair<string, string> pair in StaticContentsConventions)
            {
                nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(pair.Key, pair.Value));
            }
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(requestContainer, pipelines, context);

            _formsAuthConfiguration.UserMapper = requestContainer.Resolve<IUserMapper>();
            FormsAuthentication.Enable(pipelines, _formsAuthConfiguration);
        }
    }
}
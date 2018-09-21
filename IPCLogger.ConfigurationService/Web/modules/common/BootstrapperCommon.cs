using Nancy;
using Nancy.Conventions;
using System.Collections.Generic;
using IPCLogger.ConfigurationService.DAL;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Session;

namespace IPCLogger.ConfigurationService.Web.modules.common
{
    public class BootstrapperCommon : DefaultNancyBootstrapper
    {
        public static readonly KeyValuePair<string, string>[] StaticContentsConventions =
        {
            new KeyValuePair<string, string>("views", "/Web/views"),
            new KeyValuePair<string, string>("css", "/Web/css"),
            new KeyValuePair<string, string>("js", "/Web/js"),
            new KeyValuePair<string, string>("assets", "/Web/assets"),
            new KeyValuePair<string, string>("fonts", "/Web/fonts")
        };

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            CookieBasedSessions.Enable(pipelines);
            Conventions.ViewLocationConventions.Add((viewName, model, context) => "Web/views/" + viewName);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            //We don't call "base" here to prevent auto-discovery of types/dependencies
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            //base.ConfigureRequestContainer(container, context);

            container.Register<IUserMapper, UserDAL>(UserDAL.Instance);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Clear();
            foreach (KeyValuePair<string, string> pair in StaticContentsConventions)
            {
                nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(pair.Key, pair.Value));
            }
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            //base.RequestStartup(requestContainer, pipelines, context);

            FormsAuthenticationConfiguration formsAuthConfiguration = new FormsAuthenticationConfiguration
            {
                RedirectUrl = "~/signin",
                UserMapper = requestContainer.Resolve<IUserMapper>()
            };
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}

using Nancy;
using Nancy.Conventions;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class BootstrapperMain : DefaultNancyBootstrapper
    {
        public static readonly KeyValuePair<string, string>[] StaticContentsConventions =
        {
            new KeyValuePair<string, string>("html", "/Web/html"),
            new KeyValuePair<string, string>("css", "/Web/css"),
            new KeyValuePair<string, string>("js", "/Web/js"),
            new KeyValuePair<string, string>("images", "/Web/images"),
            new KeyValuePair<string, string>("fonts", "/Web/fonts"),
        };

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Clear();
            foreach (KeyValuePair<string, string> pair in StaticContentsConventions)
            {
                nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(pair.Key, pair.Value));
            }
        }
    }
}

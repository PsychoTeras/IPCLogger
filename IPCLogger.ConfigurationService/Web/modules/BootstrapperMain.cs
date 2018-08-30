using Nancy;
using Nancy.Conventions;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class BootstrapperMain : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Clear();
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("css", "/Web/css"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("js", "/Web/js"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("images", "/Web/images"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("fonts", "/Web/fonts"));
        }
    }
}

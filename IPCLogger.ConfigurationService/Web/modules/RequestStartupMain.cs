using Nancy;
using Nancy.Bootstrapper;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class RequestStartupMain : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            pipelines.OnError += (ctx, ex) => throw ex;
        }
    }
}

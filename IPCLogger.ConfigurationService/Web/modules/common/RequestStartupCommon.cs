using Nancy;
using Nancy.Bootstrapper;

namespace IPCLogger.ConfigurationService.Web.modules.common
{
    public class RequestStartupCommon : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            pipelines.OnError += (ctx, ex) => throw ex;
        }
    }
}

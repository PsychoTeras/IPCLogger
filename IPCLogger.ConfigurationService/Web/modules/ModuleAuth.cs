using Nancy;
using Nancy.Extensions;
using Nancy.Security;

namespace IPCLogger.ConfigurationService.Web.modules
{
    public class ModuleAuth : NancyModule
    {
        public ModuleAuth() : this(string.Empty)
        {
        }

        public ModuleAuth(string modulePath) : base(modulePath)
        {
            //this.RequiresAuthentication();

            //Before += x =>
            //{
            //    if (x.Request.Path == "/signin")
            //    {
            //        return null;
            //    }
            //    return x.CurrentUser != null
            //        ? x.Response
            //        : x.GetRedirect("~/signin");
            //    //Response.AsRedirect("~/signin", Nancy.Responses.RedirectResponse.RedirectType.Temporary);
            //};
        }
    }
}

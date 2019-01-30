using IPCLogger.Core.Resolvers.Base;

namespace IPCLogger.ConfigurationService.CoreServices.Resolvers
{
    public class RSnippet : IBaseResolver
    {
        public object Resolve(string name)
        {
            return new[] { "1", "2", "3" };
        }
    }
}

using IPCLogger.Core.Resolvers.Base;

namespace IPCLogger.ConfigurationService.CoreServices.Resolvers
{
    public class RPatterns : IBaseResolver
    {
        public object Resolve(string name)
        {
            return new[] { "1", "2", "3" };
        }
    }
}

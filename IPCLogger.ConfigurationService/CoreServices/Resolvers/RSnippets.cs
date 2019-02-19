using IPCLogger.Core.Resolvers;
using IPCLogger.Core.Resolvers.Base;

namespace IPCLogger.ConfigurationService.CoreServices.Resolvers
{
    public class RSnippets : BaseResolver
    {

#region Properties

        public override ResolverType Type => ResolverType.Snippets;

#endregion

#region Class methods

        public override object Resolve(object key)
        {
            return null;
        }

#endregion

    }
}

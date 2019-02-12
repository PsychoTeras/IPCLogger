using IPCLogger.Core.Snippets.Base;
using System;

namespace IPCLogger.Core.Snippets.Patterns
{
    using IPCLogger.Core.Patterns;
    using IPCLogger.Core.Patterns.Base;

    internal class SPattern : BaseSnippet
    {

#region Properties

        public override string[] Names
        {
            get { return null; }
        }

#endregion

#region Ctor

        public SPattern() : base(SnippetType.Pattern) { }

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName,
            byte[] data, string text, string @params, PFactory pFactory)
        {
            Pattern pattern;
            return pFactory != null && (pattern = pFactory.Get(callerType, snippetName)) != null
                ? SFactory.Process(callerType, eventType, data, text, pattern, pFactory)
                : null;
        }

#endregion

    }
}

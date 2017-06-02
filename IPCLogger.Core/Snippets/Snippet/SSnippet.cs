using System;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Patterns.Base;
using IPCLogger.Core.Snippets.Base;

namespace IPCLogger.Core.Snippets.Snippet
{
    internal sealed class SSnippet : BaseSnippet
    {

#region Properties

        public override string[] Names
        {
            get { return null; }
        }

#endregion

#region Ctor

        public SSnippet() : base(SnippetType.Snippet) { }

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            Pattern pattern;
            return pFactory != null && (pattern = pFactory.Get(callerType, snippetName)) != null
                ? SFactory.Process(callerType, eventType, text, pattern, pFactory)
                : null;
        }

#endregion

    }
}

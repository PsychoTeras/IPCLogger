using IPCLogger.Patterns;
using IPCLogger.Snippets.Base;
using System;

namespace IPCLogger.Snippets.Pattern
{
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
            Patterns.Base.Pattern pattern;
            return pFactory != null && (pattern = pFactory.Get(callerType, snippetName)) != null
                ? SFactory.Process(callerType, eventType, data, text, pattern, pFactory)
                : null;
        }

#endregion

    }
}

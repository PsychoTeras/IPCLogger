using System;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;
using IPCLogger.Core.Storages;

namespace IPCLogger.Core.Snippets.Storage
{
    internal sealed class SStorage : BaseSnippet
    {

#region Properties

        public override string[] Names
        {
            get { return null; }
        }

#endregion

#region Ctor

        public SStorage() : base(SnippetType.Storage) { }

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            object val = TLS.Get(snippetName);
            return val != null ? val.ToString() : null;
        }

#endregion

    }
}

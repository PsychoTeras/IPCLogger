using System;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;
using IPCLogger.Core.Storages;

namespace IPCLogger.Core.Snippets.AutoKey
{
    internal sealed class AutoKey : BaseSnippet
    {

#region Properties

        public override string[] Names
        {
            get { return null; }
        }

#endregion

#region Ctor

        public AutoKey() : base(SnippetType.AutoKey) { }

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            return AutoKeyS.Pop(snippetName);
        }

#endregion

    }
}

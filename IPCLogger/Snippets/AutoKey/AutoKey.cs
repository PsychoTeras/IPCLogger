using IPCLogger.Patterns;
using IPCLogger.Snippets.Base;
using IPCLogger.Storages;
using System;

namespace IPCLogger.Snippets.AutoKey
{
    internal class AutoKey : BaseSnippet
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
            byte[] data, string text, string @params, PFactory pFactory)
        {
            return AutoKeyS.Pop(snippetName);
        }

#endregion

    }
}

using System;
using System.Threading;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;

namespace IPCLogger.Core.Snippets.Code
{
    internal sealed class SThread : BaseSnippet
    {

#region Properties

        public override string[] Names
        {
            get
            {
                return new[]
                {
                     "thread.id"
                    ,"thread.name"
                };
            }
        }

#endregion

#region Ctor

        public SThread() : base(SnippetType.Code) {}

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            Thread thread = Thread.CurrentThread;
            switch (snippetName)
            {
                case "thread.id":
                    return thread.ManagedThreadId.ToString();
                case "thread.name":
                    return thread.Name;
            }
            return null;
        }

#endregion

    }
}

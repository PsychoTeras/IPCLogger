using System;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;
using System.Diagnostics;

namespace IPCLogger.Core.Snippets.Code
{
    internal sealed class SProcess : BaseSnippet
    {

#region Private fields

        private static readonly Process _curPrc = System.Diagnostics.Process.GetCurrentProcess();

#endregion

#region Properties

        public override string[] Names
        {
            get
            {
                return new[]
                {
                    "process.id",
                    "process.name"
                };
            }
        }

#endregion

#region Ctor

        public SProcess() : base(SnippetType.Code) {}

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            switch (snippetName)
            {
                case "process.id":
                    return _curPrc.Id.ToString();
                case "process.name":
                    return _curPrc.ProcessName;
            }
            return null;
        }

#endregion

    }
}

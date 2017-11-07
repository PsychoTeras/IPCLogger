using System;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;
using IPCLogger.Core.Storages;

namespace IPCLogger.Core.Snippets.Template
{
    sealed class SException : BaseSnippet
    {

#region Properties

        public override string[] Names
        {
            get
            {
                return new[]
                {
                    "exception"
                };
            }
        }

#endregion

#region Ctor

        public SException() : base(SnippetType.Snippet) { }

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName,
            byte[] data, string text, string @params, PFactory pFactory)
        {
            LSObject lsObj = LS.Peek();
            if (lsObj == null || lsObj.Exception == null) return null;

            switch (@params)
            {
                case "message":
                    return lsObj.Exception.Message;
                case "stack":
                    return lsObj.Exception.StackTrace;
                default:
                    return lsObj.Exception.ToString();
            }
        }

#endregion

    }
}

using System;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;
using IPCLogger.Core.Storages;

namespace IPCLogger.Core.Snippets.Code
{
    internal sealed class SException : BaseSnippet
    {

#region Properties

        public override string[] Names
        {
            get
            {
                return new[]
                {
                     "exception"
                    ,"exception.Message"
                    ,"exception.StackTrace"
                };
            }
        }

#endregion

#region Ctor

        public SException() : base(SnippetType.Code) { }

#endregion

#region Class methods

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            LSObject lsObj = LS.Peek();
            if (lsObj == null || lsObj.Exception == null) return null;

            switch (snippetName)
            {
                case "exception":
                    return lsObj.Exception.ToString();
                case "exception.Message":
                    return lsObj.Exception.Message;
                case "exception.StackTrace":
                    return lsObj.Exception.StackTrace;
                default:
                    return null;
            }
        }

#endregion

    }
}

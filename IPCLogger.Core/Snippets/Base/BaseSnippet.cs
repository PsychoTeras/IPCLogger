using System;
using System.Collections.Generic;
using IPCLogger.Core.Patterns;

namespace IPCLogger.Core.Snippets.Base
{
    public abstract class BaseSnippet
    {

#region Static fields

        internal static readonly Dictionary<SnippetType, char> SnippetMarks = 
            new Dictionary<SnippetType, char>
            {
                {SnippetType.Template, '$'},
                {SnippetType.Snippet, '%'},
                {SnippetType.Storage, '#'},
                {SnippetType.AutoKey, '^'}
            };

        internal static readonly Dictionary<char, SnippetType> SnippetTypes =
            new Dictionary<char, SnippetType>(SnippetMarks.Count);

        internal static readonly string DefNullValueString = "<NULL>";

#endregion

#region Protected fields

        protected char Mark { get; }


#endregion

#region Properties

        public SnippetType Type { get; protected set; }

        public abstract string[] Names { get; }

#endregion

#region Ctor

        static BaseSnippet()
        {
            foreach (KeyValuePair<SnippetType, char> pair in SnippetMarks)
            {
                SnippetTypes.Add(pair.Value, pair.Key);
            }
        }

        protected BaseSnippet(SnippetType type)
        {
            Type = type;
            Mark = SnippetMarks[type];
        }

#endregion

#region Class methods

        public abstract string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory);

#endregion

    }
}

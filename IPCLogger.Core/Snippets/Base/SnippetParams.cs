using System;
using System.Collections.Generic;

namespace IPCLogger.Core.Snippets.Base
{
    public sealed class SnippetParams : Dictionary<string, string>
    {
        private const char CHAR_PARAM_SPLITTER = ';';
        private const char CHAR_PARAM_VAL_SPLITTER = '=';

        public bool HasValue(string key, bool def)
        {
            return ContainsKey(key) || def;
        }

        public T GetValue<T>(string key, T def)
        {
            string value;
            if (TryGetValue(key, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return def;
        }

        public static SnippetParams Parse(string @params)
        {
            SnippetParams dictParams = new SnippetParams();
            if (@params.Length != 0)
            {
                string[] paramsList = @params.Split(new[] {CHAR_PARAM_SPLITTER},
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (string paramVal in paramsList)
                {
                    string[] paramKv = paramVal.Split(CHAR_PARAM_VAL_SPLITTER);
                    if (!dictParams.ContainsKey(paramKv[0]))
                    {
                        dictParams.Add(paramKv[0], paramKv.Length == 2 ? paramKv[1] : string.Empty);
                    }
                }
            }
            return dictParams;
        }
    }
}

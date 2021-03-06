﻿using IPCLogger.Caches;
using IPCLogger.Patterns;
using IPCLogger.Snippets.Base;
using IPCLogger.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace IPCLogger.Snippets
{
    internal static class SFactory
    {

#region Static fields

        private static readonly Dictionary<SnippetType, BaseSnippet> _customBodySnippets =
            new Dictionary<SnippetType, BaseSnippet>();

        private static readonly Dictionary<SnippetType, Dictionary<string, BaseSnippet>> _snippets =
            new Dictionary<SnippetType, Dictionary<string, BaseSnippet>>();

        private static readonly Dictionary<int, SnippetsCache> _snippetsCache =
            new Dictionary<int, SnippetsCache>();

        private static readonly Dictionary<SnippetType, List<BaseSnippet>> _snippetsList =
            new Dictionary<SnippetType, List<BaseSnippet>>();

        private static Regex _regexParseString;

#endregion

#region Static ctor

        static SFactory()
        {
            InitializeSnippetsList();
            InitializeParseStringRegex();
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoadEventHandler;
        }

#endregion

#region Private methods

        private static IEnumerable<Type> GetDeclaredResolvers(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseSnippet)));
        }

        private static void AssemblyLoadEventHandler(object sender, AssemblyLoadEventArgs args)
        {
            try
            {
                IEnumerable<Type> snippetTypes = GetDeclaredResolvers(args.LoadedAssembly);
                AppendSnippets(snippetTypes);
                InitializeParseStringRegex();
            }
            catch { }
        }

        private static void AppendSnippets(IEnumerable<Type> snippetTypes)
        {
            lock (_snippetsCache)
            {
                foreach (Type baseSnippetType in snippetTypes)
                {
                    if (Activator.CreateInstance(baseSnippetType) is BaseSnippet snippet)
                    {
                        SnippetType snippetType = snippet.Type;

                        if (!_snippetsList.TryGetValue(snippetType, out var snippets))
                        {
                            snippets = new List<BaseSnippet>();
                            _snippetsList.Add(snippetType, snippets);
                        }
                        snippets.Add(snippet);

                        if (snippet.Names == null)
                        {
                            if (!_customBodySnippets.ContainsKey(snippetType))
                            {
                                _customBodySnippets.Add(snippetType, snippet);
                            }
                            else
                            {
                                string msg = $"Duplicate custom-body snippet, type '{snippetType}'";
                                throw new Exception(msg);
                            }
                        }
                        else
                        {
                            if (!_snippets.ContainsKey(snippetType))
                            {
                                _snippets.Add(snippetType, new Dictionary<string, BaseSnippet>());
                            }
                            Dictionary<string, BaseSnippet> snippetsList = _snippets[snippetType];
                            foreach (string name in snippet.Names)
                            {
                                if (!snippetsList.ContainsKey(name))
                                {
                                    snippetsList.Add(name, snippet);
                                }
                                else
                                {
                                    string msg = $"Duplicate snippet, name '{name}', type '{snippetType}'";
                                    throw new Exception(msg);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void InitializeSnippetsList()
        {
            List<Type> types = new List<Type>();
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.
                GetAssemblies().
                Where(a => !a.FullName.StartsWith("System."));
            foreach (Assembly assembly in assemblies)
            {
                try
                {                    
                    types.AddRange(GetDeclaredResolvers(assembly));
                }
                catch { }
            }

            AppendSnippets(types);
        }

        private static void InitializeParseStringRegex()
        {
            StringBuilder typeMarks = new StringBuilder();
            StringBuilder snippetNames = new StringBuilder();
            StringBuilder regexPattern = new StringBuilder();

            //Create regexp for regular snippets
            List<string> namesList = new List<string>();
            foreach (KeyValuePair<SnippetType, Dictionary<string, BaseSnippet>> dict in _snippets)
            {
                typeMarks.Append(BaseSnippet.SnippetMarks[dict.Key]);
                namesList.AddRange(dict.Value.Select(s => Regex.Escape(s.Key)));
            }
            namesList.Sort();

            foreach (string snippet in namesList)
            {
                snippetNames.AppendFormat("{0}{1}", snippetNames.Length > 0 ? "|" : string.Empty, snippet);
            }
            regexPattern.AppendFormat(@"({{(?<T>[{0}])(?<N>{1})(:(?<P>((?!}}).)*))*}})", typeMarks, snippetNames);

            //Create regexp for custom-body snippets
            typeMarks = new StringBuilder();
            foreach (KeyValuePair<SnippetType, BaseSnippet> dict in _customBodySnippets)
            {
                typeMarks.Append(BaseSnippet.SnippetMarks[dict.Key]);
            }
            regexPattern.AppendFormat(@"|({{(?<T>[{0}])(?<N>.*?)(:(?<P>((?!}}).)*))*}})", typeMarks);

            //Compile regex
            RegexOptions regexOpt = RegexOptions.ExplicitCapture;
            _regexParseString = new Regex(regexPattern.ToString(), regexOpt);
        }

        private static string ProcessToCache(Type callerType, Enum eventType, byte[] data, string text, 
            string pattern, PFactory pFactory, SnippetsCache record)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return text;
            }

            MatchCollection matches = _regexParseString.Matches(pattern);
            if (matches.Count == 0)
            {
                return pattern;
            }

            int startIdx;
            Match prewMatch = null;
            StringBuilder result = new StringBuilder();
            string contentPart;

            foreach (Match match in matches)
            {
                if (record != null && prewMatch != null) record = record.CreateNext();

                if (prewMatch == null && match.Index > 0)
                {
                    result.Append(contentPart = pattern.Substring(0, match.Index));
                    if (record != null) record.PreContent = contentPart;
                }
                else if (prewMatch != null && (startIdx = prewMatch.Index + prewMatch.Length) < match.Index)
                {
                    result.Append(contentPart = pattern.Substring(startIdx, match.Index - startIdx));
                    if (record != null) record.PreContent = contentPart;
                }

                char typeMark = match.Groups["T"].Value[0];
                SnippetType snippetType = BaseSnippet.SnippetTypes[typeMark];
                string name = match.Groups["N"].Value;

                BaseSnippet snippet;
                Dictionary<string, BaseSnippet> snippets;
                bool snippetHasBeenFound =
                    _snippets.TryGetValue(snippetType, out snippets) && snippets.TryGetValue(name, out snippet) ||
                    _customBodySnippets.TryGetValue(snippetType, out snippet);
                if (snippetHasBeenFound)
                {
                    string @params = match.Groups["P"].Value;

                    if (record != null)
                    {
                        record.Snippet = snippet;
                        record.Name = name;
                        record.Params = @params;
                        ProcessSnippetsCacheRecord(record);
                    }

                    string value = snippet.Process(callerType, eventType, name, data, text, @params, pFactory);
                    if (!string.IsNullOrEmpty(value))
                    {
                        result.Append(value);
                    }
                }

                prewMatch = match;
            }

            if (prewMatch != null && (startIdx = prewMatch.Index + prewMatch.Length) < pattern.Length)
            {
                result.Append(contentPart = pattern.Substring(startIdx, pattern.Length - startIdx));
                if (record != null) record.CreateNext().PreContent = contentPart;
            }

            return result.ToString();
        }

        private static void ProcessSnippetsCacheRecord(SnippetsCache record)
        {
            switch (record.Snippet.Type)
            {
                case SnippetType.AutoKey:
                {
                    SnippetParams sParams = SnippetParams.Parse(record.Params);
                    int init;
                    if (!int.TryParse(sParams.GetValue("init", string.Empty), out init))
                    {
                        init = 1;
                    }
                    int increment;
                    if (!int.TryParse(sParams.GetValue("increment", string.Empty), out increment))
                    {
                        increment = 1;
                    }
                    string format = sParams.GetValue<string>("format", null);
                    if (string.IsNullOrWhiteSpace(format))
                    {
                        format = null;
                    }
                    AutoKeyS.Add(record.Name, init, increment, format);
                    break;
                }
            }
        }

        private static string Process(Type callerType, Enum eventType, byte[] data, string text, string pattern, 
            int patternId, PFactory pFactory)
        {
            SnippetsCache record = null;
            if (patternId != -1 && !_snippetsCache.TryGetValue(patternId, out record))
            {
                lock (_snippetsCache)
                {
                    if (!_snippetsCache.TryGetValue(patternId, out record))
                    {
                        record = new SnippetsCache();
                        string result = ProcessToCache(callerType, eventType, data, text, pattern, pFactory, record);
                        _snippetsCache.Add(patternId, record);
                        return result;
                    }
                }
            }
            
            return record == null
                ? ProcessToCache(callerType, eventType, data, text, pattern, pFactory, null)
                : record.Process(callerType, eventType, data, text, pFactory);
        }

#endregion

#region Public methods

        public static string Process(string pattern, PFactory pFactory)
        {
            return Process(null, null, null, null, pattern, -1, null);
        }

        public static string Process(Type callerType, Enum eventType, byte[] data, string text, 
            string pattern, PFactory pFactory)
        {
            return Process(callerType, eventType, data, text, pattern, -1, pFactory);
        }

        internal static string Process(Type callerType, Enum eventType, byte[] data, string text, 
            Patterns.Base.Pattern pattern, PFactory pFactory)
        {
            return Process(callerType, eventType, data, text, pattern.Content, pattern.Id, pFactory);
        }

#endregion

    }
}

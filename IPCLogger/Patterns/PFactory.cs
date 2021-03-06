﻿using IPCLogger.Common;
using IPCLogger.Patterns.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace IPCLogger.Patterns
{
    // ReSharper disable PossibleNullReferenceException
    public sealed class PFactory
    {

#region Definitions

    class ContentComparer : IComparer<string>
    {
        private int PosOrMax(int val)
        {
            return val >= 0 ? val : int.MaxValue;
        }

        public int Compare(string a, string b)
        {
            int maskAIdx = 0, maskBIdx = 0;
            do
            {
                maskAIdx = a.IndexOf(Constants.ApplicableForAllMark, maskAIdx + 1);
                maskBIdx = b.IndexOf(Constants.ApplicableForAllMark, maskBIdx + 1);
                if (maskAIdx != maskBIdx)
                {
                    return PosOrMax(maskBIdx).CompareTo(PosOrMax(maskAIdx));
                }
            } while (maskAIdx == maskBIdx && maskAIdx != -1);
            return b.CompareTo(a);
        }
    }

#endregion

#region Private fields

        private bool _isGenericPatternAvailable;
        
        private Dictionary<string, Dictionary<Type, Pattern>> _compiledTypedPatterns;
        private Dictionary<string, Pattern> _compiledUntypedPatterns;
        private Dictionary<string, RawPatterns> _rawPatterns;
        private HashSet<string> _missingPatterns;

#endregion

#region Static properties

        public static PFactory Instance { get; private set; }

        internal static Dictionary<string, string> PropertyAttributes { get; } = 
            new Dictionary<string, string>
        {
            {"Description", "description"},
            {"Events", "events"},
            {"ImmediateFlush", "immediate-flush"},
            {"ApplicableFor", "applicable-for"}
        };

#endregion

#region Ctor

        static PFactory()
        {
            Instance = new PFactory();
        }

        private PFactory()
        {
            ReinitializeDictionaries();
        }

#endregion

#region Static methods

        public static PFactory CreateNewInstance()
        {
            return new PFactory();
        }

        internal static List<DeclaredPattern> GetDeclaredPatterns(XmlDocument xmlCfg)
        {
            List<DeclaredPattern> patterns = new List<DeclaredPattern>();

            string patternsXPath = $"{Constants.RootPatternsCfgPath}/*";
            XmlNodeList cfgNodes = xmlCfg.SelectNodes(patternsXPath);
            foreach (XmlNode cfgNode in cfgNodes.OfType<XmlNode>())
            {
                DeclaredPattern declaredPattern = new DeclaredPattern(cfgNode);
                patterns.Add(declaredPattern);
            }

            return patterns;
        }

        internal static List<PatternContent> GetPatternContent(XmlNode cfgNode)
        {
            string patternContentXPath = "./Content";
            List<PatternContent> content = new List<PatternContent>();
            IEnumerable<XmlNode> contentNodes = cfgNode.
                SelectNodes(patternContentXPath).
                Cast<XmlNode>();
            foreach (XmlNode node in contentNodes)
            {
                content.Add(new PatternContent(node));
            }
            return content;
        }

#endregion

#region Public methods

       internal Pattern Get(Type callerType, string patternName)
        {
            if (patternName.Length == 0 || _missingPatterns.Contains(patternName)) return null;

            Pattern pattern;
            RawPatterns rawPattern;
            Dictionary<Type, Pattern> compiledTyped;

            string curPatternName = patternName;
            bool triedGetGenericPattern = false;

get_generic_pattern:

            bool hasRawPattern = _rawPatterns.TryGetValue(curPatternName, out rawPattern);
            bool hasCompiledTyped = _compiledTypedPatterns.TryGetValue(curPatternName, out compiledTyped);

            if (!hasCompiledTyped && !hasRawPattern &&
                _compiledUntypedPatterns.TryGetValue(curPatternName, out pattern))
            {
                return pattern;
            }

            if (callerType == null) return null;

            if (hasCompiledTyped && compiledTyped.TryGetValue(callerType, out pattern))
            {
                return pattern;
            }

            if (hasRawPattern)
            {
                string typeName = callerType.FullName;
                if (rawPattern.Strong.TryGetValue(typeName, out pattern))
                {
                    return AppendCompiledTypedPattern(patternName, callerType, pattern);
                }

                foreach (Regex regexMask in rawPattern.Masked.Keys)
                {
                    if (regexMask.IsMatch(typeName))
                    {
                        pattern = rawPattern.Masked[regexMask];
                        return AppendCompiledTypedPattern(patternName, callerType, pattern);
                    }
                }
            }

            if (_compiledUntypedPatterns.TryGetValue(curPatternName, out pattern))
            {
                return AppendCompiledTypedPattern(patternName, callerType, pattern);
            }

            if (_isGenericPatternAvailable && !triedGetGenericPattern)
            {
                triedGetGenericPattern = true;
                curPatternName = Constants.ApplicableForAllMark;
                goto get_generic_pattern;
            }

            AppendMissingPattern(patternName);

            return null;
        }

        public void Setup(string configurationFile)
        {
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                string msg = $"Configuration file '{configurationFile}' is invalid or doesn't exist";
                throw new ArgumentException(msg);
            }

            XmlDocument xmlCfg = new XmlDocument();
            xmlCfg.Load(configurationFile);
            Setup(xmlCfg);
        }

        public void Setup(XmlDocument xmlCfg)
        {
            string patternXPath = $"{Constants.RootPatternsCfgPath}/Pattern";
            XmlNodeList patternNodes = xmlCfg.SelectNodes(patternXPath);
            if (patternNodes == null)
            {
                ReinitializeDictionaries();
                return;
            }

            Setup(patternNodes);
        }

#endregion

#region Private methods

        private void ReinitializeDictionaries()
        {
            _isGenericPatternAvailable = false;
            _compiledTypedPatterns = new Dictionary<string, Dictionary<Type, Pattern>>();
            _compiledUntypedPatterns = new Dictionary<string, Pattern>();
            _rawPatterns = new Dictionary<string, RawPatterns>();
            _missingPatterns = new HashSet<string>();
        }

        private Pattern AppendCompiledTypedPattern(string patternName, Type type, Pattern pattern)
        {
            lock (_compiledTypedPatterns)
            {
                Dictionary<Type, Pattern> compiledTyped;
                if (!_compiledTypedPatterns.TryGetValue(patternName, out compiledTyped))
                {
                    compiledTyped = new Dictionary<Type, Pattern>();
                    _compiledTypedPatterns.Add(patternName, compiledTyped);
                }
                if (!compiledTyped.ContainsKey(type))
                {
                    compiledTyped.Add(type, pattern);
                }
            }
            return pattern;
        }

        private void AppendMissingPattern(string patternName)
        {
            lock (_missingPatterns)
            {
                if (!_missingPatterns.Contains(patternName))
                {
                    _missingPatterns.Add(patternName);
                }
            }
        }

        private void Setup(XmlNodeList patternNodes)
        {
            var tmpRawPatterns = new Dictionary<string, RawPatterns>();
            var tmpCompiledUntypedPatterns = new Dictionary<string, Pattern>();

            string patternContentXPath = "./Content";
            foreach (XmlNode patternNode in patternNodes)
            {
                IEnumerable<XmlNode> contentNodes = patternNode.
                    SelectNodes(patternContentXPath).
                    Cast<XmlNode>().
                    Where(n => !string.IsNullOrWhiteSpace(n.InnerText));
                if (!contentNodes.Any())
                {
                    continue;
                }

                XmlAttribute aEvents = patternNode.Attributes["events"];
                string events = aEvents?.Value;
                if (string.IsNullOrEmpty(events))
                {
                    events = Constants.ApplicableForAllMark;
                    _isGenericPatternAvailable = true;
                }

                XmlAttribute aImmediateFlush = patternNode.Attributes["immediate-flush"];
                bool immediateFlush = 
                    aImmediateFlush != null && 
                    bool.TryParse(aImmediateFlush.Value, out immediateFlush) && 
                    immediateFlush;

                foreach (string eventName in events.Split(Constants.Splitter).Select(s => s.Trim()))
                {
                    if (string.IsNullOrEmpty(eventName)) continue;

                    bool newPattern = !tmpRawPatterns.ContainsKey(eventName);
                    RawPatterns rawPattern = newPattern
                        ? new RawPatterns(eventName)
                        : tmpRawPatterns[eventName];

                    SetupContent(rawPattern, contentNodes, tmpCompiledUntypedPatterns, immediateFlush);
                    if (newPattern && !rawPattern.Empty) //!!! Doesn't work in case when default Pattern has a content record
                                                         //whose applicable-for is "IPCLogger.TestService.Common.*"
                    {
                        tmpRawPatterns.Add(eventName, rawPattern);
                    }
                }
            }

            OrderContent(_rawPatterns = tmpRawPatterns);
            
            _compiledUntypedPatterns = tmpCompiledUntypedPatterns;
            _compiledTypedPatterns = new Dictionary<string, Dictionary<Type, Pattern>>();
            _missingPatterns = new HashSet<string>();
        }

        private void SetupContent(RawPatterns rawPattern, IEnumerable<XmlNode> contentNodes,
            Dictionary<string, Pattern> compiledUntypedPatterns, bool immediateFlush)
        {
            foreach (XmlNode contentNode in contentNodes)
            {
                string content = contentNode.InnerText.Trim();

                XmlAttribute aApplicableFor = contentNode.Attributes["applicable-for"];
                IEnumerable<string> applicableFor = aApplicableFor != null
                    ? aApplicableFor.Value.
                        Split(new[] {Constants.Splitter}, StringSplitOptions.RemoveEmptyEntries).
                        Distinct()
                    : new[] {Constants.ApplicableForAllMark};

                foreach (string classMask in applicableFor)
                {
                    if (ReferenceEquals(classMask, Constants.ApplicableForAllMark) &&
                        !compiledUntypedPatterns.ContainsKey(rawPattern.EventName))
                    {
                        compiledUntypedPatterns.Add(rawPattern.EventName, new Pattern(content, immediateFlush));
                        continue;
                    }
                    
                    if (!ReferenceEquals(classMask, Constants.ApplicableForAllMark) &&
                        !rawPattern.Strong.ContainsKey(classMask))
                    {
                        if (classMask.Contains(Constants.ApplicableForAllMark))
                        {
                            string regexClassMask = Regex.Escape(classMask).Replace(@"\*", ".*?");
                            Regex regexMask = new Regex(regexClassMask, RegexOptions.Compiled);
                            rawPattern.Masked.Add(regexMask, new Pattern(content, immediateFlush));
                        }
                        else
                        {
                            rawPattern.Strong.Add(classMask, new Pattern(content, immediateFlush));
                        }
                        continue;
                    }

                    string msg = $"Duplicate applicable-for '{classMask}' for pattern '{rawPattern.EventName}'";
                    throw new Exception(msg);
                }
            }
        }

        private void OrderContent(Dictionary<string, RawPatterns> rawPatterns)
        {
            foreach (RawPatterns rawPattern in rawPatterns.Values)
            {
                rawPattern.Strong = rawPattern.Strong.
                    OrderBy(s => s.Key, new ContentComparer()).
                    ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

#endregion

    }

    internal class DeclaredPattern
    {
        public string Description;
        public string Events;
        public bool ImmediateFlush;
        public XmlNode CfgNode;

        public DeclaredPattern(XmlNode cfgNode)
        {
            CfgNode = cfgNode;
            Description = cfgNode.Attributes["description"]?.Value;
            Events = cfgNode.Attributes["events"]?.Value;
            XmlAttribute aImmediateFlush = cfgNode.Attributes["immediate-flush"];
            ImmediateFlush =
                aImmediateFlush != null &&
                bool.TryParse(aImmediateFlush.Value, out ImmediateFlush) &&
                ImmediateFlush;
        }
    }

    internal class PatternContent
    {
        public string ApplicableFor;
        public string Content;
        public XmlNode CfgNode;

        public PatternContent(XmlNode cfgNode)
        {
            CfgNode = cfgNode;
            ApplicableFor = cfgNode.Attributes["applicable-for"]?.Value;
            Content = cfgNode.InnerText.Trim();
        }
    }
    // ReSharper restore PossibleNullReferenceException
}

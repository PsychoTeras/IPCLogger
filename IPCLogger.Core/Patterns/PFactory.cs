using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns.Base;
using System.Collections.Generic;

namespace IPCLogger.Core.Patterns
{
    // ReSharper disable PossibleNullReferenceException
    public sealed class PFactory
    {

#region Constants

        private const string ROOT_PATTERNS_CFG_PATH   = Constants.ROOT_LOGGER_CFG_PATH + "/Patterns/Pattern";
        private const string PATTERN_CONTENT_CFG_PATH = "./Content";


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

#endregion

#region Class methods

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

        public Pattern Get(Type callerType, string patternName)
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

        public void Setup()
        {
            Setup(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        public void Setup(string configurationFile)
        {
            if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile))
            {
                string msg = string.Format("Configuration file '{0}' is invalid or doesn't exists",
                    configurationFile);
                throw new ArgumentException(msg);
            }

            XmlDocument xmlCfg = new XmlDocument();
            xmlCfg.Load(configurationFile);
            Setup(xmlCfg);
        }

        public void Setup(XmlDocument xmlCfg)
        {
            XmlNodeList patternNodes = xmlCfg.SelectNodes(ROOT_PATTERNS_CFG_PATH);
            if (patternNodes == null)
            {
                ReinitializeDictionaries();
                return;
            }

            Setup(patternNodes);
        }

        private void Setup(XmlNodeList patternNodes)
        {
            var tmpRawPatterns = new Dictionary<string, RawPatterns>();
            var tmpCompiledUntypedPatterns = new Dictionary<string, Pattern>();

            foreach (XmlNode patternNode in patternNodes)
            {
                XmlNodeList contentNodes = patternNode.SelectNodes(PATTERN_CONTENT_CFG_PATH);
                XmlNode[] orderedNodes = contentNodes.
                    Cast<XmlNode>().
                    Where(n => !string.IsNullOrEmpty(n.InnerText)).
                    ToArray();
                if (!orderedNodes.Any())
                {
                    continue;
                }

                XmlAttribute aEvents = patternNode.Attributes["events"];
                string events = aEvents != null ? aEvents.Value : null;
                if (string.IsNullOrEmpty(events))
                {
                    events = Constants.ApplicableForAllMark;
                    _isGenericPatternAvailable = true;
                }

                foreach (string s in events.Split(Constants.Splitter))
                {
                    string @event = s.Trim();
                    if (string.IsNullOrEmpty(@event)) continue;

                    bool newPattern = !tmpRawPatterns.ContainsKey(@event);
                    RawPatterns rawPattern = newPattern
                        ? new RawPatterns(@event)
                        : tmpRawPatterns[@event];

                    SetupContent(rawPattern, orderedNodes, tmpCompiledUntypedPatterns);
                    if (newPattern && !rawPattern.Empty)
                    {
                        tmpRawPatterns.Add(@event, rawPattern);
                    }
                }
            }

            OrderContent(_rawPatterns = tmpRawPatterns);
            
            _compiledUntypedPatterns = tmpCompiledUntypedPatterns;
            _compiledTypedPatterns = new Dictionary<string, Dictionary<Type, Pattern>>();
            _missingPatterns = new HashSet<string>();
        }

        private void SetupContent(RawPatterns rawPattern, XmlNode[] orderedNodes,
            Dictionary<string, Pattern> compiledUntypedPatterns)
        {
            foreach (XmlNode contentNode in orderedNodes)
            {
                string content = contentNode.InnerText.Trim().
                    Replace("\\r\\n", "\r\n").
                    Replace("\\r", "\r").
                    Replace("\\n", "\n");

                XmlAttribute aApplicableFor = contentNode.Attributes["applicable-for"];
                string[] applicableFor = aApplicableFor != null
                    ? aApplicableFor.Value.
                        Split(new[] { Constants.Splitter }, StringSplitOptions.RemoveEmptyEntries).
                        Distinct().
                        ToArray()
                    : new[] { Constants.ApplicableForAllMark };

                foreach (string classMask in applicableFor)
                {
                    if (ReferenceEquals(classMask, Constants.ApplicableForAllMark) &&
                        !compiledUntypedPatterns.ContainsKey(rawPattern.Event))
                    {
                        compiledUntypedPatterns.Add(rawPattern.Event, new Pattern(content));
                        continue;
                    }
                    
                    if (!ReferenceEquals(classMask, Constants.ApplicableForAllMark) &&
                        !rawPattern.Strong.ContainsKey(classMask))
                    {
                        if (classMask.Contains(Constants.ApplicableForAllMark))
                        {
                            string regexClassMask = Regex.Escape(classMask).Replace(@"\*", ".*?");
                            Regex regexMask = new Regex(regexClassMask, RegexOptions.Compiled);
                            rawPattern.Masked.Add(regexMask, new Pattern(content));
                        }
                        else
                        {
                            rawPattern.Strong.Add(classMask, new Pattern(content));
                        }
                        continue;
                    }

                    string msg = string.Format("Duplicate applicable-for '{0}' for pattern '{1}'",
                        classMask, rawPattern.Event);
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
    // ReSharper restore PossibleNullReferenceException

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
}

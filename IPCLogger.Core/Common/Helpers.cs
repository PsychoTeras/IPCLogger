using IPCLogger.Core.Loggers.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace IPCLogger.Core.Common
{
    internal static class Helpers
    {
        private const long ONE_KBYTE = 1024;
        private const long ONE_MBYTE = ONE_KBYTE * ONE_KBYTE;
        private const long ONE_GBYTE = ONE_MBYTE * ONE_MBYTE;

        [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PathFileExists(string path);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        private static readonly Regex _regexBytesString = new Regex(@"\s*(?<SIZE>\d+)+[ ]*(?<UNIT>[a-z]+)*",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        private static readonly Regex _regexTimeString = new Regex(@"(?<COUNT>\d+)+[ ]*(?<UNIT>[a-z]+)+",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        public static bool ByteArrayEquals(byte[] b1, byte[] b2)
        {
            return b1 == null && b2 == null ||
                   b1 != null && b2 != null &&
                   b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        public static string CalculateUniqueId(string name, string type, string nameSpace)
        {
            return $"^{name}${type}%{type}^";
        }

        public static bool IsAssignableTo(Type givenType, Type genericType)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType &&
                givenType.UnderlyingSystemType != genericType)
            {
                return true;
            }

            Type baseType = givenType.BaseType;
            return baseType != null && IsAssignableTo(baseType, genericType);
        }

        public static int FindCallerStackLevel(StackTrace stack)
        {
            int frame, cnt = stack.FrameCount - 1;
            for (frame = cnt; frame >= 0; frame--)
            {
                Type t = stack.GetFrame(frame).GetMethod().DeclaringType;
                if (t != null && t.IsAssignableFrom(typeof(BaseLogger<>)))
                {
                    break;
                }
            }

            return frame + 1;
        }

        public static long BytesStringToSize(string sBytes)
        {
            if (string.IsNullOrEmpty(sBytes))
            {
                string msg = "Value cannot be empty";
                throw new Exception(msg);
            }

            if (long.TryParse(sBytes, out long value))
            {
                return value;
            }

            MatchCollection matches = _regexBytesString.Matches(sBytes);
            if (matches.Count == 0)
            {
                string msg = "Size string is invalid";
                throw new Exception(msg);
            }

            foreach (Match match in matches)
            {
                string sSize = match.Groups["SIZE"].Value;
                if (sSize == string.Empty)
                {
                    string msg = "Value is not defined";
                    throw new Exception(msg);
                }

                if (!long.TryParse(sSize, out long size))
                {
                    string msg = $"Value '{sSize}' is invalid";
                    throw new Exception(msg);
                }

                long multiplier = 1;
                string sUnit = match.Groups["UNIT"].Value.ToUpper();
                if (sUnit != string.Empty)
                {
                    switch (sUnit)
                    {
                        case "B":
                            break;
                        case "KB":
                            multiplier = ONE_KBYTE;
                            break;
                        case "MB":
                            multiplier = ONE_MBYTE;
                            break;
                        case "GB":
                            multiplier = ONE_GBYTE;
                            break;
                        default:
                            string msg = $"Unit '{sUnit}' is invalid. Use B, KB, MB, GB instead (case-insensitive)";
                            throw new Exception(msg);
                    }
                }

                value += size * multiplier;
            }

            return value;
        }

        public static string SizeToBytesString(long size)
        {
            string sSize = string.Empty;

            double sizeLeft = size;
            while (sizeLeft >= ONE_KBYTE)
            {
                long value = 0;
                if (sizeLeft >= ONE_GBYTE)
                {
                    value = (long) Math.Floor(sizeLeft / ONE_GBYTE);
                    sSize += $"{value}GB ";
                    sizeLeft -= value * ONE_GBYTE;
                }
                else if (sizeLeft >= ONE_MBYTE)
                {
                    value = (long) Math.Floor(sizeLeft / ONE_MBYTE);
                    sSize += $"{value}MB ";
                    sizeLeft -= value * ONE_MBYTE;
                }
                else if (sizeLeft >= ONE_KBYTE)
                {
                    value = (long) Math.Floor(sizeLeft / ONE_KBYTE);
                    sSize += $"{value}KB ";
                    sizeLeft -= value * ONE_KBYTE;
                }
            }

            if (sSize == string.Empty)
            {
                sSize = $"{sizeLeft}B";
            }
            else if (Math.Abs(sizeLeft) > float.Epsilon)
            {
                sSize += $"{sizeLeft}B";
            }

            return sSize.TrimEnd();
        }

        public static TimeSpan TimeStringToTimeSpan(string sTime)
        {
            if (string.IsNullOrEmpty(sTime))
            {
                string msg = "Time string cannot be empty";
                throw new Exception(msg);
            }

            if (TimeSpan.TryParse(sTime, out TimeSpan timeSpan))
            {
                return timeSpan;
            }

            MatchCollection matches = _regexTimeString.Matches(sTime);
            if (matches.Count == 0)
            {
                string msg = "Time string is invalid";
                throw new Exception(msg);
            }

            int days = 0, hours = 0, minutes = 0, seconds = 0;
            foreach (Match match in matches)
            {
                string sCount = match.Groups["COUNT"].Value;
                if (!int.TryParse(sCount, out int count))
                {
                    string msg = $"Value '{sCount}' is invalid";
                    throw new Exception(msg);
                }

                string sUnit = match.Groups["UNIT"].Value.ToLower();
                switch (sUnit)
                {
                    case "d":
                        days += count;
                        break;
                    case "h":
                        hours += count;
                        break;
                    case "m":
                        minutes += count;
                        break;
                    case "s":
                        seconds += count;
                        break;
                    default:
                        string msg =
                            $"Unit '{sUnit}' is invalid. Use d (=days), h (=hours), m (=minutes), s (=seconds) instead (case-insensitive)";
                        throw new Exception(msg);
                }
            }

            return new TimeSpan(days, hours, minutes, seconds);
        }

        public static string TimeSpanToTimeString(TimeSpan timeSpan)
        {
            string sTime = string.Empty;

            if (timeSpan.Days > 0)
            {
                sTime += $"{timeSpan.Days}d ";
            }

            if (timeSpan.Hours > 0)
            {
                sTime += $"{timeSpan.Hours}h ";
            }

            if (timeSpan.Minutes > 0)
            {
                sTime += $"{timeSpan.Minutes}m ";
            }

            if (timeSpan.Seconds > 0)
            {
                sTime += $"{timeSpan.Seconds}s";
            }

            return sTime.TrimEnd();
        }

        public static string ByteArrayToString(byte[] bytes, int lineLength)
        {
            if (bytes == null || bytes.Length == 0) return null;

            int curCharsCnt = 0;
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
                if (++curCharsCnt >= lineLength)
                {
                    curCharsCnt = 0;
                    hex.AppendLine();
                }
            }

            return hex.ToString();
        }

        public static object StringToStringList(Type dataType, string sValue, bool removeEmpty, char splitter)
        {
            if (sValue == null)
            {
                string msg = "Comma-separated string cannot be null";
                throw new Exception(msg);
            }

            object result = Activator.CreateInstance(dataType);

            if (sValue == string.Empty)
            {
                return result;
            }

            StringSplitOptions sso = removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            string[] value = sValue.Split(new[] { splitter }, sso).Select(s => s.Trim()).ToArray();

            if (value.Any())
            {
                switch (result)
                {
                    case HashSet<string> hsString:
                        Array.ForEach(value, s => hsString.Add(s));
                        break;
                    case List<string> lsString:
                        Array.ForEach(value, s => lsString.Add(s));
                        break;
                    case string[] _:
                        result = value;
                        break;
                    default:
                        string msg = $"Type '{dataType.Name}' is not supported";
                        throw new Exception(msg);
                }
            }

            return result;
        }

        public static string StringListToString(IEnumerable<string> value, char splitter)
        {
            string separator = splitter + " ";
            return value?.Aggregate((current, next) => current + separator + next);
        }

        private static void AddKeyValueToDictionary(IDictionary dict, string key, object value, Type valueType)
        {
            object dictValue;
            try
            {
                dictValue = Convert.ChangeType(value, valueType);
            }
            catch
            {
                string msg = $"Wrong value '{value}' of type '{valueType.Name}'";
                throw new Exception(msg);
            }

            if (dict.Contains(key))
            {
                string msg = $"Duplicate dictionary key '{key}'";
                throw new Exception(msg);
            }

            dict.Add(key, dictValue);
        }

        public static object XmlNodeToKeyValue(Type dataType, XmlNode xmlNode)
        {
            if (xmlNode == null)
            {
                string msg = "XmlNode cannot be null";
                throw new Exception(msg);
            }

            object result = Activator.CreateInstance(dataType);
            XmlNodeList paramNodes = xmlNode.ChildNodes;
            if (paramNodes.Count == 0)
            {
                return result;
            }

            Type dictValueType = null;

            foreach (XmlNode paramNode in paramNodes)
            {
                switch (result)
                {
                    case IDictionary dict:
                        if (dictValueType == null)
                        {
                            Type[] arguments = result.GetType().GetGenericArguments();
                            dictValueType = arguments[1];
                        }
                        AddKeyValueToDictionary(dict, paramNode.Name, paramNode.InnerText, dictValueType);
                        break;
                    default:
                        string msg = $"Type '{dataType.Name}' is not supported";
                        throw new Exception(msg);
                }
            }

            return result;
        }

        public static void KeyValueToXmlNode(Type dataType, string keyName, string valueName, 
            object value, XmlNode xmlNode)
        {
            xmlNode.InnerXml = string.Empty;

            if (value == null)
            {
                return;
            }

            switch (value)
            {
                case IDictionary dict:
                    DictionaryToXmlNode(dict, keyName, valueName, xmlNode);
                    break;
                default:
                    string msg = $"Type '{dataType.Name}' is not supported";
                    throw new Exception(msg);
            }
        }

        private static void DictionaryToXmlNode(IDictionary dict, string keyName, string valueName, XmlNode xmlNode)
        {
            XmlDocument xmlDoc = xmlNode.OwnerDocument;
            foreach (string key in dict.Keys.Cast<string>())
            {
                if (string.IsNullOrEmpty(key))
                {
                    string msg = $"{keyName} cannot be empty";
                    throw new Exception(msg);
                }

                object dictValue = dict[key];

                XmlNode valNode;
                try
                {
                    valNode = xmlDoc?.CreateNode(XmlNodeType.Element, key, xmlDoc.NamespaceURI);
                }
                catch (XmlException ex)
                {
                    string msg = $"{keyName} '{key}'. {ex.Message}";
                    throw new Exception(msg);
                }

                xmlNode.AppendChild(valNode);
                valNode.InnerText = dictValue?.ToString() ?? string.Empty;
            }
        }

        public static string KeyValueToJson(Type dataType, string keyName, string valueName, object value)
        {
            if (value == null)
            {
                return "{}";
            }

            switch (value)
            {
                case IDictionary dictPair:
                    return DictionaryToJson(keyName, valueName, dictPair);
                default:
                    string msg = $"Type '{dataType.Name}' is not supported";
                    throw new Exception(msg);
            }
        }

        private static string DictionaryToJson(string keyName, string valueName, IDictionary dict)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append($"{{ \"colsNumber\": 2, \"col1\": \"{keyName}\", \"col2\": \"{valueName}\"");

            List<string> entries = new List<string>();
            foreach (DictionaryEntry d in dict)
            {
                entries.Add($"{{ \"col1\": \"{d.Key}\", \"col2\": \"{d.Value}\" }}");
            }

            sbJson.Append($", \"values\":[ {string.Join(",", entries)}] }}");
            return sbJson.ToString();
        }

        public static object JsonToKeyValue(Type dataType, string sJson)
        {
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return JsonToDictionary(dataType, sJson);
            }

            string msg = $"Type '{dataType.Name}' is not supported";
            throw new Exception(msg);
        }

        private static object JsonToDictionary(Type dataType, string sJson)
        {
            Type[] arguments = dataType.GetGenericArguments();
            Type dictType = typeof(Dictionary<,>).MakeGenericType(typeof(string), arguments[1]);
            Type listType = typeof(List<>).MakeGenericType(dictType);

            IList jsonObject = (IList) sJson?.FromJson(listType);
            if (jsonObject == null)
            {
                throw new Exception("Invalid JSON string");
            }

            IDictionary result = (IDictionary) Activator.CreateInstance(dictType);
            foreach (IDictionary dict in jsonObject)
            {
                AddKeyValueToDictionary(result, (string) dict["col1"], dict["col2"], arguments[1]);
            }

            return result;
        }
    }
}
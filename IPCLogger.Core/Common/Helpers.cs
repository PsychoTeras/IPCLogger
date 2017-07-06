using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Common
{
    internal static class Helpers
    {
        [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PathFileExists(string path);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        private static readonly Regex _regexBytesString = new Regex(@"^\s*(?<SIZE>\d+)+[ ]*(?<UNIT>[a-z]+)*", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        private static readonly Regex _regexTimeString = new Regex(@"(?<COUNT>\d+)+[ ]*(?<UNIT>[a-z]+)+",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        public static bool ByteArrayEquals(byte[] b1, byte[] b2)
        {
            return (b1 == null && b2 == null) ||
                   (b1 != null && b2 != null && 
                    b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0);
        }
        
        public static string CalculateUniqueId(string name, string type, string nameSpace)
        {
            return string.Format("^{0}${1}%{2}^", name, type, nameSpace);
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

        public static int BytesStringToSize(string sBytes)
        {
            if (sBytes == string.Empty)
            {
                string msg = "Value cannot be empty";
                throw new Exception(msg);
            }

            Match match = _regexBytesString.Match(sBytes);
            string sSize = match.Groups["SIZE"].Value;
            if (sSize == string.Empty)
            {
                string msg = "Value is not defined";
                throw new Exception(msg);
            }

            int size;
            if (!Int32.TryParse(sSize, out size))
            {
                string msg = string.Format("Value '{0}' is invalid", sSize);
                throw new Exception(msg);
            }

            int multiplier = 1;
            string sUnit = match.Groups["UNIT"].Value.ToLower();
            if (sUnit != string.Empty)
            {
                switch (sUnit)
                {
                    case "b":
                        break;
                    case "kb":
                        multiplier = 1024;
                        break;
                    case "mb":
                        multiplier = 1048576;
                        break;
                    case "gb":
                        multiplier = 1073741824;
                        break;
                    default:
                        string msg = string.Format("Unit '{0}' is invalid. Use B, KB, MB, GB instead", sUnit);
                        throw new Exception(msg);
                }
            }

            return size*multiplier;
        }

        public static TimeSpan TimeStringToTimeSpan(string sTime)
        {
            if (sTime == string.Empty)
            {
                string msg = "Time string cannot be empty";
                throw new Exception(msg);
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
                int count;
                string sCount = match.Groups["COUNT"].Value;
                if (!Int32.TryParse(sCount, out count))
                {
                    string msg = string.Format("Value '{0}' is invalid", sCount);
                    throw new Exception(msg);
                }

                string sUnit = match.Groups["UNIT"].Value;
                switch (sUnit)
                {
                    case "y":
                        days += count*365;
                        break;
                    case "M":
                        days += count*31;
                        break;
                    case "w":
                        days += count*7;
                        break;
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
                        string msg = string.Format(@"Unit '{0}' is invalid.
Use y (=years), M (=months), w (=weeks), d (=days), h (=hours), m (=minutes), s (=seconds) instead", sUnit);
                        throw new Exception(msg);
                }
            }

            return new TimeSpan(days, hours, minutes, seconds);
        }
    }
}

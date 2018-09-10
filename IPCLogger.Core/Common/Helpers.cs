﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Common
{
    internal static class Helpers
    {
        private const int ONE_GBYTE = 1073741824;
        private const int ONE_MBYTE = 1048576;
        private const int ONE_KBYTE = 1024;

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

        public static string SizeToBytesString(long size)
        {
            string sBytes = string.Empty;

            double sizeLeft = size;
            while (sizeLeft >= ONE_KBYTE)
            {
                int value = 0;
                if (sizeLeft >= ONE_GBYTE)
                {
                    value = (int)Math.Floor(sizeLeft / ONE_GBYTE);
                    sBytes += $"{value} GB ";
                    sizeLeft -= value * ONE_GBYTE;
                }
                else if (sizeLeft >= ONE_MBYTE)
                {
                    value = (int)Math.Floor(sizeLeft / ONE_MBYTE);
                    sBytes += $"{value} MB ";
                    sizeLeft -= value * ONE_MBYTE;
                }
                else if (sizeLeft >= ONE_KBYTE)
                {
                    value = (int)Math.Floor(sizeLeft / ONE_KBYTE);
                    sBytes += $"{value} KB ";
                    sizeLeft -= value * ONE_KBYTE;
                }
            }

            if (sBytes == string.Empty)
            {
                sBytes = $"{sizeLeft} B";
            }
            else if (sizeLeft != 0)
            {
                sBytes += $"{sizeLeft} B";
            }

            return sBytes.TrimEnd();
        }

        public static long BytesStringToSize(string sBytes)
        {
            if (sBytes == string.Empty)
            {
                string msg = "Value cannot be empty";
                throw new Exception(msg);
            }

            MatchCollection matches = _regexBytesString.Matches(sBytes);
            if (matches.Count == 0)
            {
                string msg = "Size string is invalid";
                throw new Exception(msg);
            }

            long value = 0;
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

                int multiplier = 1;
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
                            string msg = $"Unit '{sUnit}' is invalid. Use B, KB, MB, GB instead";
                            throw new Exception(msg);
                    }
                }
                value += size * multiplier;
            }
            return value;
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
                string sCount = match.Groups["COUNT"].Value;
                if (!int.TryParse(sCount, out int count))
                {
                    string msg = $"Value '{sCount}' is invalid";
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
                        string msg = $"Unit '{sUnit}' is invalid. Use y (=years), M (=months), w (=weeks), d (=days), h (=hours), m (=minutes), s (=seconds) instead";
                        throw new Exception(msg);
                }
            }

            return new TimeSpan(days, hours, minutes, seconds);
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
    }
}

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Common
{
    internal static class Helpers
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        private static readonly Regex _regexBytesString = new Regex(@"^\s*(?<SIZE>\d+)+[ ]*(?<UNIT>\w+)*", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

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
            if (!int.TryParse(sSize, out size))
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
                        string msg = string.Format("Value unit '{0}' is invalid. Use B, KB, MB, GB instead", sUnit);
                        throw new Exception(msg);
                }
            }

            return size*multiplier;
        }
    }
}

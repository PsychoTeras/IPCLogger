using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Common
{
    internal static class Helpers
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

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
    }
}

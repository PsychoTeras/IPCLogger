using System;
using System.Linq;
using System.Reflection;

namespace IPCLogger.Core.Common
{
    internal static class Extensions
    {
        public static T GetAttribute<T>(this PropertyInfo pi) where T : Attribute
        {
            return pi.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }

        public static bool IsDefined<T>(this PropertyInfo pi) where T : Attribute
        {
            return pi.IsDefined(typeof(T), true);
        }
    }
}

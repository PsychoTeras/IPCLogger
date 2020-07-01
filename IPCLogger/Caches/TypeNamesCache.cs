using System;
using System.Collections.Generic;
using System.Text;

namespace IPCLogger.Caches
{
    internal static class TypeNamesCache
    {
        private static readonly Dictionary<Type, string> _typeNames = new Dictionary<Type, string>();

        private static void BuildGenericTypeName(Type type, Type[] inGenericTypes, StringBuilder sbName)
        {
            string name = type.Name;
            Type[] genericTypes = inGenericTypes ?? type.GetGenericArguments();
            if (genericTypes.Length == 0)
            {
                sbName.Append(name);
                return;
            }

            int idx = name.IndexOf("`", StringComparison.Ordinal);
            if (idx == -1)
            {
                sbName.Append(name);
                return;
            }

            sbName.AppendFormat("{0}<", name.Substring(0, idx));
            for (int i = 0; i < genericTypes.Length; i++)
            {
                if (i != 0)
                {
                    sbName.Append(", ");
                }
                Type gType = genericTypes[i];
                Type[] subGenericTypes = gType.GetGenericArguments();
                if (subGenericTypes.Length == 0)
                {
                    sbName.Append(gType.Name);
                }
                else
                {
                    BuildGenericTypeName(gType, subGenericTypes, sbName);
                }
            }
            sbName.Append(">");
        }

        public static string GetTypeName(Type type)
        {
            if (type == null) return string.Empty;

            string typeName;
            if (!_typeNames.TryGetValue(type, out typeName))
            {
                lock (_typeNames)
                {
                    if (!_typeNames.TryGetValue(type, out typeName))
                    {
                        StringBuilder sbName = new StringBuilder();
                        BuildGenericTypeName(type, null, sbName);
                        _typeNames.Add(type, typeName = sbName.ToString());
                    }
                }
            }
            return typeName;
        }

    }
}

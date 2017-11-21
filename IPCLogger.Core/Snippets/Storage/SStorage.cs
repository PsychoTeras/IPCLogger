using System;
using System.Collections;
using System.Linq;
using System.Text;
using IPCLogger.Core.Caches;
using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;
using IPCLogger.Core.Storages;

namespace IPCLogger.Core.Snippets.Storage
{
    sealed class SStorage : BaseSnippet
    {

#region Private fields

        private static readonly string _collItemPrefix = "\t";

#endregion

#region Properties

        public override string[] Names
        {
            get { return null; }
        }

#endregion

#region Ctor

        public SStorage() : base(SnippetType.Storage) { }

#endregion

#region Class methods

        private bool InvokeValueAsDelegate(Type type, ref string typeName, ref object val)
        {
            if (type == typeof (FuncObject))
            {
                FuncObject funcObj = (FuncObject) val;
                typeName = TypeNamesCache.GetTypeName(funcObj.ObjType);
                MulticastDelegate d = (MulticastDelegate) funcObj.Delegate;
                try
                {
                    val = d.DynamicInvoke();
                    return true;
                }
                catch (Exception ex)
                {
                    val = string.Format("{0} invocation error: {1}", d.Method.Name, ex);
                }
            }
            return false;
        }

        private void ObjectToString(object obj, string objName, StringBuilder sb, bool unfold, 
            bool detailed, string prefix, string defValue)
        {
            if (prefix != null)
            {
                sb.Append(prefix);
            }

            if (detailed && objName != null)
            {
                sb.AppendFormat("{0} ", objName);
            }

            Type type = obj != null ? obj.GetType() : null;
            if (type == typeof(TLSObject))
            {
                if (detailed)
                {
                    sb.Append(Constants.NewLine);
                    prefix = prefix == null ? _collItemPrefix : prefix + _collItemPrefix;
                }

                TLSObject tlsObj = (TLSObject) obj;
                int idx = 0;
                int keysCount = tlsObj.Values.Count;
                foreach (FuncObject propObj in tlsObj.Values)
                {
                    ObjectToString(propObj, propObj.ObjName, sb, unfold, detailed, prefix, defValue);
                    if (++idx < keysCount)
                    {
                        sb.Append(Constants.NewLine);
                    }
                }
                return;
            }

            string typeName = null;
            if (type == typeof(DictionaryEntry))
            {
                DictionaryEntry de = (DictionaryEntry) obj;
                if (detailed)
                {
                    sb.AppendFormat("{0} ", de.Key);
                }
                obj = de.Value;
            }

            ICollection objAsCollection;
            if (detailed)
            {
                if (type != null && !InvokeValueAsDelegate(type, ref typeName, ref obj))
                {
                    typeName = TypeNamesCache.GetTypeName(type);
                }
                typeName = typeName ?? DefUnknownTypeString;

                objAsCollection = unfold ? obj as ICollection : null;
                if (objAsCollection == null)
                {
                    sb.AppendFormat("[{0}]: {1}", typeName, obj ?? defValue);
                }
                else
                {
                    if (prefix == null)
                    {
                        prefix = _collItemPrefix;
                        sb.AppendFormat("{0}{1}", Constants.NewLine, prefix);
                    }
                    sb.AppendFormat("[{0}, size {1}] >", typeName, objAsCollection.Count);
                }
            }
            else
            {
                InvokeValueAsDelegate(type, ref typeName, ref obj);
                objAsCollection = unfold ? obj as ICollection : null;
                if (objAsCollection == null)
                {
                    sb.Append(obj ?? defValue);
                }
            }

            if (objAsCollection != null && objAsCollection.Count > 0)
            {
                prefix = prefix == null ? _collItemPrefix : prefix + _collItemPrefix;
                foreach (object collectionItem in objAsCollection)
                {
                    sb.Append(Constants.NewLine);
                    ObjectToString(collectionItem, null, sb, true, detailed, prefix, defValue);
                }
            }
        }

        public override string Process(Type callerType, Enum eventType, string snippetName,
            byte[] data, string text, string @params, PFactory pFactory)
        {
            if (snippetName == string.Empty) return null;

            StringBuilder sb = new StringBuilder();

            SnippetParams sParams = ParseSnippetParams(@params);
            bool unfold = sParams.HasValue("unfold");
            bool detailed = sParams.HasValue("detailed");
            bool ordered = sParams.HasValue("ordered");
            string defValue = detailed ? DefNullValueString : null;

            if (snippetName == Constants.ApplicableForAllMark)
            {
                TLSObject val = TLS.Peek();
                int keysCount;
                if (val == null || (keysCount = val.Count) == 0) return null;

                int idx = 0;
                IEnumerable keys = ordered
                    ? val.Keys.OrderBy(k => k).AsEnumerable()
                    : val.Keys;
                foreach (string objName in keys)
                {
                    ObjectToString(val[objName], objName, sb, unfold, detailed, null, defValue);
                    if (++idx < keysCount)
                    {
                        sb.Append(Constants.NewLine);
                    }
                }
            }
            else
            {
                object val = TLS.Get(snippetName);
                ObjectToString(val, snippetName, sb, unfold, detailed, null, defValue);
            }
            return sb.Length == 0 ? null : sb.ToString();
        }

#endregion

    }
}

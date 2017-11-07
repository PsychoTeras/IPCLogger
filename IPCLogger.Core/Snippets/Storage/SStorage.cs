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

        private bool InvokeValueAsDelegate(Type type, ref string name, ref object val)
        {
            if (type == typeof (FuncObject))
            {
                FuncObject funcObj = (FuncObject) val;
                name = TypeNamesCache.GetTypeName(funcObj.ObjType);
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

        private void ObjectToString(object val, StringBuilder sb, bool unfold, bool detailed, 
            string prefix, string defValue)
        {
            if (prefix != null)
            {
                sb.Append(prefix);
            }

            string name = null;
            if (val is DictionaryEntry)
            {
                DictionaryEntry de = (DictionaryEntry) val;
                if (detailed)
                {
                    sb.AppendFormat("{0} ", de.Key);
                }
                val = de.Value;
            }

            Type type = val != null ? val.GetType() : null;
            ICollection iColl = unfold ? val as ICollection : null;
            if (detailed)
            {
                if (type != null && !InvokeValueAsDelegate(type, ref name, ref val))
                {
                    name = TypeNamesCache.GetTypeName(type);
                }
                name = name ?? DefUnknownTypeString;
                if (iColl == null)
                {
                    sb.AppendFormat("[{0}]: {1}", name, val ?? defValue);
                }
                else if (!(iColl is TLSObject))
                {
                    if (prefix == null)
                    {
                        prefix = _collItemPrefix;
                        sb.AppendFormat("{0}{1}", Constants.NewLine, prefix);
                    }
                    sb.AppendFormat("[{0}, size {1}] >", name, iColl.Count);
                }
            }
            else
            {
                if (iColl == null)
                {
                    InvokeValueAsDelegate(type, ref name, ref val);
                    sb.Append(val ?? defValue);
                }
                else
                {
                    sb.AppendFormat("Collection, size {0} >", iColl.Count);
                }
            }

            if (iColl != null && iColl.Count > 0)
            {
                string newPrefix = prefix == null ? _collItemPrefix : prefix + _collItemPrefix;
                foreach (object item in iColl)
                {
                    sb.Append(Constants.NewLine);
                    ObjectToString(item, sb, true, detailed, newPrefix, defValue);
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
                Hashtable val = TLS.Peek();
                if (val == null || val.Count == 0) return null;

                int idx = 0;
                int cnt = val.Keys.Count;
                IEnumerable keys = ordered
                    ? (IEnumerable) val.Keys.OfType<object>().OrderBy(k => k).AsEnumerable()
                    : val.Keys;
                foreach (object key in keys)
                {
                    if (detailed)
                    {
                        sb.AppendFormat("{0} ", key);
                    }
                    ObjectToString(val[key], sb, unfold, detailed, null, defValue);
                    if (++idx < cnt)
                    {
                        sb.Append(Constants.NewLine);
                    }
                }
            }
            else
            {
                object val = TLS.Get(snippetName);
                if (detailed)
                {
                    sb.AppendFormat("{0} ", snippetName);
                }
                ObjectToString(val, sb, unfold, detailed, null, defValue);
            }
            return sb.Length == 0 ? null : sb.ToString();
        }

#endregion

    }
}

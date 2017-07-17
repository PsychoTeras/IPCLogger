using System;
using System.Collections;
using System.Text;
using IPCLogger.Core.Caches;
using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;
using IPCLogger.Core.Storages;

namespace IPCLogger.Core.Snippets.Storage
{
    internal sealed class SStorage : BaseSnippet
    {

#region Private fields

        private static readonly string CollItemPrefix = "\t";

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
            if (type.BaseType == typeof (MulticastDelegate))
            {
                MulticastDelegate d = (MulticastDelegate) val;
                try
                {
                    val = d.DynamicInvoke();
                    if (val != null)
                    {
                        name = TypeNamesCache.GetTypeName(val.GetType());
                    }
                }
                catch (Exception ex)
                {
                    val = string.Format("{0} invocation error: {1}", d.Method.Name, ex);
                }
                return true;
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
            Type type = val != null ? val.GetType() : null;
            ICollection iColl = unfold ? val as ICollection : null;
            if (detailed)
            {
                if (type == null || !InvokeValueAsDelegate(type, ref name, ref val))
                {
                    name = TypeNamesCache.GetTypeName(type);
                }
                if (iColl == null)
                {
                    sb.AppendFormat("[{0}] {1}", name, val ?? defValue);
                }
                else
                {
                    if (prefix == null)
                    {
                        prefix = CollItemPrefix;
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
                string newPrefix = prefix == null ? CollItemPrefix : prefix + CollItemPrefix;
                foreach (object item in iColl)
                {
                    sb.Append(Constants.NewLine);
                    ObjectToString(item, sb, true, detailed, newPrefix, defValue);
                }
            }
        }

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            if (snippetName == string.Empty) return null;

            StringBuilder sb = new StringBuilder();
            SnippetParams sParams = SnippetParams.Parse(@params);
            bool unfold = sParams.HasValue("unfold");
            bool detailed = sParams.HasValue("detailed");
            string defValue = detailed ? DefNullValueString : null;

            if (snippetName == Constants.ApplicableForAllMark)
            {
                Hashtable val = TLS.Peek();
                if (val == null || val.Count == 0) return null;

                foreach (object key in val.Keys)
                {
                    sb.AppendFormat("{0}: ", key);
                    ObjectToString(val[key], sb, unfold, detailed, null, defValue);
                    sb.Append(Constants.NewLine);
                }
            }
            else
            {
                object val = TLS.Get(snippetName);
                ObjectToString(val, sb, unfold, detailed, null, defValue);
            }
            return sb.Length == 0 ? null : sb.ToString();
        }

#endregion

    }
}

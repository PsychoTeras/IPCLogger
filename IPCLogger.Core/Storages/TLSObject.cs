using System;
using System.Collections;
using System.Linq.Expressions;
using IPCLogger.Core.Caches;

namespace IPCLogger.Core.Storages
{
    public sealed class TLSObject : Hashtable, IDisposable
    {

#region Ctor

        internal TLSObject() { }

#endregion

#region IDisposable

        public void SetClosure<T>(Expression<Func<T>> memberExpression)
        {
            SetClosure(null, memberExpression);
        }

        public void SetClosure<T>(string key, Expression<Func<T>> memberExpression)
        {
            switch (memberExpression.Body.NodeType)
            {
                case ExpressionType.Constant:
                {
                    ConstantExpression ce = (ConstantExpression) memberExpression.Body;
                    string name = key ?? ce.Value.ToString();
                    this[name] = name;
                    break;
                }
                case ExpressionType.MemberAccess:
                {
                    MemberExpression body = (MemberExpression) memberExpression.Body;
                    string name = key ?? body.Member.Name;
                    if (body.Expression == null)
                    {
                        this[name] = TLSClosureMembers.GetTLSClosureMember(body.Member, () =>
                        {
                            Type delegateType = typeof (Func<>).MakeGenericType(body.Type);
                            return Expression.Lambda(delegateType, body).Compile();
                        });
                    }
                    else
                    {
                        this[name] = body;
                    }
                    break;
                }
                case ExpressionType.Call:
                {
                    MethodCallExpression body = (MethodCallExpression) memberExpression.Body;
                    string name = key ?? body.Method.Name;
                    this[name] = TLSClosureMembers.GetTLSClosureMember(body.Method, () =>
                    {
                        Type delegateType = typeof (Func<>).MakeGenericType(body.Type);
                        return Expression.Lambda(delegateType, body).Compile();
                    });
                    break;
                }
            }
        }

        public void Dispose()
        {
            TLS.Pop();
        }

#endregion

    }
}

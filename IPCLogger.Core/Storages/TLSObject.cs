using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using IPCLogger.Core.Caches;

namespace IPCLogger.Core.Storages
{
    public sealed class TLSObject : Hashtable, IDisposable
    {

#region Ctor

        internal TLSObject() { }

#endregion

#region Class methods

        private static Expression NullSafeEvalWrapper(Expression expr, Type type)
        {
            Expression obj;
            Expression safe = expr;
            string memberName;
            while (!IsNullSafe(expr, out obj, out memberName) && obj != null)
            {
                Expression isNull = Expression.Equal(obj, Expression.Constant(null));
                safe = Expression.Condition(isNull,
                    memberName != null
                        ? (Expression) Expression.Constant(
                            string.Format("<{0}{1} NULL>", memberName, obj.NodeType == ExpressionType.Call ? "() returned" : " is"))
                        : Expression.Default(type), safe);
                expr = obj;
            }
            return safe;
        }

        private static bool IsNullSafe(Expression expr, out Expression nullableObject, out string memberName)
        {
            memberName = null;
            nullableObject = null;

            MemberExpression memberExpr = expr as MemberExpression;
            MethodCallExpression callExpr = expr as MethodCallExpression;
            if (memberExpr != null || callExpr != null)
            {
                Expression obj;

                if (memberExpr != null)
                {
                    //Static fields don't require an instance
                    FieldInfo field = memberExpr.Member as FieldInfo;
                    if (field != null)
                    {
                        if (field.IsStatic)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        //Static properties don't require an instance
                        PropertyInfo property = memberExpr.Member as PropertyInfo;
                        if (property != null)
                        {
                            MethodInfo getter = property.GetGetMethod();
                            if (getter != null && getter.IsStatic)
                            {
                                return true;
                            }
                        }
                    }

                    obj = memberExpr.Expression;
                    memberName = ((MemberExpression)obj).Member.Name;
                }
                else
                {
                    //Static methods don't require an instance
                    if (callExpr.Method.IsStatic)
                    {
                        return true;
                    }

                    obj = callExpr.Object;
                    if (obj is MethodCallExpression)
                    {
                        memberName = ((MethodCallExpression) obj).Method.Name;
                    } else if (obj is MemberExpression)
                    {
                        memberName = ((MemberExpression) obj).Member.Name;
                    }
                }

                //Value types can't be null
                if (obj != null && obj.Type.IsValueType)
                {
                    return true;
                }

                //Instance member access or instance method call is not safe
                nullableObject = obj;
                return false;
            }

            return true;
        }

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
                    this[name] = TLSClosureMembers.GetTLSClosureMember(body.Member, () =>
                    {
                        Type bodyType = body.Type;
                        Type delegateType = typeof (Func<>).MakeGenericType(bodyType);
                        return Expression.Lambda(delegateType, NullSafeEvalWrapper(body, bodyType)).Compile();
                    });
                    break;
                }
                case ExpressionType.Call:
                {
                    MethodCallExpression body = (MethodCallExpression) memberExpression.Body;
                    string name = key ?? body.Method.Name;
                    this[name] = TLSClosureMembers.GetTLSClosureMember(body.Method, () =>
                    {
                        Type bodyType = body.Type;
                        Type delegateType = typeof (Func<>).MakeGenericType(bodyType);
                        return Expression.Lambda(delegateType, NullSafeEvalWrapper(body, bodyType)).Compile();
                    });
                    break;
                }
            }
        }

#endregion

#region #region IDisposable

        public void Dispose()
        {
            TLS.Pop();
        }

#endregion

    }
}

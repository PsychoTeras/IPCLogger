using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using IPCLogger.Core.Caches;

namespace IPCLogger.Core.Storages
{
    public sealed class TLSObject : Dictionary<string, object>, IDisposable
    {

#region Constants

        public const BindingFlags BF_DEFAULT =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic;

#endregion

#region Private fields

        private static readonly Expression _exprConstantNull = Expression.Constant(null);

        private static readonly DictionaryCache<object, FuncObject> _cacheClosureMembers =
            new DictionaryCache<object, FuncObject>();

#endregion

#region Ctor

        internal TLSObject() { }

#endregion

#region Class methods

        private static Expression NullSafeWrapper(Expression expr, Type type)
        {
            Expression obj;
            Expression safe = expr;
            string memberName;
            while (!IsNullSafe(expr, out obj, out memberName) && obj != null)
            {
                Expression isNull = Expression.Equal(obj, _exprConstantNull);
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
                    if (field != null && field.IsStatic)
                    {
                        return true;
                    }

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

                    obj = memberExpr.Expression;
                    if (obj is MemberExpression)
                    {
                        memberName = ((MemberExpression) obj).Member.Name;
                    }
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
                    }
                    else if (obj is MemberExpression)
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

        private static string GetMemberName(Expression body)
        {
            string name = body.ToString();
            int idxOfName = name.IndexOf(").");
            return idxOfName == -1 ? name : name.Substring(idxOfName + 2);
        }

        public void SetClosure<T>(Expression<Func<T>> memberExpression)
        {
            SetClosure(null, memberExpression);
        }

        public void SetClosure(params Expression<Func<object>>[] membersExpressions)
        {
            foreach (Expression<Func<object>> memberExpression in membersExpressions)
            {
                SetClosure((string) null, memberExpression);
            }
        }

        public void SetClosure<T>(string key, Expression<Func<T>> memberExpression)
        {
            switch (memberExpression.Body.NodeType)
            {
                case ExpressionType.Constant:
                {
                    ConstantExpression ce = (ConstantExpression) memberExpression.Body;
                    bool isKeyEmpty = string.IsNullOrEmpty(key);
                    if (ce.Value == null && isKeyEmpty)
                    {
                        return;
                    }
                    string name = isKeyEmpty ? ce.Value.ToString() : key;
                    this[name] = name;
                    break;
                }
                case ExpressionType.NewArrayInit:
                {
                    NewArrayExpression body = (NewArrayExpression)memberExpression.Body;
                    string name = key ?? GetMemberName(body);
                    this[name] = _cacheClosureMembers.Get(body.Expressions, () =>
                    {
                        Type bodyType = body.Type;
                        Type delegateType = typeof(Func<>).MakeGenericType(bodyType);
                        return new FuncObject(Expression.Lambda(delegateType, NullSafeWrapper(body, bodyType)).Compile(), name, bodyType);
                    });
                    break;
                }
                case ExpressionType.MemberAccess:
                {
                    MemberExpression body = (MemberExpression) memberExpression.Body;
                    string name = key ?? GetMemberName(body);
                    this[name] = _cacheClosureMembers.Get(body.Member, () =>
                    {
                        Type bodyType = body.Type;
                        Type delegateType = typeof (Func<>).MakeGenericType(bodyType);
                        return new FuncObject(Expression.Lambda(delegateType, NullSafeWrapper(body, bodyType)).Compile(), name, bodyType);
                    });
                    break;
                }
                case ExpressionType.Call:
                {
                    MethodCallExpression body = (MethodCallExpression) memberExpression.Body;
                    string name = key ?? GetMemberName(body);
                    this[name] = _cacheClosureMembers.Get(body.Method, () =>
                    {
                        Type bodyType = body.Type;
                        Type delegateType = typeof (Func<>).MakeGenericType(bodyType);
                        return new FuncObject(Expression.Lambda(delegateType, NullSafeWrapper(body, bodyType)).Compile(), name, bodyType);
                    });
                    break;
                }
            }
        }

        private void CaptureObjectField<T>(TLSObject tlsObj, T obj, bool useFullClassName, FieldInfo field, HashSet<string> excludeNames)
        {
            string fieldName = field.Name;
            if (excludeNames != null && excludeNames.Contains(fieldName)) return;

            string name = useFullClassName ? typeof(T).Name + "." + fieldName : fieldName;
            tlsObj[name] = _cacheClosureMembers.Get(field, () =>
            {
                Type fieldType = field.FieldType;
                MemberExpression body = field.IsStatic
                    ? Expression.Field(null, field)
                    : Expression.Field(Expression.Constant(obj), fieldName);
                Type delegateType = typeof (Func<>).MakeGenericType(fieldType);
                return new FuncObject(Expression.Lambda(delegateType, NullSafeWrapper(body, body.Type)).Compile(), name, fieldType);
            });
        }

        private void CaptureObjectProperty<T>(TLSObject tlsObj, T obj, bool useFullClassName, PropertyInfo property, bool isStatic,
            HashSet<string> excludeNames)
        {
            string propertyName = property.Name;
            if (excludeNames != null && excludeNames.Contains(propertyName)) return;

            string name = useFullClassName ? typeof(T).Name + "." + propertyName : propertyName;
            tlsObj[name] = _cacheClosureMembers.Get(property, () =>
            {
                Type propertyType = property.PropertyType;
                MemberExpression body = isStatic
                    ? Expression.Property(null, property)
                    : Expression.Property(Expression.Constant(obj), propertyName);
                Type delegateType = typeof(Func<>).MakeGenericType(propertyType);
                return new FuncObject(Expression.Lambda(delegateType, NullSafeWrapper(body, body.Type)).Compile(), name, propertyType);
            });
        }

        public void CaptureObject<T>(string key, T obj, bool useFullClassName = true,
            BindingFlags? bfFields = BindingFlags.GetField | BF_DEFAULT, 
            BindingFlags? bfProperties = BindingFlags.GetProperty | BF_DEFAULT,
            HashSet<string> excludeNames = null)
        {
            if (obj == null) return;

            if (string.IsNullOrEmpty(key))
            {
                key = typeof (T).Name + "_" + obj.GetHashCode();
            }

            TLSObject tlsObj = new TLSObject();

            if (bfFields != null)
            {
                FieldInfo[] fields = typeof(T).GetFields(bfFields.Value);
                foreach (FieldInfo field in fields)
                {
                    if (field.Name[0] != '<')
                    {
                        CaptureObjectField(tlsObj, obj, useFullClassName, field, excludeNames);
                    }
                }
            }

            if (bfProperties != null)
            {
                PropertyInfo[] properties = typeof(T).GetProperties(bfProperties.Value);
                foreach (PropertyInfo property in properties)
                {
                    MethodInfo[] propAccessors = property.GetAccessors(true);
                    if (propAccessors.Length > 0)
                    {
                        CaptureObjectProperty(tlsObj, obj, useFullClassName, property, propAccessors[0].IsStatic, excludeNames);
                    }
                }
            }

            if (tlsObj.Count > 0)
            {
                this[key] = tlsObj;
            }
        }

#endregion

#region IDisposable

        public void Dispose()
        {
            TLS.Pop();
        }

#endregion

    }
}

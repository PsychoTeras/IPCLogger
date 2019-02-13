using System;
using System.Collections.Generic;

namespace IPCLogger.Core.Resolvers.Base
{
    public abstract class BaseResolver<T> : IResolver
        where T : struct, IConvertible
    {
        public Enum Type { get; }

        protected BaseResolver(T t)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            Type = t as Enum;
        }

        public abstract object Resolve(object key);

        public virtual string AsString(object key)
        {
            return Resolve(key) as string;
        }

        public virtual string[] AsArray(object key)
        {
            return Resolve(key) as string[];
        }

        public virtual TO AsObject<TO>(object key)
        {
            return (TO)Resolve(key);
        }

        public virtual IEnumerable<TO> GetKeys<TO>()
        {
            return null;
        }

        public virtual IEnumerable<TO> GetValues<TO>()
        {
            return null;
        }
    }
}

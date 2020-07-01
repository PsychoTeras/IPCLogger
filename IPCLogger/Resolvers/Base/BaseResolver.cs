using System.Collections.Generic;

namespace IPCLogger.Resolvers.Base
{
    public abstract class BaseResolver : IResolver
    {
        public abstract ResolverType Type { get; }

        public object Tag { get; } = null;

        public abstract object Resolve(object key);

        public virtual string AsString(object key)
        {
            return Resolve(key) as string;
        }

        public virtual string[] AsArray(object key)
        {
            return Resolve(key) as string[];
        }

        public virtual T AsObject<T>(object key)
        {
            return (T)Resolve(key);
        }

        public virtual IEnumerable<T> GetKeys<T>()
        {
            return null;
        }

        public virtual IEnumerable<T> GetValues<T>()
        {
            return null;
        }
    }
}

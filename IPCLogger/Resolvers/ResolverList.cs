using IPCLogger.Resolvers.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IPCLogger.Resolvers
{
    internal class ResolverList : IResolver
    {
        private List<IResolver> _resolvers;

        public ResolverType Type { get; }

        public object Tag { get; } = null;

        public ResolverList(ResolverType type)
        {
            Type = type;
            _resolvers = new List<IResolver>();
        }

        public void Add(IResolver resolver)
        {
            if (resolver == null)
            {
                throw new Exception("resolver is null");
            }
            _resolvers.Add(resolver);
        }

        public object Resolve(object key)
        {
            foreach (IResolver resolver in _resolvers)
            {
                object result = resolver.Resolve(key);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public string AsString(object key)
        {
            return Resolve(key) as string;
        }

        public string[] AsArray(object key)
        {
            return Resolve(key) as string[];
        }

        public T AsObject<T>(object key)
        {
            return (T)Resolve(key);
        }

        public IEnumerable<T> GetKeys<T>()
        {
            return _resolvers.SelectMany(r => r.GetKeys<T>() ?? Enumerable.Empty<T>());
        }

        public IEnumerable<T> GetValues<T>()
        {
            return _resolvers.SelectMany(r => r.GetValues<T>() ?? Enumerable.Empty<T>());
        }

        public IResolver GetByTag(object tag)
        {
            return _resolvers.FirstOrDefault(r => r.Tag == tag);
        }
    }
}

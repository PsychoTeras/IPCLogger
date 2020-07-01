using System.Collections.Generic;

namespace IPCLogger.Resolvers.Base
{
    public interface IResolver
    {
        ResolverType Type { get; }

        object Tag { get; }

        object Resolve(object key);

        string AsString(object key);

        string[] AsArray(object key);

        T AsObject<T>(object key);

        IEnumerable<T> GetKeys<T>();

        IEnumerable<T> GetValues<T>();
    }
}

using System;
using System.Collections.Generic;

namespace IPCLogger.Core.Resolvers.Base
{
    public interface IResolver
    {
        Enum Type { get; }

        object Resolve(object key);

        string AsString(object key);

        string[] AsArray(object key);

        T AsObject<T>(object key);

        IEnumerable<T> GetKeys<T>();

        IEnumerable<T> GetValues<T>();
    }
}

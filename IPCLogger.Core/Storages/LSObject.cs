using System;

namespace IPCLogger.Core.Storages
{
    internal sealed class LsObject : IDisposable
    {

#region Public fields

        public Exception Exception;

#endregion

#region IDisposable

        public void Dispose()
        {
            Ls.Pop();
        }

#endregion

    }
}

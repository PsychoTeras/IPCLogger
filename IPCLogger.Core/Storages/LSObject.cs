using System;

namespace IPCLogger.Core.Storages
{
    internal class LSObject : IDisposable
    {

#region Public fields

        public Exception Exception;

#endregion

#region IDisposable

        public void Dispose()
        {
            LS.Pop();
        }

#endregion

    }
}

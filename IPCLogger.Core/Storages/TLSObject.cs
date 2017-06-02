using System;
using System.Collections;

namespace IPCLogger.Core.Storages
{
    public sealed class TLSObject : Hashtable, IDisposable
    {

#region Ctor

        internal TLSObject() { }

#endregion

#region IDisposable

        public void Dispose()
        {
            TLS.Pop();
        }

#endregion

    }
}

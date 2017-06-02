using System.Runtime.InteropServices;

namespace IPCLogger.Core.Loggers.LIPC.FileMap
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MapRingBufferHeader
    {

#region Public fields

        public int Count;
        public int MaxCount;
        public int CurrentIndex;
        public bool Updating;

#endregion

#region Ctor

        public MapRingBufferHeader(int maxCount)
        {
            Count = 0;
            MaxCount = maxCount;
            CurrentIndex = -1;
            Updating = false;
        }

#endregion

    }
}

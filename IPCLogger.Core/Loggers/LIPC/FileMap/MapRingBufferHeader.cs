﻿using System.Runtime.InteropServices;

namespace IPCLogger.Core.Loggers.LIPC.FileMap
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MapRingBufferHeader
    {

#region Public fields

        public int Count;
        public int MaxCount;
        public int CurrentIndex;
        public int CurrentItemPosition;
        public int CurrentItemSize;
        public bool Updating;

#endregion

#region Ctor

        public MapRingBufferHeader(int maxCount)
        {
            Count = 0;
            MaxCount = maxCount;
            CurrentIndex = -1;
            CurrentItemPosition = 0;
            CurrentItemSize = 0;
            Updating = false;
        }

#endregion

    }
}

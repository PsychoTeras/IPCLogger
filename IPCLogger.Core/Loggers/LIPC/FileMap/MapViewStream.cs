using System;
using System.Runtime.InteropServices;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Loggers.LIPC.FileMap
{
    internal sealed unsafe class MapViewStream : IDisposable
    {

#region Private fields

        private MemoryMappedFile _mmf;
        private IntPtr _viewBasePtr;
        private long _viewBaseAddr;
        private int _headerSize;
        private MapAccess _access;

#endregion

#region Properties

        public bool IsViewMapped
        {
            get { return _viewBasePtr != IntPtr.Zero; }
        }

#endregion

#region Ctor
        
        public MapViewStream(MemoryMappedFile mmf, MapProtection protection)
        {
            _mmf = mmf;
            bool isWriteable = (protection | MapProtection.PageReadWrite) == protection;
            _access = isWriteable ? MapAccess.FileMapWrite : MapAccess.FileMapRead;
            MapView();
        }

#endregion

#region Class methods

        private void MapView()
        {
            if (!IsViewMapped)
            {
                _headerSize = Marshal.SizeOf(typeof(MapRingBufferHeader));
                _viewBasePtr = Win32.MapViewOfFile(_mmf.Handle, _access, 0, 0, 0);
                if (_viewBasePtr == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
                _viewBaseAddr = _viewBasePtr.ToInt64();
            }
        }

        private void UnmapView()
        {
            if (IsViewMapped)
            {
                Win32.UnmapViewOfFile(_viewBasePtr);
                _viewBasePtr = IntPtr.Zero;
            }
        }

        public void Flush()
        {
            Win32.FlushViewOfFile(_viewBasePtr, 0);
        }

        public void FlushFromBeginning(int count)
        {
            Win32.FlushViewOfFile(_viewBasePtr, count);
        }

        public void FlushHeader()
        {
            Win32.FlushViewOfFile(_viewBasePtr, _headerSize);
        }

        public void* GetElemPtr(int position)
        {
            return (void*)(_viewBaseAddr + position);
        }

        public void Write(void* buffer, int position, int count)
        {
            Win32.Copy((void*)(_viewBaseAddr + position), buffer, count);
        }

        public void Dispose()
        {
            if (IsViewMapped)
            {
                Flush();
                UnmapView();
            }
        }

#endregion

    }

}

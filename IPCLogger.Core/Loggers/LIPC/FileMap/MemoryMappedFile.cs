using System;
using System.Runtime.InteropServices;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Loggers.LIPC.FileMap
{
    internal unsafe class MemoryMappedFile : IDisposable
    {

#region Constants

        private const int DEF_FILE_SIZE = 1024;
        private const int MAX_FILE_SIZE = int.MaxValue/4;

#endregion

#region Private fields

        private IntPtr _handle;
        private IntPtr _viewBasePtr;
        private long _viewBaseAddr;

#endregion

#region Properties

        public bool IsOpen
        {
            get { return _handle != IntPtr.Zero; }
        }

        public int Size { get; private set; }

#endregion

#region Static methods

        public static MemoryMappedFile Create(string name, MapProtection protection)
        {
            MemoryMappedFile map = new MemoryMappedFile();

            using (SecurityAttributes sa = SecurityAttributes.GetNullDacl())
            {
                protection |= MapProtection.SecReserve;
                map._handle = Win32.CreateFileMapping(new IntPtr(-1), sa, protection, 0, MAX_FILE_SIZE, name);
            }

            if (map._handle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            bool isWriteable = (protection | MapProtection.PageReadWrite) == protection;
            map.MapView(isWriteable ? MapAccess.FileMapWrite : MapAccess.FileMapRead);

            return map;
        }

        public static MemoryMappedFile Open(string name, MapAccess access)
        {
            MemoryMappedFile map = new MemoryMappedFile();
            map._handle = Win32.OpenFileMapping(access, false, name);
            if (map._handle != IntPtr.Zero)
            {
                map.MapView(access);
            }
            return map._handle == IntPtr.Zero ? null : map;
        }

#endregion

#region Ctor

        private MemoryMappedFile() { }

#endregion

#region Class methods

        private void MapView(MapAccess access)
        {
            _viewBasePtr = Win32.MapViewOfFile(_handle, access, 0, 0, 0);
            if (_viewBasePtr == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            _viewBaseAddr = _viewBasePtr.ToInt64();
        }

        private void UnmapView()
        {
            if (IsOpen)
            {
                Win32.UnmapViewOfFile(_viewBasePtr);
                _viewBasePtr = IntPtr.Zero;
                _viewBaseAddr = Size = 0;
            }
        }

        private void ReadFileSize()
        {
            MemoryBasicInformation info;
            int mbiSize = Marshal.SizeOf(typeof(MemoryBasicInformation));
            Win32.VirtualQuery(_viewBasePtr, out info, mbiSize);
            Size = info.RegionSize.ToInt32();
        }

        private void Expand(int size)
        {
            _viewBasePtr = Win32.VirtualAlloc(_viewBasePtr, size, AllocationType.Commit, MemoryProtection.ReadWrite);
            ReadFileSize();
        }

        public void Flush()
        {
            Win32.FlushViewOfFile(_viewBasePtr, 0);
        }

        public void FlushFromBeginning(int count)
        {
            Win32.FlushViewOfFile(_viewBasePtr, count);
        }

        public void* GetItemPtr(int position)
        {
            return (void*)(_viewBaseAddr + position);
        }

        public void Write(void* buffer, int position, int count)
        {
            if (position + count > Size)
            {
                Expand(Math.Max(Size, DEF_FILE_SIZE) *2);
            }
            Win32.Copy((void*)(_viewBaseAddr + position), buffer, count);
        }

        public void Dispose()
        {
            if (IsOpen)
            {
                Flush();
                UnmapView();
                Win32.CloseHandle(_handle);
            }
            _viewBaseAddr = Size = 0;
            _viewBasePtr = _handle = IntPtr.Zero;
        }

#endregion

    }
}

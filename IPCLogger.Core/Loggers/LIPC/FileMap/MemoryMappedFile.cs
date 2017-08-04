using System;
using System.Data;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Loggers.LIPC.FileMap
{
    internal class MemoryMappedFile : IDisposable
    {

#region Constants

        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

#endregion

#region Private fields

        private MapProtection _protection;

#endregion

#region Properties

        public int Size;
        
        public bool Is64Bit { get; }

        public IntPtr Handle { get; private set; }

        public bool IsOpen
        {
            get { return Handle != IntPtr.Zero; }
        }

#endregion

#region Ctor

        private MemoryMappedFile()
        {
            Is64Bit = IntPtr.Size == 8;
        }

#endregion

#region Static methods

        public static MemoryMappedFile Create(string name, long size, MapProtection protection)
        {
            MemoryMappedFile map = new MemoryMappedFile();
            if (!map.Is64Bit && size > uint.MaxValue)
            {
                throw new ConstraintException("32bit systems support max size of ~4gb");
            }

            using (SecurityAttributes sa = SecurityAttributes.GetNullDacl())
            {
                map.Handle = Win32.CreateFileMapping(InvalidHandleValue, sa,
                    protection, (int) ((size >> 32) & 0xFFFFFFFF), (int) (size & 0xFFFFFFFF),
                    name);
            }

            if (map.Handle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            map._protection = protection;
            map.Size = (int) size;

            return map;
        }

        public static MemoryMappedFile Open(string name, MapAccess access)
        {
            MemoryMappedFile map = new MemoryMappedFile
                                       {
                                           Handle = Win32.OpenFileMapping(access, false, name)
                                       };
            return map.Handle == IntPtr.Zero ? null : map;
        }

#endregion

#region Class methods

        public MapViewStream MapAsStream()
        {
            if (!IsOpen)
            {
                throw new ObjectDisposedException("Already closed");
            }
            return new MapViewStream(this, _protection);
        }

        public void Dispose()
        {
            if (IsOpen)
            {
                Win32.CloseHandle(Handle);
            }

            Handle = IntPtr.Zero;
        }

#endregion

    }
}

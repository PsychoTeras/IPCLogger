using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace IPCLogger.Core.Common
{
    [Flags]
    internal enum MapProtection
    {
        //Access
        PageNone = 0x00000000,
        PageReadOnly = 0x00000002,
        PageReadWrite = 0x00000004,
        PageWriteCopy = 0x00000008,
        
        //Params
        SecImage = 0x01000000,
        SecReserve = 0x04000000,
        SecCommit = 0x08000000,
        SecNoCache = 0x10000000,
    }

    internal enum MapAccess
    {
        FileMapCopy = 0x0001,
        FileMapWrite = 0x0002,
        FileMapRead = 0x0004,
        FileMapAllAccess = 0x001f,
    }

    [Flags]
    internal enum AllocationType
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Reset = 0x80000,
        Physical = 0x400000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        LargePages = 0x20000000
    }

    [Flags]
    public enum MemoryProtection
    {
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        GuardModifierflag = 0x100,
        NoCacheModifierflag = 0x200,
        WriteCombineModifierflag = 0x400
    }

    internal static unsafe class Win32
    {
        [DllImport("kernel32", EntryPoint = "CopyMemory")]
        public static extern void* Copy(void* dest, void* src, int count);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(
            IntPtr hFile, SecurityAttributes lpAttributes, MapProtection flProtect,
            int dwMaximumSizeLow, int dwMaximumSizeHigh, string lpName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FlushViewOfFile(IntPtr lpBaseAddress, int dwNumBytesToFlush);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject, MapAccess dwDesiredAccess, int dwFileOffsetHigh,
            int dwFileOffsetLow, int dwNumBytesToMap);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(
            MapAccess dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", SetLastError = true)]
        public static extern int VirtualQuery(IntPtr lpAddress, out MemoryBasicInformation lpBuffer, int dwLength);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, int dwSize, AllocationType flAllocationType,
            MemoryProtection flProtect);

        public static void* HeapAlloc(int size, bool zeroMem = true)
        {
            return Marshal.AllocHGlobal(size).ToPointer();
        }

        public static void HeapFree(void* block)
        {
            Marshal.FreeHGlobal(new IntPtr(block));
        }

        public static void* HeapReAlloc(void* block, int size, bool zeroMem = true)
        {
            return Marshal.ReAllocHGlobal(new IntPtr(block), new IntPtr(size)).ToPointer();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryBasicInformation
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public uint AllocationProtect;
        public IntPtr RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class SecurityAttributes : IDisposable
    {
        public int Length;
        public IntPtr SecurityDescriptor;
        public int InheritHandle;

        public static SecurityAttributes GetNullDacl()
        {
            SecurityAttributes sa = new SecurityAttributes();
            sa.Length = Marshal.SizeOf(sa);
            sa.InheritHandle = 1;

            RawSecurityDescriptor gsd = new RawSecurityDescriptor(ControlFlags.DiscretionaryAclPresent,
                null, null, null, null);

            byte[] desc = new byte[gsd.BinaryLength];
            gsd.GetBinaryForm(desc, 0);
            sa.SecurityDescriptor = Marshal.AllocHGlobal(desc.Length);
            Marshal.Copy(desc, 0, sa.SecurityDescriptor, desc.Length);

            return sa;
        }

        public void Dispose()
        {
            if (SecurityDescriptor != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(SecurityDescriptor);
                SecurityDescriptor = IntPtr.Zero;
            }
        }
    }

    [Serializable]
    internal class Win32Exception : IOException
    {

#region Properties

        public int Win32ErrorCode { get; }

        public override string Message
        {
            get
            {
                if (Win32ErrorCode != 0)
                {
                    return base.Message + " (" + Win32ErrorCode + ")";
                }
                return base.Message;
            }
        }

#endregion

#region Ctor

        public Win32Exception()
        {
            Win32ErrorCode = Marshal.GetHRForLastWin32Error();
        }

#endregion

    }
}
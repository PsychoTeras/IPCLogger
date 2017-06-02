using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace IPCLogger.Core.Loggers.LIPC.FileMap
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

    internal static unsafe class Win32
    {
        [DllImport("msvcrt.dll", EntryPoint = "memmove", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* Move(void* dest, void* src, int count);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(
            IntPtr hFile, SECURITY_ATTRIBUTES lpAttributes, MapProtection flProtect,
            int dwMaximumSizeLow, int dwMaximumSizeHigh, string lpName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FlushViewOfFile(IntPtr lpBaseAddress, int dwNumBytesToFlush);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FlushViewOfFile(void* lpBaseAddress, int dwNumBytesToFlush);

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
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class SECURITY_ATTRIBUTES : IDisposable
    {
        public int Length;
        public IntPtr SecurityDescriptor;
        public int InheritHandle;

        public static SECURITY_ATTRIBUTES GetNullDacl()
        {
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
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

        public int Win32ErrorCode { get; private set; }

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
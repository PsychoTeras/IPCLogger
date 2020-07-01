using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace IPCLogger.Common
{
    [Flags]
    enum MapProtection
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

    enum MapAccess
    {
        FileMapCopy = 0x0001,
        FileMapWrite = 0x0002,
        FileMapRead = 0x0004,
        FileMapAllAccess = 0x001f,
    }

    [Flags]
    enum AllocationType
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
    enum MemoryProtection
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

    [StructLayout(LayoutKind.Sequential)]
    struct MemoryBasicInformation
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
    class SecurityAttributes : IDisposable
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
    class Win32Exception : IOException
    {
        private string _message;
        public int Win32ErrorCode { get; }

        public override string Message
        {
            get
            {
                if (Win32ErrorCode != 0)
                {
                    return _message + " Error code: " + Win32ErrorCode;
                }
                return _message;
            }
        }

        public Win32Exception()
        {
            _message = base.Message;
            Win32ErrorCode = Marshal.GetHRForLastWin32Error();
        }

        public Win32Exception(string message, int errorCode)
        {
            _message = message;
            Win32ErrorCode = errorCode;
        }
    }

    static unsafe class Win32
    {
        [DllImport("ntdll", EntryPoint = "RtlZeroMemory")]
        public static extern void* Zero(void* dest, int count);

        [DllImport("ntdll", EntryPoint = "RtlCopyMemory")]
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

    static class NetworkConnection
    {
        class ConnectionManager
        {
            private static ConnectionManager _instance = new ConnectionManager();
            private readonly Dictionary<string, int> _serverList = new Dictionary<string, int>();

            public static ConnectionManager Instance
            {
                get { return _instance; }
            }

            public void Connect(string server)
            {
                lock (_serverList)
                {
                    server = server.ToLower();
                    if (!_serverList.ContainsKey(server))
                    {
                        _serverList.Add(server, 1);
                    }
                    else
                    {
                        _serverList[server]++;
                    }
                }
            }

            public bool Disconnect(string server)
            {
                lock (_serverList)
                {
                    server = server.ToLower();
                    if (!_serverList.ContainsKey(server))
                    {
                        return false;
                    }
                    if (--_serverList[server] == 0)
                    {
                        _serverList.Remove(server);
                    }
                    return true;
                }
            }
        }

        [DllImport("netapi32", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int NetUseAdd(string uncServerName, uint level, ref UseInfo2 ui, IntPtr paramError);

        [DllImport("netapi32", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int NetUseDel(string uncServerName, string useName, uint forceCond);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct UseInfo2
        {
            public string local;
            public string remote;
            public string password;
            public uint status;
            public uint asgtype;
            public uint refcount;
            public uint usecount;
            public string username;
            public string domainname;
        }

        private static Regex _regexUNCPath = new Regex(@"""*(?<SHARE>\\\\.+?\\.+?)(\\|""|$)",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static Regex _regexUserName = new Regex(@"((?<U>.+?)@(?<D>.+?)|(?<D>.+?)\\(?<U>.+?)|(?<U>.+?))$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public static void ConnectShare(string share, string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(share))
            {
                throw new Win32Exception("Failed to connect share. Invalid parameters.", 0);
            }

            share = _regexUNCPath.Match(share).Groups["SHARE"].Value;
            if (string.IsNullOrEmpty(share))
            {
                throw new Win32Exception("Failed to connect share. Invalid parameters.", 0);
            }

            UseInfo2 useInfo = new UseInfo2
            {
                remote = share,
                password = password ?? string.Empty
            };

            Match match = _regexUserName.Match(username);
            useInfo.username = match.Groups["U"].Value;
            useInfo.domainname = match.Groups["D"].Value;

            int result = NetUseAdd(null, 2, ref useInfo, IntPtr.Zero);
            if (result != 0)
            {
                throw new Win32Exception("Failed to connect share.", result);
            }

            ConnectionManager.Instance.Connect(share);
        }

        public static void DisconnectShare(string share, string username)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(share))
            {
                share = _regexUNCPath.Match(share).Groups["SHARE"].Value;
                if (ConnectionManager.Instance.Disconnect(share))
                {
                    NetUseDel(null, share, 2);
                }
            }
        }
    }
}
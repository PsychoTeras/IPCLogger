using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IPCLogger.TestService
{
    public static class ServiceHelper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        const int STD_OUTPUT_HANDLE = -11;

        public static bool IsBeingRunAsService()
        {
            return GetStdHandle(STD_OUTPUT_HANDLE) == IntPtr.Zero;
        }

        public static FileVersionInfo FileVersionInfo
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return FileVersionInfo.GetVersionInfo(assembly.Location);
            }
        }
    }
}

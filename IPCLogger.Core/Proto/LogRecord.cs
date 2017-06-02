using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace IPCLogger.Core.Proto
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LogRecord
    {

#region Static private fields

        private static int _idShift = Environment.TickCount;
        
#endregion

#region Public fields

        public int Id;

        public int Type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
        public string Message;

        public bool IsEmpty
        {
            get { return Id == 0; }
        }

#endregion

#region Ctor

        public void Setup(int type, string message)
        {
            unchecked
            {
                Id = Interlocked.Increment(ref _idShift);
            }
            Message = message;
            Type = type;
        }

#endregion

    }
}

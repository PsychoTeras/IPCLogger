using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using IPCLogger.Core.Common;
using IPCLogger.Core.Loggers.Base;

namespace IPCLogger.Core.Loggers.LSysDebug
{
    public sealed class LSysDebug : BaseLogger<LSysDebugSettings>
    {

#region P/Invoke

        [DllImport("kernel32.dll")]
        private static extern void OutputDebugString(string message);

#endregion

#region ILogger

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected internal override void Write(Type callerType, Enum eventType, string eventName, 
            string text, bool writeLine)
        {
            if (writeLine) text += Constants.NewLine;
            OutputDebugString(text);
        }

        public override void Initialize() { }

        public override void Deinitialize() { }

#endregion

    }
}

using IPCLogger.Common;
using System;
using System.Threading;

namespace IPCLogger.Proto
{
    public unsafe struct LogItem : ISerializable
    {

#region Static private fields

        private char* _pMessage;
        private static readonly int _maxMessageLength = 8192 * sizeof(char);
        private static int _idShift = Environment.TickCount;
        
#endregion

#region Public fields

        public int Id;

        public int Type;

        public string Message { get; private set; }

        public bool IsEmpty
        {
            get { return Id == 0; }
        }

#endregion

#region Class methods

        public void Setup(int type, string message)
        {
            unchecked
            {
                Id = Interlocked.Increment(ref _idShift);
            }
            Type = type;
            if (_pMessage == null)
            {
                _pMessage = (char*)Win32.HeapAlloc(_maxMessageLength);
            }
            Win32.Zero(_pMessage, _maxMessageLength);
            if (message != null)
            {
                fixed (char* p = message)
                {
                    int messageLength = message.Length * sizeof(char);
                    if (messageLength > _maxMessageLength - sizeof(char))
                    {
                        messageLength = _maxMessageLength - sizeof(char);
                    }
                    Win32.Copy(_pMessage, p, messageLength);
                }
            }
            Message = message;
        }

        public int SizeOf
        {
            get
            {
                return
                    sizeof(int) +
                    sizeof(int) +
                    sizeof(int) + _maxMessageLength;
            }
        }

        public unsafe void Serialize(byte* bData, ref int pos)
        {
            Serializer.Write(bData, Id, ref pos);
            Serializer.Write(bData, Type, ref pos);
            Serializer.Write(bData, _pMessage, _maxMessageLength, ref pos);
        }

        public unsafe void Deserialize(byte* bData, ref int pos)
        {
            Serializer.Read(bData, out Id, ref pos);
            Serializer.Read(bData, out Type, ref pos);
            Serializer.Read(bData, out void* message, ref pos);
            Message = new string((char*)message);
            Win32.HeapFree(message);
        }

#endregion

    }
}

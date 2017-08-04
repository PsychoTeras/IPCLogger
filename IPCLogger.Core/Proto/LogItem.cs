using System;
using System.Threading;

namespace IPCLogger.Core.Proto
{
    public struct LogItem : ISerializable
    {

#region Static private fields

        private static int _idShift = Environment.TickCount;
        
#endregion

#region Public fields

        public int Id;

        public int Type;

        public string Message;

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
            Message = message;
            Type = type;
        }

        public int SizeOf
        {
            get
            {
                return
                    sizeof(int) +
                    sizeof(int) +
                    sizeof(int) + (!string.IsNullOrEmpty(Message) ? Message.Length : 0);
            }
        }

        public unsafe void Serialize(byte* bData, ref int pos)
        {
            Serializer.Write(bData, Id, ref pos);
            Serializer.Write(bData, Type, ref pos);
            Serializer.Write(bData, Message, ref pos);
        }

        public unsafe void Deserialize(byte* bData, ref int pos)
        {
            Serializer.Read(bData, out Id, ref pos);
            Serializer.Read(bData, out Type, ref pos);
            Serializer.Read(bData, out Message, ref pos);
        }

#endregion

    }
}

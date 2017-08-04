namespace IPCLogger.Core.Proto
{
    public unsafe interface ISerializable
    {
        int SizeOf { get; }
        void Serialize(byte* bData, ref int pos);
        void Deserialize(byte* bData, ref int pos);
    }
}

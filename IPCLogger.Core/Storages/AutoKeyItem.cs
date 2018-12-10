using System.Threading;

namespace IPCLogger.Core.Storages
{
    internal class AutoKeyItem
    {
        public int InitValue;
        public int Value;
        public int Increment;
        public string Format;

        public AutoKeyItem(int initValue, int increment, string format)
        {
            InitValue = Value = initValue;
            Increment = increment;
            Format = format;
        }

        public string GetAndIncrease()
        {
            int value = Interlocked.Add(ref Value, Increment) - Increment;
            return Format != null ? value.ToString(Format) : value.ToString();
        }

        public void Reset()
        {
            Value = InitValue;
        }
    }
}
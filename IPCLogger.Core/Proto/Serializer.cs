using System.Collections.Generic;
using IPCLogger.Core.Common;

namespace IPCLogger.Core.Proto
{
    internal static unsafe class Serializer
    {
        public static int SizeOf(byte[] data)
        {
            const int iSize = sizeof(int);
            return data != null ? iSize + data.Length : iSize;
        }

        public static int SizeOf(string str)
        {
            const int sSize = sizeof(int);
            int strLen = str == null ? -1 : str.Length * sizeof(char);
            return strLen > 0 ? sSize + strLen : sSize;
        }

        public static int SizeOf(ISerializable obj)
        {
            const int bNotNull = sizeof(bool);
            return obj != null ? bNotNull + obj.SizeOf : bNotNull;
        }

        // ReSharper disable once PossibleNullReferenceException
        public static int SizeOf<T>(IList<T> collection) 
            where T : ISerializable
        {
            const int iSize = sizeof(int);
            int elsCount = collection == null ? -1 : collection.Count, elsSize = 0;
            for (int i = 0; i < elsCount; i++)
            {
                elsSize += collection[i].SizeOf;
            }
            return iSize + elsSize;
        }

        public static void Read(byte* bData, out long val, ref int pos)
        {
            val = *(long*)&bData[pos];
            pos += sizeof(long);
        }

        public static void Read(byte* bData, out int val, ref int pos)
        {
            val = *(int*)&bData[pos];
            pos += sizeof(int);
        }

        public static void Read(byte* bData, out short val, ref int pos)
        {
            val = *(short*)&bData[pos];
            pos += sizeof(short);
        }

        public static void Read(byte* bData, out ushort val, ref int pos)
        {
            val = *(ushort*)&bData[pos];
            pos += sizeof(ushort);
        }

        public static void Read(byte* bData, out byte val, ref int pos)
        {
            val = bData[pos];
            pos += sizeof(byte);
        }

        public static void Read(byte* bData, out float val, ref int pos)
        {
            val = *(float*)&bData[pos];
            pos += sizeof(float);
        }

        public static void Read(byte* bData, out byte[] data, ref int pos)
        {
            int size = *(int*)&bData[pos];
            pos += sizeof(int);
            if (size == -1)
                data = null;
            else
            {
                data = new byte[size];
                if (size > 0)
                {
                    fixed (byte* d = data) Win32.Copy(d, &bData[pos], size);
                }
                pos += size;
            }
        }

        public static void Read(byte* bData, out string str, ref int pos)
        {
            int strLen = *(int*)&bData[pos];
            pos += sizeof(int);
            if (strLen == -1)
                str = null;
            else if (strLen == 0)
                str = "";
            else
            {
                str = new string((char*)&bData[pos], 0, strLen / sizeof(char));
                pos += strLen;
            }
        }

        public static void Read<T>(byte* bData, out T obj, ref int pos)
            where T : ISerializable, new()
        {
            bool bNotNull = *(bool*)&bData[pos];
            pos += sizeof(bool);
            if (bNotNull)
            {
                obj = new T();
                obj.Deserialize(bData, ref pos);
            }
            else
                obj = default(T);
        }

        public static void Read<T, TC>(byte* bData, out TC collection, ref int pos)
            where T : ISerializable, new() where TC: class, IList<T>, new()
        {
            int elsCount = *(int*)&bData[pos];
            pos += sizeof (int);
            if (elsCount == -1)
                collection = null;
            else
            {
                collection = new TC();
                for (int i = 0; i < elsCount; i++)
                {
                    T obj = new T();
                    obj.Deserialize(bData, ref pos);
                    collection.Add(obj);
                }
            }
        }

        public static void Write(byte* bData, long val, ref int pos)
        {
            *(long*)&bData[pos] = val;
            pos += sizeof(long);
        }

        public static void Write(byte* bData, int val, ref int pos)
        {
            *(int*)&bData[pos] = val;
            pos += sizeof (int);
        }

        public static void Write(byte* bData, short val, ref int pos)
        {
            *(short*)&bData[pos] = val;
            pos += sizeof(short);
        }

        public static void Write(byte* bData, ushort val, ref int pos)
        {
            *(ushort*)&bData[pos] = val;
            pos += sizeof(ushort);
        }

        public static void Write(byte* bData, byte val, ref int pos)
        {
            bData[pos] = val;
            pos += sizeof(byte);
        }

        public static void Write(byte* bData, float val, ref int pos)
        {
            *(float*)&bData[pos] = val;
            pos += sizeof(float);
        }
        
        public static void Write(byte* bData, byte[] data, ref int pos)
        {
            int size = data == null ? -1 : data.Length;
            *(int*)&bData[pos] = size;
            pos += sizeof(int);
            if (size > 0)
            {
                fixed (byte* d = data) Win32.Copy(&bData[pos], d, size);
                pos += size;
            }
        }

        public static void Write(byte* bData, string str, ref int pos)
        {
            int strLen = str == null ? -1 : str.Length * sizeof(char);
            *(int*)&bData[pos] = strLen;
            pos += sizeof(int);
            if (strLen > 0)
            {
                fixed (void* c = str) Win32.Copy(&bData[pos], c, strLen);
                pos += strLen;
            }
        }

        public static void Write<T>(byte* bData, T obj, ref int pos)
            where T : class, ISerializable
        {
            bool bNotNull = obj != null;
            *(bool*)&bData[pos] = bNotNull;
            pos += sizeof(bool);
            if (bNotNull)
            {
                obj.Serialize(bData, ref pos);
            }
        }

        // ReSharper disable once PossibleNullReferenceException
        public static void Write<T, TC>(byte* bData, TC collection, ref int pos)
            where T : ISerializable where TC : class, IList<T>
        {
            int elsCount = collection == null ? -1 : collection.Count;
            *(int*)&bData[pos] = elsCount;
            pos += sizeof(int);
            for (int i = 0; i < elsCount; i++)
            {
                collection[i].Serialize(bData, ref pos);
            }
        }
    }
}

using System;

namespace IPCLogger.Core.Loggers.LDB.DAL
{
    public class ColumnInfo
    {
        private Type _type;

        public string Name;
        public byte TypeId;
        public bool IsIdentity;
        public bool IsNullable;

        public Type Type
        {
            get
            {
                if (_type == null) switch (TypeId)
                {
                    case 35:
                    case 99:
                    case 167:
                    case 175:
                    case 231:
                    case 239:
                    case 241:
                        return _type = typeof(string);
                    case 36:
                        return _type = typeof(Guid);
                    case 40:
                    case 42:
                    case 58:
                    case 61:
                        return _type = typeof(DateTime);
                    case 41:
                        return _type = typeof(TimeSpan);
                    case 43:
                        return _type = typeof(DateTimeOffset);
                    case 48:
                        return _type = typeof(byte);
                    case 52:
                        return _type = typeof(short);
                    case 56:
                        return _type = typeof(int);
                    case 59:
                        return _type = typeof(float);
                    case 60:
                    case 106:
                    case 108:
                        return _type = typeof(decimal);
                    case 62:
                        return _type = typeof(double);
                    case 104:
                        return _type = typeof(bool);
                    case 127:
                        return _type = typeof(long);
                    case 34:
                    case 165:
                    case 173:
                    case 189:
                        return _type = typeof(byte[]);
                    default:
                        return _type = null;
                }
                return _type;
            }
        }
    }
}

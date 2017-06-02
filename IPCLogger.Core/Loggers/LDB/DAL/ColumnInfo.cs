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
                    case 40:
                    case 61:
                        _type = typeof(DateTime);
                        break;
                    case 48:
                        _type = typeof(byte);
                        break;
                    case 52:
                        _type = typeof(short);
                        break;
                    case 56:
                        _type = typeof(int);
                        break;
                    case 59:
                    case 62:
                        _type = typeof(double);
                        break;
                    case 104:
                        _type = typeof(bool);
                        break;
                    case 106:
                        _type = typeof(decimal);
                        break;
                    case 167:
                    case 231:
                        _type = typeof(string);
                        break;
                    default:
                        _type = null;
                        break;
                }
                return _type;
            }
        }
    }
}

using System;

namespace IPCLogger.Core.Storages
{
    sealed class FuncObject
    {
        public Delegate Delegate;
        public string ObjName;
        public Type ObjType;

        public FuncObject(Delegate @delegate, string objName, Type objType)
        {
            Delegate = @delegate;
            ObjName = objName;
            ObjType = objType;
        }
    }
}

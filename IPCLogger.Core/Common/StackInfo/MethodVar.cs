using System;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace IPCLogger.Core.Common.StackInfo
{
    internal enum MethodVarType
    {
        Ref,
        Val
    }

    internal sealed unsafe class MethodVar
    {
        public string Name;
        public Type Type;
        public object Value;

        private static Type GetParamType(TypeReference typeRef)
        {
            string typeName = string.Format("{0}, {1}", typeRef.FullName, typeRef.Scope);
            return Type.GetType(typeName);
        }

        private static MethodVar ReadFromStackByRef(string varName, ref void* stackPtr,
            TypeReference typeRef)
        {
            TypedReference reference = new TypedReference();

            bool objIsNotNull;
            if (StackInfo.Is64Bit)
            {
                long* pReference = (long*) &reference;
                pReference[0] = (long) stackPtr; //Value* - pointer, that contains address of
                //the .NET class instance in the .NET Objects Table
                long* iObjAddr = *(long**) (long) stackPtr;
                objIsNotNull = iObjAddr != null;
                if (objIsNotNull)
                {
                    pReference[1] = *iObjAddr; //**(long**) (long) stackPtr
                    //RuntimeType* - pointer, that contais reference on the RuntimeType of the object.
                    //First 8 bytes (long) of Value
                }
            }
            else
            {
                int* pReference = (int*)&reference;
                pReference[0] = (int)stackPtr; //Value* - pointer, that contains address of
                //the .NET class instance in the .NET Objects Table
                int* iObjAddr = *(int**)(int)stackPtr;
                objIsNotNull = iObjAddr != null;
                if (objIsNotNull)
                {
                    pReference[1] = *iObjAddr; //**(int**) (int) stackPtr
                    //RuntimeType* - pointer, that contais reference on the RuntimeType of the object.
                    //First 4 bytes (int) of Value
                }
            }

            stackPtr = (byte*) stackPtr - IntPtr.Size;

            return new MethodVar
            {
                Name = varName,
                Type = TypedReference.GetTargetType(reference),
                Value = objIsNotNull ? TypedReference.ToObject(reference) : null
            };
        }

        private static MethodVar ReadFromStackByVal(string varName, ref void* stackPtr,
            TypeReference typeRef)
        {
            Type paramType = GetParamType(typeRef);

            TypedReference reference = new TypedReference();

            if (StackInfo.Is64Bit)
            {
                long* pReference = (long*)&reference;
                pReference[0] = (long)stackPtr;
                pReference[1] = (long)paramType.TypeHandle.Value;
            }
            else
            {
                int* pReference = (int*)&reference;
                pReference[0] = (int)stackPtr;
                pReference[1] = (int)paramType.TypeHandle.Value;
            }

            int size = Marshal.SizeOf(paramType);
            size += size % IntPtr.Size;
            stackPtr = (byte*)stackPtr - Math.Max(size, IntPtr.Size);

            return new MethodVar
            {
                Name = varName,
                Type = TypedReference.GetTargetType(reference),
                Value = TypedReference.ToObject(reference)
            };
        }

        public static MethodVar ReadFromStack(string varName, ref void* stackPtr,
            TypeReference typeRef)
        {
            return typeRef.IsValueType
                ? ReadFromStackByVal(varName, ref stackPtr, typeRef)
                : ReadFromStackByRef(varName, ref stackPtr, typeRef);
        }

        public static void Skip(ref void* stackPtr, TypeReference typeRef)
        {
            if (typeRef.IsValueType)
            {
                Type paramType = GetParamType(typeRef);
                int size = Marshal.SizeOf(paramType);
                size += size%IntPtr.Size;
                stackPtr = (byte*) stackPtr - Math.Max(size, IntPtr.Size);
            }
            else
            {
                stackPtr = (byte*) stackPtr - IntPtr.Size;
            }
        }

        public static int SizeOf(TypeReference typeRef)
        {
            if (typeRef.IsValueType)
            {
                Type paramType = GetParamType(typeRef);
                return Marshal.SizeOf(paramType);
            }
            return IntPtr.Size;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}] = {2}", Name, Type != null ? Type.Name : "?", Value ?? "<NULL>");
        }
    }
}

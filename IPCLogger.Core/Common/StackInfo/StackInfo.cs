using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace IPCLogger.Core.Common.StackInfo
{
    static unsafe class StackInfo
    {
        public static readonly bool Is64Bit = IntPtr.Size == 8;
        static readonly long StackEBPShift = Debugger.IsAttached ? 60 : sizeof(int);

        public static MethodVarsInfo RetreiveMethodVarsInfo(int stackLevel, MethodBase mBase)
        {
            long iEbp;
            if (Is64Bit)
            {
                iEbp = (long)&stackLevel + StackEBPShift;
                for (int i = 0; i < stackLevel; i++)
                {
                    iEbp = *(long*)iEbp;
                }
            }
            else
            {
                iEbp = (int)&stackLevel + StackEBPShift;
                for (int i = 0; i < stackLevel; i++)
                {
                    iEbp = *(int*)iEbp;
                }
            }

            void* stackPtr = (void*)(iEbp - StackEBPShift);

            MethodVarsInfo result = new MethodVarsInfo();

            mBase = mBase ?? new StackFrame(stackLevel).GetMethod();

            //ReSharper disable once PossibleNullReferenceException
            AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(mBase.DeclaringType.Assembly.Location);
            asm.MainModule.ReadSymbols();
            MethodDefinition method = asm.MainModule.Types.
                First(t => t.FullName.Equals(mBase.DeclaringType.FullName)).
                Methods.First(m => m.Name == mBase.Name);

            ParameterDefinition[] mParams = method.Parameters.ToArray();
            result.MethodParams = new List<MethodVar>(mParams.Length);
            foreach (ParameterDefinition var in mParams)
            {
                MethodVar mv = MethodVar.ReadFromStack(var.Name, ref stackPtr, var.ParameterType, var.IsOut);
                result.MethodParams.Add(mv);
            }

            VariableDefinition[] mVars = method.Body.Variables.ToArray();
            result.MethodVars = new List<MethodVar>(mVars.Length);
            foreach (VariableDefinition var in mVars)
            {
                if (var.Name != string.Empty)
                {
                    MethodVar mv = MethodVar.ReadFromStack(var.Name, ref stackPtr, var.VariableType, false);
                    result.MethodVars.Add(mv);
                }
            }

            return result;
        }
    }
}

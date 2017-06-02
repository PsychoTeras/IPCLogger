using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using IPCLogger.Core.Common;
using IPCLogger.Core.Patterns;
using IPCLogger.Core.Snippets.Base;

namespace IPCLogger.Core.Snippets.Template
{
    internal sealed class SStack : BaseSnippet
    {

#region Constants

        private const bool DEF_DETAILED = false;
        private const ushort DEF_STACK_LEVEL = ushort.MaxValue;

#endregion

#region Properties

        public override string[] Names
        {
            get
            {
                return new[]
                {
                     "srcline"
                    ,"srcfile"
                    ,"stack"
                };
            }
        }

#endregion

#region Ctor

        public SStack() : base(SnippetType.Template) { }

#endregion

#region Class methods

        private StackFrame FindCallerStackFrame(bool getFileInfo)
        {
            StackTrace stack = new StackTrace(getFileInfo);
            int firstFrame = Helpers.FindCallerStackLevel(stack);
            return stack.GetFrame(firstFrame);
        }

        private string GetStackInfo(ref string @params)
        {
            StringBuilder result = new StringBuilder();

            SnippetParams sParams = SnippetParams.Parse(@params);
            int level = sParams.GetValue("level", DEF_STACK_LEVEL);
            bool detailed = sParams.HasValue("detailed", DEF_DETAILED);

            StackTrace stack = new StackTrace(detailed);
            int firstFrame = Helpers.FindCallerStackLevel(stack);
            int minStackLevel = Math.Min(firstFrame + level, stack.FrameCount);

            for (int i = firstFrame; i < minStackLevel; i++)
            {
                StackFrame frame = stack.GetFrame(i);
                MethodBase method = frame.GetMethod();
                Type declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    result.AppendFormat(" > [???] {0}{1}", method.Name, Constants.NewLine);
                }
                else
                {
                    string assemblyName = Path.GetFileName(declaringType.Assembly.Location);
                    result.AppendFormat(" > [{0}] {1}.{2}{3}", assemblyName, declaringType.FullName,
                        method.Name, !detailed ? Constants.NewLine : "(");
                    if (detailed)
                    {
                        ParameterInfo[] paramaters = method.GetParameters();
                        for (int j = 0; j < paramaters.Length; j++)
                        {
                            ParameterInfo parameter = paramaters[j];

                            string prefix = string.Empty;
                            string parameterType = parameter.ParameterType.Name;
                            if (parameter.IsOut)
                            {
                                prefix = "out ";
                            }
                            else if (parameter.ParameterType.IsByRef)
                            {
                                prefix = "ref ";
                                parameterType = parameterType.Remove(parameterType.Length - 1);
                            }

                            result.AppendFormat("{0}{1}{2} {3}", j > 0 ? ", " : string.Empty, prefix, 
                                parameterType, parameter.Name);
                        }
                        result.AppendFormat(") Line {0}{1}", frame.GetFileLineNumber(),
                            i < minStackLevel - 1 ? Constants.NewLine : string.Empty);
                    }
                }
            }

            return result.ToString().TrimEnd();
        }

        public override string Process(Type callerType, Enum eventType, string snippetName, 
            string text, string @params, PFactory pFactory)
        {
            StackFrame stackFrame;
            switch (snippetName)
            {
                case "srcline":
                    stackFrame = FindCallerStackFrame(true);
                    return stackFrame.GetFileLineNumber().ToString(@params);
                case "srcfile":
                    stackFrame = FindCallerStackFrame(true);
                    return stackFrame.GetFileName();
                case "stack":
                    return GetStackInfo(ref @params);
            }
            return null;
        }

#endregion

    }
}

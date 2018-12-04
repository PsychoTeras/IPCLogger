using System;

namespace IPCLogger.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NonSettingAttribute : Attribute { }
}

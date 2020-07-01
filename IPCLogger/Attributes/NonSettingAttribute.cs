using System;

namespace IPCLogger.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NonSettingAttribute : Attribute { }
}

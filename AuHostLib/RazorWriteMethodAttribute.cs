using System;

namespace AuHost.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RazorWriteMethodAttribute : Attribute { }
}
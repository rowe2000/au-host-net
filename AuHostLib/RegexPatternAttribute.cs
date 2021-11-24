using System;

namespace AuHost.Annotations
{
    /// <summary>
    /// Indicates that the marked parameter, field, or property is a regular expression pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RegexPatternAttribute : Attribute { }
}
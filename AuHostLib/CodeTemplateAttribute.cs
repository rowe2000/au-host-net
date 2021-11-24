using System;

namespace AuHost.Annotations
{
    /// <summary>
    /// <para>
    /// Defines the code search template using the Structural Search and Replace syntax.
    /// It allows you to find and, if necessary, replace blocks of code that match a specific pattern.
    /// Search and replace patterns consist of a textual part and placeholders.
    /// Textural part must contain only identifiers allowed in the target language and will be matched exactly (white spaces, tabulation characters, and line breaks are ignored).
    /// Placeholders allow matching variable parts of the target code blocks.
    /// A placeholder has the following format: $placeholder_name$- where placeholder_name is an arbitrary identifier.
    /// </para>
    /// <para>
    /// Available placeholders:
    /// <list type="bullet">
    /// <item>$this$ - expression of containing type</item>
    /// <item>$thisType$ - containing type</item>
    /// <item>$member$ - current member placeholder</item>
    /// <item>$qualifier$ - this placeholder is available in the replace pattern and can be used to insert qualifier expression matched by the $member$ placeholder.
    /// (Note that if $qualifier$ placeholder is used, then $member$ placeholder will match only qualified references)</item>
    /// <item>$expression$ - expression of any type</item>
    /// <item>$args$ - any number of arguments</item>
    /// <item>$arg$ - single argument</item>
    /// <item>$arg1$ ... $arg10$ - single argument</item>
    /// <item>$stmts$ - any number of statements</item>
    /// <item>$stmt$ - single statement</item>
    /// <item>$stmt1$ ... $stmt10$ - single statement</item>
    /// <item>$name{Expression, 'Namespace.FooType'}$ - expression with 'Namespace.FooType' type</item>
    /// <item>$expression{'Namespace.FooType'}$ - expression with 'Namespace.FooType' type</item>
    /// <item>$name{Type, 'Namespace.FooType'}$ - 'Namespace.FooType' type</item>
    /// <item>$type{'Namespace.FooType'}$ - 'Namespace.FooType' type</item>
    /// <item>$statement{1,2}$ - 1 or 2 statements</item>
    /// </list>
    /// </para>
    /// <para>
    /// For more information please refer to the <a href="https://www.jetbrains.com/help/resharper/Navigation_and_Search__Structural_Search_and_Replace.html">Structural Search and Replace</a> article.
    /// </para>
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method
        | AttributeTargets.Constructor
        | AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Event
        | AttributeTargets.Interface
        | AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Enum,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class CodeTemplateAttribute : Attribute
    {
        public CodeTemplateAttribute(string searchTemplate)
        {
            SearchTemplate = searchTemplate;
        }

        /// <summary>
        /// Structural search pattern to use in the code template.
        /// Pattern includes textual part, which must contain only identifiers allowed in the target language,
        /// and placeholders, which allow matching variable parts of the target code blocks.
        /// </summary>
        public string SearchTemplate { get; }

        /// <summary>
        /// Message to show when the search pattern was found.
        /// You can also prepend the message text with "Error:", "Warning:", "Suggestion:" or "Hint:" prefix to specify the pattern severity.
        /// Code patterns with replace template produce suggestions by default.
        /// However, if replace template is not provided, then warning severity will be used.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Structural search replace pattern to use in code template replacement.
        /// </summary>
        public string ReplaceTemplate { get; set; }

        /// <summary>
        /// Replace message to show in the light bulb.
        /// </summary>
        public string ReplaceMessage { get; set; }

        /// <summary>
        /// Apply code formatting after code replacement.
        /// </summary>
        public bool FormatAfterReplace { get; set; } = true;

        /// <summary>
        /// Whether similar code blocks should be matched.
        /// </summary>
        public bool MatchSimilarConstructs { get; set; }

        /// <summary>
        /// Automatically insert namespace import directives or remove qualifiers that become redundant after the template is applied.
        /// </summary>
        public bool ShortenReferences { get; set; }

        /// <summary>
        /// String to use as a suppression key.
        /// By default the following suppression key is used 'CodeTemplate_SomeType_SomeMember',
        /// where 'SomeType' and 'SomeMember' are names of the associated containing type and member to which this attribute is applied.
        /// </summary>
        public string SuppressionKey { get; set; }
    }
}
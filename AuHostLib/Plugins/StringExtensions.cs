using System.Linq;

namespace AuHost.Plugins
{
    public static class StringExtensions
    {
        public static bool ContainsOnly(this string text, string p)
        {
            return text.All(c => p.Contains(c));
        }

        
    }
}
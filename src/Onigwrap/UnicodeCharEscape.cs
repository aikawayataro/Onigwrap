using System;
using System.Text.RegularExpressions;

namespace Onigwrap
{
    public partial class UnicodeCharEscape
    {
        private const string WithoutBracesPattern = "\\\\x[A-Fa-f0-9]{2,8}";
        private static readonly Regex UNICODE_WITHOUT_BRACES_PATTERN =

#if NET8_0_OR_GREATER
            WithoutBracesRegex();

        [GeneratedRegex(WithoutBracesPattern)]
        private static partial Regex WithoutBracesRegex();
#else
            new Regex(WithoutBracesPattern);
#endif

        public static string AddBracesToUnicodePatterns(string pattern)
        {
            return UNICODE_WITHOUT_BRACES_PATTERN.Replace(pattern, (m) =>
            {
                const string prefix = "\\x";

                var substr =
#if NET8_0_OR_GREATER
                    m.Value.AsSpan(prefix.Length);
#else
                    m.Value.Substring(prefix.Length);
#endif

                return string.Concat(prefix, "{", substr, "}");
            });
        }

        internal static string ConstraintUnicodePatternLenght(string pattern)
        {
            // php grammar has this kind of unicode chars, and
            // oniguruma library doesn't like them
            return pattern.Replace("\\x{7fffffff}", "\\x{7ffff}");
        }
    }
}

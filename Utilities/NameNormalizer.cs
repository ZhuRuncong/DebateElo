using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Linq;

namespace DebateElo.Utilities
{
    public static class NameNormalizer
    {
        private static readonly Regex HtmlTagRegex = new Regex("<.*?>", RegexOptions.Compiled);
        private static readonly Regex RemoveQuotedOrBracketed = new Regex(@"[""(\[].*?["")\]]", RegexOptions.Compiled);
        private static readonly Regex NonAlphaRegex = new Regex("[^A-Za-z]", RegexOptions.Compiled);
        private static readonly Dictionary<string, string> CustomMappings = new()
        {
            { "nicholasabernathy", "nicholasabernethy" }
            // Add more mappings here
        };

        public static string Normalize(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var s = HtmlTagRegex.Replace(name, "");
            s = WebUtility.HtmlDecode(s);
            s = RemoveQuotedOrBracketed.Replace(s, "");
            s = s.Replace("-", "");

            s = new string(s.Where(c =>
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                return cat != UnicodeCategory.OtherSymbol && cat != UnicodeCategory.Surrogate;
            }).ToArray());
            var decomposed = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            s = sb.ToString().Normalize(NormalizationForm.FormC);

            s = NonAlphaRegex.Replace(s, "");
            s = s.ToLowerInvariant();

            if (CustomMappings.TryGetValue(s, out var mapped))
                return mapped;

            return s;
        }
    }
}

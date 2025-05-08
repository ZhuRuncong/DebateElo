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
        private static readonly Regex NicknameRegex = new Regex(@"\s*[""'][^""']+[""']|\s*\([^)]*\)", RegexOptions.Compiled);
        private static readonly Regex MultiSpaceRegex = new Regex(@"\s{2,}", RegexOptions.Compiled);

        public static string Normalize(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var s = HtmlTagRegex.Replace(name, "");
            s = WebUtility.HtmlDecode(s).Trim();
            s = NicknameRegex.Replace(s, " ").Trim();
            s = new string(s.Where(c =>
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                return cat != UnicodeCategory.OtherSymbol && cat != UnicodeCategory.Surrogate;
            }).ToArray());

            var temp = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in temp)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            s = sb.ToString().Normalize(NormalizationForm.FormC);
            s = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
            s = MultiSpaceRegex.Replace(s, " ");

            return s;
        }
    }
}

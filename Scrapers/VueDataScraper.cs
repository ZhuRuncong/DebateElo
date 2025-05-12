using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using DebateElo.Utilities;

namespace DebateElo.Scrapers
{
    public class VueDataScraper
    {
        public JObject ExtractVueData(string url)
        {
            var html = HtmlFetcher.FetchSync(url);

            var match = Regex.Match(html, @"window\.vueData\s*=\s*({[\s\S]+?})\s*</script>");
            if (!match.Success)
                throw new Exception("vueData not found in HTML.");

            return JObject.Parse(match.Groups[1].Value);
        }

        public JArray GetVueTables(JObject vueData)
        {
            return vueData["tablesData"] as JArray
                   ?? throw new Exception("vueData does not contain a valid tablesData array.");
        }
    }
}
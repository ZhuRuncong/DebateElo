using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace DebateElo.Scrapers
{
    public class VueDataScraper
    {
        private static readonly HttpClient client = new();

        public JObject ExtractVueData(string url)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            var html = client.GetStringAsync(url).Result;

            var match = Regex.Match(html, @"window\.vueData\s*=\s*({[\s\S]+?})\s*</script>");
            if (!match.Success)
                throw new Exception("vueData not found in HTML.");

            return JObject.Parse(match.Groups[1].Value);
        }

        public JArray GetVueTables(JObject vueData)
        {
            var tables = vueData["tablesData"] as JArray;
            if (tables == null || !tables.HasValues)
                throw new Exception("vueData does not contain a valid tablesData array.");

            return tables;
        }
    }
}

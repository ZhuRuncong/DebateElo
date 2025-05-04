using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DebateElo.Models;

namespace DebateElo.Scrapers
{
    public class BPRoundScraper : IRoundScraper
    {
        private static readonly HttpClient client = new HttpClient();

        public List<RoundResult> ScrapeRound(string url)
        {
            var results = new List<RoundResult>();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

            var response = client.GetStringAsync(url).Result;
            var match = Regex.Match(response, @"window\.vueData\s*=\s*({[\s\S]+?})\s*</script>");
            if (!match.Success)
                throw new Exception("vueData not found in the HTML.");

            var vueDataRaw = match.Groups[1].Value;
            var vueData = JsonConvert.DeserializeObject<JObject>(vueDataRaw)
                ?? throw new Exception("Failed to parse vueData.");

            var tablesData = vueData["tablesData"] as JArray
                ?? throw new Exception("tablesData missing or invalid.");

            if (tablesData.Count == 0)
                throw new Exception("tablesData is empty.");

            var table = tablesData[0];
            var head = table["head"] as JArray
                ?? throw new Exception("head is missing or invalid.");
            var data = table["data"] as JArray
                ?? throw new Exception("data is missing or invalid.");

            var colMap = new Dictionary<string, int>();
            for (int i = 0; i < head.Count; i++)
            {
                string title = head[i]?["title"]?.ToString() ?? head[i]?["key"]?.ToString() ?? $"col_{i}";
                colMap[title] = i;
            }

            foreach (var row in data)
            {
                var r = (JArray)row;

                string adjHtml = r[colMap["Adjudicators"]]?["text"]?.ToString() ?? "";
                var adjDoc = new HtmlDocument();
                adjDoc.LoadHtml(adjHtml);
                var adjSpans = adjDoc.DocumentNode.SelectNodes("//span[@class='d-inline']");
                List<string> adjNames = new();
                if (adjSpans != null)
                {
                    foreach (var span in adjSpans)
                    {
                        var name = HtmlEntity.DeEntitize(span.InnerText.Trim());
                        if (!string.IsNullOrEmpty(name))
                            adjNames.Add(name);
                    }
                }

                results.Add(new RoundResult
                {
                    Adjudicator = string.Join(", ", adjNames),

                    OG = new TeamStanding(
                        r[colMap["OG"]]?["text"]?.ToString() ?? "",
                        r[colMap["OG"]]?["sort"]?.ToObject<int?>() ?? -1
                    ),
                    OO = new TeamStanding(
                        r[colMap["OO"]]?["text"]?.ToString() ?? "",
                        r[colMap["OO"]]?["sort"]?.ToObject<int?>() ?? -1
                    ),
                    CG = new TeamStanding(
                        r[colMap["CG"]]?["text"]?.ToString() ?? "",
                        r[colMap["CG"]]?["sort"]?.ToObject<int?>() ?? -1
                    ),
                    CO = new TeamStanding(
                        r[colMap["CO"]]?["text"]?.ToString() ?? "",
                        r[colMap["CO"]]?["sort"]?.ToObject<int?>() ?? -1
                    )
                });

            }

            return results;
        }
    }
}
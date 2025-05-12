using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using DebateElo.Models;
using DebateElo.Utilities;

namespace DebateElo.Scrapers
{
    public class BPRoundScraper : IRoundScraper
    {
        private readonly VueDataScraper vueScraper = new();

        public List<RoundResult> ScrapeRound(string url, int roundNumber)
        {
            var fullUrl = url.TrimEnd('/') + "/results/round/" + roundNumber + "/?view=debate";
            var results = new List<RoundResult>();

            var rawHtml = HtmlFetcher.FetchSync(fullUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(rawHtml);

            string roundTitle = "Unknown Round";
            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='pageTitle']//small[contains(@class, 'text-muted')]");
            if (titleNode != null)

                roundTitle = titleNode.InnerText.Trim().Replace("for ", "");

            JObject vueData = vueScraper.ExtractVueData(fullUrl);
            JArray tables = vueScraper.GetVueTables(vueData);

            var table = tables[0];
            var head = table["head"] as JArray;
            var data = table["data"] as JArray;

            if (head == null) throw new Exception("Round table is missing headers.");
            if (data == null) throw new Exception("Round table is missing data.");

            var colMap = new Dictionary<string, int>();
            for (int i = 0; i < head.Count; i++)
            {
                string key = head[i]?["key"]?.ToString() ?? $"col_{i}";
                colMap[key] = i;
            }

            foreach (var row in data)
            {
                if (row is not JArray r) continue;

                var adjHtml = r[colMap["adjudicators"]]?["text"]?.ToString() ?? "";
                var adjNames = ExtractAdjudicatorNames(adjHtml);

                results.Add(new RoundResult
                {
                    RoundTitle = roundTitle,
                    Adjudicator = string.Join(", ", adjNames),
                    OG = ParseStanding(r, colMap, "OG"),
                    OO = ParseStanding(r, colMap, "OO"),
                    CG = ParseStanding(r, colMap, "CG"),
                    CO = ParseStanding(r, colMap, "CO")
                });
            }

            return results;
        }

        private static List<string> ExtractAdjudicatorNames(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var spans = doc.DocumentNode.SelectNodes("//span[@class='d-inline']");
            var names = new List<string>();
            if (spans != null)
            {
                foreach (var span in spans)
                {
                    var name = HtmlEntity.DeEntitize(span.InnerText.Trim());
                    if (!string.IsNullOrEmpty(name))
                        names.Add(name);
                }
            }
            return names;
        }

        private static TeamStanding ParseStanding(JArray row, Dictionary<string, int> colMap, string key)
        {
            var col = row[colMap[key]];
            return new TeamStanding(
                col?["text"]?.ToString() ?? "",
                col?["sort"]?.ToObject<int?>() ?? -1
            );
        }
    }
}

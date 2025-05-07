using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using DebateElo.Models;

namespace DebateElo.Scrapers
{
    public class BPRoundScraper : IRoundScraper
     {
        private readonly VueDataScraper vueScraper = new();

        public List<RoundResult> ScrapeRound(string url, int roundNumber)
        {
            var fullUrl = url.TrimEnd('/') + "/results/round/" + roundNumber + "/?view=debate";
            var results = new List<RoundResult>();
            JObject vueData = vueScraper.ExtractVueData(fullUrl);
            JArray tables  = vueScraper.GetVueTables(vueData);

            var table = tables[0];
            var head  = table["head"] as JArray;
            var data  = table["data"] as JArray;

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
                if (!(row is JArray r)) continue;

                string adjHtml = r[colMap["Adjudicators"]]?["text"]?.ToString() ?? "";
                var adjDoc = new HtmlDocument();
                adjDoc.LoadHtml(adjHtml);
                var adjSpans = adjDoc.DocumentNode.SelectNodes("//span[@class='d-inline']");
                var adjNames = new List<string>();
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

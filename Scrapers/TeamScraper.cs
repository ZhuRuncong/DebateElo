using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using DebateElo.Models;

namespace DebateElo.Scrapers
{
    public class TeamScraper
    {
        private readonly VueDataScraper vueScraper = new();

        public List<Team> ScrapeTeams(string baseUrl, string tournamentName)
        {
            var listUrl     = baseUrl.TrimEnd('/') + "/participants/list/";
            var fallbackUrl = baseUrl.TrimEnd('/') + "/tab/team/";
            JObject vueData;
            try
            {
                vueData = vueScraper.ExtractVueData(listUrl);
            }
            catch
            {
                vueData = vueScraper.ExtractVueData(fallbackUrl);
            }

            var tables    = vueData["tablesData"] as JArray ?? throw new Exception("tablesData missing");
            var teamTable = tables.Count > 1 ? tables[1] : tables[0];
            var headers   = teamTable["head"] as JArray ?? throw new Exception("Team table missing headers");
            var rows      = teamTable["data"] as JArray ?? throw new Exception("Team table missing data");

            int teamCol = -1;
            for (int i = 0; i < headers.Count; i++)
                if ((string?)headers[i]?["key"] == "team")
                {
                    teamCol = i;
                    break;
                }
            if (teamCol < 0) throw new Exception("Could not find 'team' column");

            var teams = new List<Team>();
            var seen  = new HashSet<string>();

            foreach (var row in rows)
            {
                if (!(row is JArray r) || teamCol >= r.Count) continue;

                var cell    = r[teamCol];
                var popover = cell?["popover"];
                var name = (string?)cell?["text"]?.ToString().Trim()
                        ?? (string?)popover?["title"]?.ToString().Trim()
                        ?? "Unknown";

                if (!seen.Add(name)) continue;

                var speakers = new List<string>();
                var content  = popover?["content"] as JArray;
                if (content != null)
                    foreach (var entry in content)
                    {
                        var text = (string?)entry?["text"];
                        if (!string.IsNullOrEmpty(text) && text.Contains(','))
                        {
                            speakers.AddRange(text.Split(',', StringSplitOptions.TrimEntries));
                            break;
                        }
                    }

                teams.Add(new Team(tournamentName, name, speakers));
            }

            return teams;
        }
    }
}

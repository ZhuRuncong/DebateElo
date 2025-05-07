using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using DebateElo.Models;

namespace DebateElo.Scrapers
{
    public class TeamScraper
    {
        private readonly VueDataScraper vueScraper = new();

        public List<Team> ScrapeTeams(string url, string tournamentName)
        {
            var fullUrl = url.TrimEnd('/') + "/participants/list/";

            var teams = new List<Team>();
            var seen = new HashSet<string>();

            JObject vueData = vueScraper.ExtractVueData(fullUrl);
            JArray tables = vueScraper.GetVueTables(vueData);

            var teamTable = tables[1];
            var headers = teamTable["head"] as JArray;
            var rows = teamTable["data"] as JArray;

            if (headers == null)
                throw new Exception("Team table is missing headers.");

            if (rows == null)
                throw new Exception("Team table is missing data.");

            int teamCol = -1;
            for (int i = 0; i < headers.Count; i++)
            {
                if ((string?)headers[i]?["key"] == "team")
                {
                    teamCol = i;
                    break;
                }
            }

            if (teamCol < 0)
                throw new Exception("Could not find 'team' column in headers.");

            foreach (var row in rows)
            {
                var r = row as JArray;
                if (r == null || teamCol >= r.Count)
                    continue;

                var cell = r[teamCol];
                var popover = cell?["popover"];
                var teamName = (string?)popover?["title"]?.ToString().Trim() 
                               ?? (string?)cell?["text"]?.ToString().Trim()
                               ?? "Unknown";

                if (!seen.Add(teamName))
                    continue;

                var speakers = new List<string>();
                var content = popover?["content"] as JArray;
                if (content != null)
                {
                    foreach (var entry in content)
                    {
                        var text = (string?)entry?["text"];
                        if (!string.IsNullOrEmpty(text) && text.Contains(','))
                        {
                            speakers.AddRange(text.Split(',', StringSplitOptions.TrimEntries));
                            break;
                        }
                    }
                }

                teams.Add(new Team(tournamentName, teamName, speakers));
            }

            return teams;
        }
    }
}

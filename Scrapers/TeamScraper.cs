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
            var teams = new List<Team>();
            var seen = new HashSet<string>();

            JObject vueData = vueScraper.ExtractVueData(url);
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

            foreach (var row in rows)
            {
                var r = (JArray)row;
                var cell = r[teamCol];

                var popover = cell["popover"];
                var teamName = (string?)popover?["title"] ?? (string?)cell["text"] ?? "Unknown";

                if (!seen.Add(teamName))
                    continue;

                var speakers = new List<string>();
                var content = popover?["content"] as JArray;
                if (content != null)
                {
                    foreach (var entry in content)
                    {
                        var text = (string?)entry?["text"];
                        if (text != null && text.Contains(','))
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

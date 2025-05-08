using System;
using System.Collections.Generic;
using DebateElo.Models;
using DebateElo.Scrapers;
using DebateElo.Utilities;

namespace DebateElo.Services
{
    public class TeamBatchScraper
    {
        private readonly TeamScraper _scraper;

        public TeamBatchScraper(TeamScraper scraper)
        {
            _scraper = scraper;
        }

        public void ScrapeAllTeamsToCsv(string inputCsvPath, string outputCsvPath)
        {
            var tournaments = CsvUtilities.LoadTournaments(inputCsvPath);
            var allTeams = new List<Team>();

            foreach (var t in tournaments)
            {
                Console.WriteLine($"Scraping teams: {t.Name}");
                try
                {
                    var teams = _scraper.ScrapeTeams(t.Url, t.Name);
                    allTeams.AddRange(teams);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Skipping {t.Name} due to HTTP error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error scraping {t.Name}: {ex.Message}");
                }
            }

            CsvUtilities.SaveCsv(
                allTeams,
                outputCsvPath,
                "Tournament,TeamName,Speakers",
                team =>
                {
                    var speakers = string.Join(";", team.Speakers).Replace("\"","\"\"");
                    return $"\"{team.Tournament.Replace("\"","\"\"")}\",\"{team.TeamName.Replace("\"","\"\"")}\",\"{speakers}\"";
                }
            );
        }
    }
}

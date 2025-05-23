using System;
using System.Collections.Generic;
using DebateElo.Models;
using DebateElo.Scrapers;
using DebateElo.Utilities;

namespace DebateElo.Services
{
    public class MotionBatchScraper
    {
        private readonly IMotionScraper _scraper;

        public MotionBatchScraper(IMotionScraper scraper)
            => _scraper = scraper;

        public void ScrapeAllMotionsToCsv(string inCsv, string outCsv)
        {
            var tournaments = CsvUtilities.LoadTournaments(inCsv);
            var allMotions  = new List<Motion>();

            foreach (var t in tournaments)
            {
                Console.WriteLine($"Scraping {t.Name}");
                t.ScrapeMotions(_scraper);
                allMotions.AddRange(t.Motions);
            }

            CsvUtilities.SaveCsv(
                allMotions,
                outCsv,
                "Tournament,Round,Title,Lead",
                m => $"\"{m.TournamentId}\",\"{m.RoundNumber}\","
                   + $"\"{m.Title.Replace("\"","\"\"")}\","
                   + $"\"{m.Lead.Replace("\"","\"\"")}\""
            );
        }
    }
}

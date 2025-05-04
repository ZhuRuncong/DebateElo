using DebateElo.Models;
using DebateElo.Scrapers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DebateElo.Services
{
    public class MotionBatchScraper
    {
        private readonly ITournamentScraper _scraper;

        public MotionBatchScraper(ITournamentScraper scraper)
        {
            _scraper = scraper;
        }

        public void ScrapeAllMotionsToCsv(string inputCsvPath, string outputCsvPath)
        {
            var tournaments = LoadTournaments(inputCsvPath);
            var allMotions = new List<Motion>();

            foreach (var tournament in tournaments)
            {
                Console.WriteLine($"Scraping: {tournament.Name}");
                tournament.ScrapeMotions(_scraper);
                allMotions.AddRange(tournament.Motions);
            }

            SaveMotionsToCsv(allMotions, outputCsvPath);
        }

        private List<Tournament> LoadTournaments(string csvPath)
        {
            var tournaments = new List<Tournament>();
            using (var reader = new StreamReader(csvPath))
            {
                _ = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var fields = line.Split(',');

                    if (fields.Length >= 3 &&
                        DateTime.TryParse(fields[2], out DateTime date))
                    {
                        tournaments.Add(new Tournament(fields[0], fields[1], date));
                    }
                }
            }
            return tournaments;
        }


        private void SaveMotionsToCsv(List<Motion> motions, string csvPath)
        {
            using (var writer = new StreamWriter(csvPath))
            {
                writer.WriteLine("Tournament,Round,Title,Lead");

                foreach (var motion in motions)
                {
                    writer.WriteLine($"\"{motion.TournamentId}\",\"{motion.RoundNumber}\",\"{motion.Title.Replace("\"", "\"\"")}\",\"{motion.Lead.Replace("\"", "\"\"")}\"");
                }
            }
        }
    }
}

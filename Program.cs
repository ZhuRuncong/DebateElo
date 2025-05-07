using System;
using DebateElo.Scrapers;
using DebateElo.Services;

namespace DebateElo
{
    class Program
    {
        static void Main(string[] args)
        {
            var roundScraper = new BPRoundScraper();
            var batchScraper = new RoundBatchScraper(roundScraper);

            string tournamentUrl = "https://westernwinter2024.calicotab.com/westernwinter2024/";

            var tournamentRounds = batchScraper.ScrapeTournamentRounds(tournamentUrl);

            Console.WriteLine($"Total rounds scraped: {tournamentRounds.Count}");
            int roundIndex = 1;
            foreach (var round in tournamentRounds)
            {
                Console.WriteLine($"Round {roundIndex}: {round.Count} results");
                roundIndex++;
            }
        }
    }
}

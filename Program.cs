using System;
using DebateElo.Scrapers;
using DebateElo.Models;
using System.Collections.Generic;

namespace DebateElo
{
    class Program
    {
        static void Main(string[] args)
        {
            var scraper = new RoundScraper();

            string url = "https://hhhs2025.calicotab.com/hhhs2025/results/round/1/?view=debate";

            try
            {
                List<RoundResult> results = scraper.ScrapeRound(url);

                foreach (var result in results)
                {
                    Console.WriteLine($"Adjudicators: {result.Adjudicator}");

                    Console.WriteLine($"OG: {result.OG.Name} (Rank {result.OG.Rank})");
                    Console.WriteLine($"OO: {result.OO.Name} (Rank {result.OO.Rank})");
                    Console.WriteLine($"CG: {result.CG.Name} (Rank {result.CG.Rank})");
                    Console.WriteLine($"CO: {result.CO.Name} (Rank {result.CO.Rank})");

                    Console.WriteLine(new string('-', 50));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error scraping round: " + ex.Message);
            }
        }
    }
}

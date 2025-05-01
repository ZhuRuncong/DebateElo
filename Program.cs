using System;
using DebateElo.Scrapers;
using DebateElo.Services;


namespace DebateElo
{
    class Program
    {
        static void Main(string[] args)
        {
            var scraper = new TournamentScraper();
            var batchScraper = new MotionBatchScraper(scraper);

            string inputCsv = "./Data/tournament_data.csv";
            string outputCsv = "./Data/motion_output.csv";

            batchScraper.ScrapeAllMotionsToCsv(inputCsv, outputCsv);

            Console.WriteLine("Motion scraping complete. Output saved to: " + outputCsv);
        }
    }
}

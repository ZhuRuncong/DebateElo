using System;
using System.Text;
using DebateElo.Services;
using DebateElo.Scrapers;

namespace DebateElo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string inputCsv  = "Data/tournament_data.csv";
            string outputCsv = "Data/team_output.csv";
            var batchScraper = new TeamBatchScraper(new TeamScraper());
            try
            {
                batchScraper.ScrapeAllTeamsToCsv(inputCsv, outputCsv);
                Console.WriteLine($"Successfully scraped all teams to '{outputCsv}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during scraping: {ex.Message}");
            }
        }
    }
}

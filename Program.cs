using System;
using DebateElo.Scrapers;
using DebateElo.Services;

class Program
{
    static void Main()
    {
        var scraper = new MotionScraper();
        var batch = new MotionBatchScraper(scraper);

        string inputCsv = "Data/tournament_data.csv";
        string outputCsv = "Data/motion_output.csv";

        try
        {
            batch.ScrapeAllMotionsToCsv(inputCsv, outputCsv);
            Console.WriteLine($"Done. Motions saved to {outputCsv}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Scraping failed:");
            Console.WriteLine(ex.Message);
        }
    }
}

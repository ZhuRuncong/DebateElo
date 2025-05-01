using System;
using DebateElo.Models;
using DebateElo.Scrapers;

namespace DebateElo
{
    class Program
    {
        static void Main(string[] args)
        {
            ITournamentScraper scraper = new TournamentScraper();

            var tournament = new Tournament(
                name: "MyTournament",
                url: "https://abp2020.calicotab.com/abp2020/",
                date: DateTime.Now
            );

            tournament.ScrapeMotions(scraper);

            foreach (var motion in tournament.Motions)
            {
                Console.WriteLine($"{motion.Lead}");
            }

            Console.WriteLine("Scraping complete.");
        }
    }
}

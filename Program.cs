using System;

namespace DebateElo
{
    class Program
    {
        static void Main(string[] args)
        {
            bool runScraper = true;

            if (runScraper)
            {
                var scraper = new TournamentScraper();
                var motions = scraper.FetchMotions("https://wudc2020.calicotab.com/wudc2020/motions/");

                foreach (var motion in motions)
                {
                    Console.WriteLine(motion);
                    Console.WriteLine();
                }
            }
        }
    }
}

using System;
using DebateElo.Scrapers;
using DebateElo.Services;

namespace DebateElo
{
    class Program
    {
        static void Main()
        {
            var fullScraper = new FullRoundBatchScraper(
                new TournamentRoundBatchScraper(new BPRoundScraper()),
                new TeamScraper()
            );

            var rankingInstances = fullScraper.ScrapeAll(
                "./Data/tournament_data.csv",
                "./Data/motion_clustered.csv"
            );

            Console.WriteLine($"Scraped {rankingInstances.Count} Ranking Instances.");

            foreach (var instance in rankingInstances)
            {
                Console.WriteLine($"Date: {instance.Date:yyyy-MM-dd}, Cluster: {instance.MotionCluster}");
                PrintRank("Rank 1", instance.Rank1Teams);
                PrintRank("Rank 2", instance.Rank2Teams);
                PrintRank("Rank 3", instance.Rank3Teams);
                PrintRank("Rank 4", instance.Rank4Teams);
                Console.WriteLine("--------------------------------");
            }
        }

        static void PrintRank(string rankName, List<List<string>> teams)
        {
            foreach (var team in teams)
                Console.WriteLine($"  {rankName}: {string.Join(", ", team)}");
        }
    }
}

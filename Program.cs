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

            fullScraper.ScrapeAll(
                "./Data/tournament_data.csv",
                "./Data/motion_clustered.csv",
                "./Data/teams_output.csv",
                "./Data/ranking_output.csv"
            );
        }
    }
}

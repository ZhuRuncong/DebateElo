using System.Collections.Generic;

namespace DebateElo.Scrapers
{
    public interface ITournamentScraper
    {
        List<(string title, string lead)> FetchMotions(string url);
    }
}

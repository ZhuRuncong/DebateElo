using System.Collections.Generic;

namespace DebateElo
{
    public interface ITournamentScraper
    {
        List<string> FetchMotions(string url);
    }
}
using System.Collections.Generic;
using DebateElo.Models;

namespace DebateElo.Scrapers
{
    public interface IRoundScraper
    {
        List<RoundResult> ScrapeRound(string baseUrl, int roundNumber);
    }
}

using System.Collections.Generic;

namespace DebateElo.Scrapers
{
    public interface IMotionScraper
    {
        List<(string title, string lead)> FetchMotions(string url);
    }
}

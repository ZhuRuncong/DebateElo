using System.Collections.Generic;

namespace BPTrueskill
{
    public interface ITournamentScraper
    {
        List<string> FetchMotions(string url);
    }
}
using System;
using System.Collections.Generic;

namespace DebateElo
{
    public class Tournament
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime Date { get; set; }

        public List<Motion> Motions { get; set; }

        public Tournament(string name, string url, DateTime date)
        {
            Name = name;
            Url = url;
            Date = date;
            Motions = new List<Motion>();
        }

        public void ScrapeMotions(ITournamentScraper scraper)
        {
            var motionTexts = scraper.FetchMotions(Url);
            for (int i = 0; i < motionTexts.Count; i++)
            {
                var motion = new Motion(motionTexts[i], i + 1, Name);

                Motions.Add(motion);
            }
        }
    }
}
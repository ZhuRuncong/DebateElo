using System;
using System.Collections.Generic;
using DebateElo.Models;
using DebateElo.Scrapers;

namespace DebateElo.Services
{
    public class RoundBatchScraper
    {
        private readonly IRoundScraper _roundScraper;

        public RoundBatchScraper(IRoundScraper roundScraper)
        {
            _roundScraper = roundScraper;
        }

        public List<List<RoundResult>> ScrapeTournamentRounds(string baseUrl)
        {
            var allRounds = new List<List<RoundResult>>();
            int roundNumber = 1;
            int consecutiveFailures = 0;

            while (consecutiveFailures < 2)
            {
                try
                {
                    var roundResults = _roundScraper.ScrapeRound(baseUrl, roundNumber);
                    if (roundResults.Count == 0)
                    {
                        consecutiveFailures++;
                    }
                    else
                    {
                        allRounds.Add(roundResults);
                        consecutiveFailures = 0;
                    }
                }
                catch (Exception ex)
                {
                    consecutiveFailures++;
                    Console.WriteLine($"Round {roundNumber} failed with {ex.GetType().Name}: {ex.Message}");
                }

                roundNumber++;
            }

            return allRounds;
        }
    }
}

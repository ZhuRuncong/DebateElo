using System;
using System.Collections.Generic;
using DebateElo.Models;
using DebateElo.Scrapers;

namespace DebateElo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var teamScraper = new TeamScraper();

            string url = "https://westernwsdc.calicotab.com/westernwsdc2024/participants/list/";
            string tournamentName = "Western WSDC 2024";

            try
            {
                List<Team> teams = teamScraper.ScrapeTeams(url, tournamentName);

                foreach (var team in teams)
                {
                    Console.WriteLine($"{team.TeamName} ({team.Tournament})");
                    Console.WriteLine("Speakers: " + string.Join(", ", team.Speakers));
                    Console.WriteLine(new string('-', 40));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error scraping teams: " + ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DebateElo.Models;
using DebateElo.Scrapers;
using DebateElo.Utilities;

namespace DebateElo.Services
{
    public class FullRoundBatchScraper
    {
        private readonly TournamentRoundBatchScraper _roundScraper;
        private readonly TeamScraper _teamScraper;

        private Dictionary<(string Tournament, string Round), int> _motionClusters = new();
        private Dictionary<string, (DateTime Date, string Name)> _tournamentMeta = new();

        public FullRoundBatchScraper(TournamentRoundBatchScraper roundScraper, TeamScraper teamScraper)
        {
            _roundScraper = roundScraper;
            _teamScraper = teamScraper;
        }

        public List<RankingInstance> ScrapeAll(string tournamentDataCsv, string motionClusterCsv)
        {
            LoadMotionClusters(motionClusterCsv);
            LoadTournamentMetadata(tournamentDataCsv);

            var allRankingInstances = new List<RankingInstance>();

            foreach (var url in _tournamentMeta.Keys)
            {
                var (date, tournamentName) = _tournamentMeta[url];
                Console.WriteLine($"Scraping tournament: {tournamentName} ({date:yyyy-MM-dd})");

                Dictionary<string, List<string>> teams;
                List<List<RoundResult>> rounds;

                try
                {
                    teams = _teamScraper.ScrapeTeams(url, tournamentName)
                        .ToDictionary(
                            team => team.TeamName,
                            team => team.Speakers.Select(NameNormalizer.Normalize).ToList()
                        );
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to scrape teams for {tournamentName}: {e.Message}");
                    continue;
                }

                try
                {
                    rounds = _roundScraper.ScrapeTournamentRounds(url);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to scrape rounds for {tournamentName}: {e.Message}");
                    continue;
                }

                foreach (var roundResults in rounds)
                {
                    foreach (var result in roundResults)
                    {
                        var instance = new RankingInstance
                        {
                            Date = date,
                            MotionCluster = GetMotionCluster(tournamentName, result.RoundTitle),
                            Rank1Teams = new(),
                            Rank2Teams = new(),
                            Rank3Teams = new(),
                            Rank4Teams = new()
                        };

                        AssignSpeakers(result.OG, teams);
                        AssignSpeakers(result.OO, teams);
                        AssignSpeakers(result.CG, teams);
                        AssignSpeakers(result.CO, teams);

                        AddTeamByRank(instance, result.OG);
                        AddTeamByRank(instance, result.OO);
                        AddTeamByRank(instance, result.CG);
                        AddTeamByRank(instance, result.CO);

                        allRankingInstances.Add(instance);
                    }
                }
            }

            SaveRankingInstances(allRankingInstances, "ranking_output.csv");
            return allRankingInstances;
        }


        private void AssignSpeakers(TeamStanding standing, Dictionary<string, List<string>> teamMap)
        {
            if (standing.TeamName != null && teamMap.TryGetValue(standing.TeamName, out var speakers))
            {
                standing.Speaker1 = speakers.ElementAtOrDefault(0) ?? "";
                standing.Speaker2 = speakers.ElementAtOrDefault(1) ?? "";

                Console.WriteLine($"Matched '{standing.TeamName}' to speakers '{standing.Speaker1}', '{standing.Speaker2}'");
            }
            else
            {
                Console.WriteLine($"Team '{standing.TeamName}' not found in teamMap");
            }
        }


        private void AddTeamByRank(RankingInstance instance, TeamStanding standing)
        {
            var speakers = new List<string>
            {
                standing.Speaker1 ?? "",
                standing.Speaker2 ?? ""
            };
            switch (standing.Rank)
            {
                case 1:
                    instance.Rank1Teams.Add(speakers);
                    break;
                case 2:
                    instance.Rank2Teams.Add(speakers);
                    break;
                case 3:
                    instance.Rank3Teams.Add(speakers);
                    break;
                case 4:
                    instance.Rank4Teams.Add(speakers);
                    break;
            }
        }

        private void LoadMotionClusters(string csvPath)
        {
            _motionClusters.Clear();

            foreach (var line in File.ReadLines(csvPath).Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length >= 3 && int.TryParse(parts[2], out int cluster))
                {
                    var key = (parts[0].Trim(), parts[1].Trim());
                    _motionClusters[key] = cluster;

                    Console.WriteLine($"Loaded cluster key: '{key}' => {cluster}");
                }
                else
                {
                    Console.WriteLine($"Failed to parse line: {line}");
                }
            }
        }

        private void LoadTournamentMetadata(string csvPath)
        {
            foreach (var line in File.ReadLines(csvPath).Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length >= 3 && DateTime.TryParse(parts[2], out var date))
                {
                    var url = parts[1].Trim();
                    var name = parts[0].Trim();
                    _tournamentMeta[url] = (date, name);
                }
            }
        }

        private int GetMotionCluster(string tournament, string round)
        {
            var key = (tournament, round);
            if (_motionClusters.TryGetValue(key, out int cluster))
                return cluster;

            Console.WriteLine($"Cluster lookup failed for Tournament: '{tournament}', Round: '{round}'");
            return -1;
        }

        public void SaveRankingInstances(List<RankingInstance> instances, string outputPath)
        {
            using var writer = new StreamWriter(outputPath);
            writer.WriteLine("Date,Cluster,Rank1Teams,Rank2Teams,Rank3Teams,Rank4Teams");

            foreach (var instance in instances)
            {
                string FormatTeams(List<List<string>> teams)
                {
                    return string.Join(";", teams.Select(team => string.Join(" & ", team)));
                }

                var row = string.Join(",",
                    instance.Date.ToString("yyyy-MM-dd"),
                    instance.MotionCluster.ToString(),
                    $"\"{FormatTeams(instance.Rank1Teams)}\"",
                    $"\"{FormatTeams(instance.Rank2Teams)}\"",
                    $"\"{FormatTeams(instance.Rank3Teams)}\"",
                    $"\"{FormatTeams(instance.Rank4Teams)}\""
                );

                writer.WriteLine(row);
            }
        }

    }


}

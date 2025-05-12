using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DebateElo.Models;

namespace DebateElo.Utilities
{
    public static class CsvUtilities
    {
        public static List<Tournament> LoadTournaments(string csvPath)
        {
            var list = new List<Tournament>();
            using var reader = new StreamReader(csvPath);
            _ = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var fields = line.Split(',');
                if (fields.Length < 2) continue;

                var name = fields[0];
                var url  = fields[1];
                list.Add(new Tournament(name, url, DateTime.MinValue));
            }
            return list;
        }

        public static void SaveCsv<T>(
            List<T> items,
            string csvPath,
            string header,
            Func<T,string> rowFormatter)
        {
            using var writer = new StreamWriter(csvPath);
            writer.WriteLine(header);
            foreach (var item in items)
                writer.WriteLine(rowFormatter(item));
        }

        public static Dictionary<(string Tournament, string TeamName), List<string>> LoadTeamSpeakerMap(string csvPath)
        {
            var map = new Dictionary<(string, string), List<string>>();

            foreach (var line in File.ReadLines(csvPath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = SplitCsvLine(line);
                if (parts.Length < 3) continue;

                var tournament = parts[0].Trim('"');
                var teamName = parts[1].Trim('"');
                var speakerPart = parts[2].Trim('"');

                var speakers = speakerPart
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(NameNormalizer.Normalize)
                    .ToList();

                map[(tournament, teamName)] = speakers;
            }

            return map;
        }

        private static string[] SplitCsvLine(string line)
        {
            return Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        }
    }
}

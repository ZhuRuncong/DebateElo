using System;
using System.Collections.Generic;
using System.IO;
using DebateElo.Models;

namespace DebateElo.Services
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
    }
}

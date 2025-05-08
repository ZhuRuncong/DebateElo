using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DebateElo.Utilities
{
    public static class SpeakerExtractor
    {
        private static readonly Regex CsvLine = new Regex("^\"(?<t>.*?)\",\"(?<team>.*?)\",\"(?<speakers>.*?)\"$");

        public static void ExtractUniqueSpeakersToCsv(string inputCsvPath, string outputCsvPath)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using var reader = new StreamReader(inputCsvPath);
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var m = CsvLine.Match(line);
                if (!m.Success) continue;
                var raw = m.Groups["speakers"].Value;
                foreach (var s in raw.Split(';'))
                {
                    var name = Regex.Replace(s, "<.*?>", "").Trim();
                    if (!string.IsNullOrEmpty(name))
                        seen.Add(name);
                }
            }
            using var writer = new StreamWriter(outputCsvPath);
            writer.WriteLine("Speaker");
            foreach (var name in seen)
                writer.WriteLine($"\"{name.Replace("\"","\"\"")}\"");
        }
    }
}

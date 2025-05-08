using System;
using DebateElo.Utilities;

class Program
{
    static void Main()
    {
        SpeakerExtractor.ExtractUniqueSpeakersToCsv(
            "data/team_output.csv",
            "data/unique_speakers.csv"
        );
    }
}

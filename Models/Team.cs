using System.Collections.Generic;

namespace DebateElo.Models
{
    public class Team
    {
        public string Tournament { get; set; }
        public string TeamName { get; set; }
        public List<string> Speakers { get; set; }

        public Team(string tournament, string teamName, List<string> speakers)
        {
            Tournament = tournament;
            TeamName = teamName;
            Speakers = speakers ?? new List<string>();
        }

        public override string ToString()
        {
            string speakerList = string.Join(", ", Speakers);
            return $"{TeamName} ({Tournament}) — Speakers: {speakerList}";
        }
    }
}

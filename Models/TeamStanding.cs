namespace DebateElo.Models
{
    public class TeamStanding
    {
        public string? TeamName { get; set; }

        public string? Speaker1 { get; set; }
        public string? Speaker2 { get; set; }

        public int Rank { get; set; } = -1;

        public string Name =>
            !string.IsNullOrWhiteSpace(TeamName)
                ? TeamName
                : $"{Speaker1 ?? "?"} & {Speaker2 ?? "?"}";

        public TeamStanding() { }

        public TeamStanding(string teamName, int rank = -1)
        {
            TeamName = teamName;
            Rank = rank;
        }

        public TeamStanding((string, string) speakers, int rank = -1)
        {
            Speaker1 = speakers.Item1;
            Speaker2 = speakers.Item2;
            Rank = rank;
        }

    }
}

namespace DebateElo.Models
{
    public class RoundResult
    {
        public string RoundTitle { get; set; } = string.Empty;

        public string Adjudicator { get; set; } = string.Empty;

        public TeamStanding OG { get; set; } = new();
        public TeamStanding OO { get; set; } = new();
        public TeamStanding CG { get; set; } = new();
        public TeamStanding CO { get; set; } = new();
    }
}

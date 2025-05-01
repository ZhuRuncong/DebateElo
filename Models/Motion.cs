namespace DebateElo.Models
{
    public class Motion
    {
        public string Title { get; set; }
        public string Lead { get; set; }
        public int RoundNumber { get; set; }
        public string TournamentId { get; set; }
        public int? ClusterLabel { get; set; }

        public Motion(string title, string lead, int roundNumber, string tournamentId)
        {
            Title = title;
            Lead = lead;
            RoundNumber = roundNumber;
            TournamentId = tournamentId;
            ClusterLabel = null;
        }

        public string Text => $"{Title}: {Lead}";
    }
}

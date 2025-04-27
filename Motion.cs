using System;
using System.Collections.Generic;

namespace BPTrueskill
{
    public class Motion
    {
        public string Text { get; set; }
        public int RoundNumber { get; set; }
        public string TournamentId { get; set; }
        public int? ClusterLabel { get; set; }

        public Motion(string text, int roundNumber, string tournamentId)
        {
            Text = text;
            RoundNumber = roundNumber;
            TournamentId = tournamentId;
            ClusterLabel = null;
        }
    }
}
using System;
using System.Collections.Generic;

namespace DebateElo.Models
{
    public class RankingInstance
    {
        public DateTime Date { get; set; }
        public int ClusterLabel { get; set; }
        public List<List<string>> Rank1Teams { get; set; } = new();
        public List<List<string>> Rank2Teams { get; set; } = new();
        public List<List<string>> Rank3Teams { get; set; } = new();
        public List<List<string>> Rank4Teams { get; set; } = new();
    }
}

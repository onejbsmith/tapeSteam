using System;
using System.Collections.Generic;

#nullable disable

namespace tapeStream.Client.Models
{
    public partial class Trade
    {
        public long? Trades { get; set; }
        public string Symbol { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int? Dur { get; set; }
        public string Type { get; set; }
        public decimal? Profit { get; set; }
        public decimal? Total { get; set; }
        public double? Mark { get; set; }
        public double? PrevMark { get; set; }
        public decimal? Diff { get; set; }
        public decimal? ProfPerHr { get; set; }
        public decimal? AvgProfit { get; set; }
        public int? AvgDur { get; set; }
        public decimal? BatAvg { get; set; }
        public decimal? AvgLoss { get; set; }
        public decimal? AvgWin { get; set; }
        public decimal? TotalLoss { get; set; }
        public int? TotalLosses { get; set; }
        public decimal? TotalWin { get; set; }
        public int? TotalWins { get; set; }
        public int? TotalDur { get; set; }
        public double? AskPrice { get; set; }
        public double? BidPrice { get; set; }
        public int Dupe { get; set; }
        public int? CrossOverAmount { get; set; }
        public int RedCrossOver { get; set; }
        public int GreenCrossOver { get; set; }
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public short Seconds { get; set; }
        public double? MarkPrice { get; set; }
        public int? Red { get; set; }
        public int? Green { get; set; }
        public int Prevred { get; set; }
        public int Prevgreen { get; set; }
        public long? Row { get; set; }
    }
}

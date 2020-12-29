using System;
using System.Collections.Generic;

#nullable disable

namespace tapeStream.Client.Models
{
    public partial class FrameWhole
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime? DateTime { get; set; }
        public short? Seconds { get; set; }
        public double? MarkPrice { get; set; }
        public int? BuysTradeSizes { get; set; }
        public int? SellsTradeSizes { get; set; }
        public int? BidsBookSizes { get; set; }
        public int? AsksBookSizes { get; set; }
        public byte? BuysPriceCount { get; set; }
        public byte? SellsPriceCount { get; set; }
        public int? BuysAbove { get; set; }
        public int? BuysInSpread { get; set; }
        public int? BuysBelow { get; set; }
        public int? SellsAbove { get; set; }
        public int? SellsInSpread { get; set; }
        public int? SellsBelow { get; set; }
        public int? BuysSumSizes { get; set; }
        public int? SellsSumSizes { get; set; }
        public int? SellsSummedAboveBelowLong { get; set; }
        public int? SellsSummedAboveBelowMed { get; set; }
        public int? SellsSummedAboveBelowShort { get; set; }
        public int? SellsSummedInSpreadLong { get; set; }
        public int? SellsSummedInSpreadMed { get; set; }
        public int? SellsSummedInSpreadShort { get; set; }
        public int? BuysSummedInSpreadLong { get; set; }
        public int? BuysSummedInSpreadMed { get; set; }
        public int? BuysSummedInSpreadShort { get; set; }
        public int? BuysSummedAboveBelowLong { get; set; }
        public int? BuysSummedAboveBelowMed { get; set; }
        public int? BuysSummedAboveBelowShort { get; set; }
        public double? BuysRatio { get; set; }
        public double? SellsRatio { get; set; }
        public double? BuysAltRatio { get; set; }
        public double? SellsAltRatio { get; set; }
        public double? BollingerHigh { get; set; }
        public double? BollingerMid { get; set; }
        public double? BollingerLow { get; set; }
    }
}

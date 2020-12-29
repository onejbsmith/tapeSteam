using System;
using System.Collections.Generic;

#nullable disable

namespace tapeStream.Client.Models
{
    public partial class FrameReal
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string RealType { get; set; }
        public DateTime? DateTime { get; set; }
        public short? Seconds { get; set; }
        public double? MarkPrice { get; set; }
        public double? BuysTradeSizes { get; set; }
        public double? SellsTradeSizes { get; set; }
        public double? BidsBookSizes { get; set; }
        public double? AsksBookSizes { get; set; }
        public double? BuysPriceCount { get; set; }
        public double? SellsPriceCount { get; set; }
        public double? BuysAbove { get; set; }
        public double? BuysInSpread { get; set; }
        public double? BuysBelow { get; set; }
        public double? SellsAbove { get; set; }
        public double? SellsInSpread { get; set; }
        public double? SellsBelow { get; set; }
        public double? BuysSumSizes { get; set; }
        public double? SellsSumSizes { get; set; }
        public double? SellsSummedAboveBelowLong { get; set; }
        public double? SellsSummedAboveBelowMed { get; set; }
        public double? SellsSummedAboveBelowShort { get; set; }
        public double? SellsSummedInSpreadLong { get; set; }
        public double? SellsSummedInSpreadMed { get; set; }
        public double? SellsSummedInSpreadShort { get; set; }
        public double? BuysSummedInSpreadLong { get; set; }
        public double? BuysSummedInSpreadMed { get; set; }
        public double? BuysSummedInSpreadShort { get; set; }
        public double? BuysSummedAboveBelowLong { get; set; }
        public double? BuysSummedAboveBelowMed { get; set; }
        public double? BuysSummedAboveBelowShort { get; set; }
        public double? BuysRatio { get; set; }
        public double? SellsRatio { get; set; }
        public double? BuysAltRatio { get; set; }
        public double? SellsAltRatio { get; set; }
        public double? BollingerHigh { get; set; }
        public double? BollingerMid { get; set; }
        public double? BollingerLow { get; set; }
    }
}

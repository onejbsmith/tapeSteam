using System;
using System.Collections.Generic;

#nullable disable

namespace tapeStream.Client.Models
{
    public partial class Candle
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime RunDateTime { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public int Volume { get; set; }
    }
}

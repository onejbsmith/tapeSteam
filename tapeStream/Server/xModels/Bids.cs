using System;

namespace tapeStream.Server.Models
{
    public partial class Bids
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime? RunDateTime { get; set; }
        public decimal Price { get; set; }
        public int Size { get; set; }

        public virtual Streamed Stream { get; set; }
    }
}

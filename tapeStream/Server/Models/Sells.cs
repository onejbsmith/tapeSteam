using System;
using System.Collections.Generic;

namespace tapeStream.Server.Models
{
    public partial class Sells
    {
        public int Id { get; set; }
        public int StreamId { get; set; }
        public decimal Price { get; set; }
        public int Size { get; set; }
        public byte PriceLevel { get; set; }

        public virtual Streamed Stream { get; set; }
    }
}

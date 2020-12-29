using System;
using System.Collections.Generic;

namespace tapeStream.Server.Models
{
    public partial class Streamed
    {
        public Streamed()
        {
            Asks = new HashSet<Asks>();
            Bids = new HashSet<Bids>();
            Buys = new HashSet<Buys>();
            Marks = new HashSet<Marks>();
            Sells = new HashSet<Sells>();
        }

        public int StreamId { get; set; }
        public int? RunId { get; set; }
        public DateTime? StreamTime { get; set; }

        public virtual Runs Run { get; set; }
        public virtual ICollection<Asks> Asks { get; set; }
        public virtual ICollection<Bids> Bids { get; set; }
        public virtual ICollection<Buys> Buys { get; set; }
        public virtual ICollection<Marks> Marks { get; set; }
        public virtual ICollection<Sells> Sells { get; set; }
    }
}

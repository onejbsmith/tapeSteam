using System;
using System.Collections.Generic;

namespace tapeStream.Server.Models
{
    public partial class Runs
    {
        public Runs()
        {
            Streamed = new HashSet<Streamed>();
        }

        public int RunId { get; set; }
        public DateTime? RunDate { get; set; }
        public string Symbol { get; set; }

        public virtual ICollection<Streamed> Streamed { get; set; }
    }
}

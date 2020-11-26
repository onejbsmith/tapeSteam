using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Data
{
    public class TimeRowsChart
    {
        public Chart chart { get; set; }
        public Title title { get; set; }
        public Subtitle subtitle { get; set; }

        public Data data { get; set; }

    }

    public class Data
    {
        public string rowsURL { get; set; }
        public bool firstRowAsNames { get; set; }
        public bool enablePolling { get; set; }
    }

    public class Title
    {
        public string text { get; set; }
    }

    public class Subtitle
    {
        public string text { get; set; }
    }
    public class Chart
    {
        public string type { get; set; }
    }
}

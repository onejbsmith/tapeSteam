using System;
using System.Collections.Generic;
using System.Text;

namespace tapeStream.Shared.Data
{
    public class TDAChart
    {
        public static Bollinger bollingerBands = new Bollinger();

        public class Bollinger
        {
            public double low { get; set; }
            public double midlow { get; set; }
            public double mid { get; set; }
            public double midhigh { get; set; }
            public double high { get; set; }
        }

        public class TDA_Chart_Content
        {
            public string service { get; set; }
            public long timestamp { get; set; }
            public string command { get; set; }
            public Chart_Content[] content { get; set; }
        }

        public class Chart_Content
        {
            public int seq { get; set; }
            public string key { get; set; }
            public float open { get; set; }
            public float high { get; set; }
            public float low { get; set; }
            public float close { get; set; }
            public float volume { get; set; }
            public int sequence { get; set; }
            public long time { get; set; }
            public int _8 { get; set; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace tapeStream.Shared.Data
{
    public class TDAChart
    {
        public static Bollinger bollingerBands = new Bollinger();

        public static Chart_Content[] ohlcs;
        public static Chart_Content[] lastCandles;

        public static double avgBuysRatio;
        public static double avgSellsRatio;
        public static double avgLtBuysRatio;
        public static double avgLtSellsRatio;

        public static List<float> lstMarkPrices { get; set; } = new List<float>();
        public static List<float> lstBuysRatios { get; set; } = new List<float>();
        public static List<float> lstSellsRatios { get; set; } = new List<float>();
        public static List<string> lstSvcTimes { get; set; } = new List<string>();

        public static string svcDate = DateTime.Now.ToLongDateString();

        private static AverageSizes _avgRatios;
        public static AverageSizes avgRatios
        {
            get { return _avgRatios; }
            set
            {
                _avgRatios = value;
                if (value.averageSize != null)
                {
                    if (value.averageSize.ContainsKey("buys"))
                        avgBuysRatio = value.averageSize["buys"];
                    if (value.averageSize.ContainsKey("sells"))
                        avgBuysRatio = value.averageSize["sells"];
                }
            }
        }
        private static AverageSizes _avgLtRatios;
        public static AverageSizes avLtRatios
        {
            get { return _avgLtRatios; }
            set
            {
                _avgLtRatios = value;
                if (value.averageSize.ContainsKey("buys"))
                    avgLtBuysRatio = value.averageSize["buys"];
                if (value.averageSize.ContainsKey("sells"))
                    avgLtSellsRatio = value.averageSize["sells"];
            }
        }
        public static AverageSizes avgStRatios { get; set; }
        public static AverageSizes avgLtRatios { get; set; }
        public static int countBuysRatioUp;
        public static int countSellsRatioUp;

        public static Chart_Content lastCandle { get; set; }

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

        public static DateTime svcDateTime { get; set; }
        public static string svcDateTimeRaw { get; set; }
        public static AverageSizes avgSizes { get; set; }
        public static AverageSizes avgStSizes { get; set; }
        public static AverageSizes avgLtSizes { get; set; }

        public static decimal avgSells { get; set; }
        public static decimal avgBuys { get; set; }
        public static decimal avgStSells { get; set; }
        public static decimal avgStBuys { get; set; }
        public static decimal avgLtSells { get; set; }
        public static decimal avgLtBuys { get; set; }

        public static int countSellRatioUp { get; set; }

        public static int countBuysUp { get; set; }
        public static int countSellsUp { get; set; }
        public static string LongDateString { get; set; }
        public static bool isActive { get; set; }

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

using System;
using System.Collections.Generic;
using System.Text;

namespace tapeStream.Shared
{
    public class CONSTANTS
    {

        public static string clockFormat = "h:mm:ss MMM d, yyyy";
        public static readonly string[] valuesName = new string[]
        { "ALL", "NASDAQ_BOOK", "TIMESALE_EQUITY", "CHART_EQUITY", "OPTION",
          "QUOTE", "ACTIVES_NYSE", "ACTIVES_NASDAQ", "ACTIVES_OPTIONS", "TimeAndSales",
          "GaugeScore","PrintsPies" };

        //public static readonly IEnumerable<object> printSeconds;

        static public int[]
        printSeconds
        { get; set; } = new int[] { 5, 10, 30, 60, 120, 240, 600 };
    }
}

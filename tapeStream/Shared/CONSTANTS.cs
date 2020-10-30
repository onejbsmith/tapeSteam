using System;
using System.Collections.Generic;
using System.Text;

namespace tapeStream.Shared
{
    public class CONSTANTS
    {
        public static readonly List<string> lineNames = new List<string>()
        {
            "rawGaugesCombined",
            "staticValueMinus7",
            "staticValue0",
            "staticValue7",
            "movingAverage30sec",
            "movingAverage5min",
            "average10min"
        };


        public static string clockFormat = "h:mm:ss MMM d, yyyy";
        public static string messageQinputFolder = @"D:\MessageQs\Inputs\";
        public static string messageQarchiveFolder = @"D:\MessageQs\Archives\";
        public static string messageQOutputFolder = @"D:\MessageQs\Outputs\";


        public static readonly string[] valuesName = new string[]
        { "ALL", "NASDAQ_BOOK", "TIMESALE_EQUITY", "CHART_EQUITY", "OPTION",
          "QUOTE", "ACTIVES_NYSE", "ACTIVES_NASDAQ", "ACTIVES_OPTIONS", "TimeAndSales",
          "GaugeScore","PrintsPies","BookColsData","BookPiesData","BookBigPieData" };

        public static string TIMESALE_EQUITY = valuesName[2];
        public static string TimeAndSales = valuesName[9];
        public static string NASDAQ_BOOK = valuesName[1];
        public static string QUOTE = valuesName[5];

        //public static readonly IEnumerable<object> printSeconds;

        static public int[]
        printSeconds
        { get; set; } = new int[] { 5, 10, 30, 60, 120, 240, 600 };


    }
}

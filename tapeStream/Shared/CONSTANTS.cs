using System;
using System.Collections.Generic;
using tapeStream.Shared.Data;

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

        public static string buysRatio = "buysRatio";
        public static string buysAltRatio = "buysAltRatio";
        public static string buysInSpread = "buysInSpread";
        public static string buysAbove = "buysAbove";
        public static string buysBelow = "buysBelow";
        public static string asksBookSizes = "asksBookSizes";
        public static string buysSumSizes = "buysSumSizes";
        public static string buysPriceCount = "buysPriceCount";
        public static string buysTradeSizes = "buysTradeSizes";
        public static string buysR = "buysR";
        public static string markPrice = "buysTradeSizes";


        public static string sellsRatio = "sellsRatio";
        public static string sellsAltRatio = "sellsAltRatio";
        public static string sellsInSpread = "sellsInSpread";
        public static string sellsAbove = "sellsAbove";
        public static string sellsBelow = "sellsBelow";
        public static string bidsBookSizes = "bidsBookSizes";
        public static string sellsSumSizes = "sellsSumSizes";
        public static string sellsPriceCount = "sellsPriceCount";
        public static string sellsTradeSizes = "sellsTradeSizes";
        public static string sellsR = "sellsR";

        public static string buysSummedInSpreadLong = "buysSummedInSpreadLong";
        public static string buysSummedInSpreadMed = "buysSummedInSpreadMed";
        public static string buysSummedInSpreadShort = "buysSummedInSpreadShort";
        public static string buysSummedAboveBelowLong = "buysSummedAboveBelowLong";
        public static string buysSummedAboveBelowMed = "buysSummedAboveBelowMed";
        public static string buysSummedAboveBelowShort = "buysSummedAboveBelowShort";

        public static string sellsSummedInSpreadLong = "sellsSummedInSpreadLong";
        public static string sellsSummedInSpreadMed = "sellsSummedInSpreadMed";
        public static string sellsSummedInSpreadShort = "sellsSummedInSpreadShort";
        public static string sellsSummedAboveBelowLong = "sellsSummedAboveBelowLong";
        public static string sellsSummedAboveBelowMed = "sellsSummedAboveBelowMed";
        public static string sellsSummedAboveBelowShort = "sellsSummedAboveBelowShort";


        public static DataItem newDataItem =
         new DataItem()
         {
             Date = DateTime.Parse("2019-12-01"),
             Revenue = 6
         };
        public static BookDataItem newBookDataItem = new BookDataItem { Price = 350.11m, Size = 1000 };


        public static string clockFormat = "h:mm:ss MMM d, yyyy";
        public static string messageQinputFolder = @"D:\MessageQs\Inputs\";
        public static string messageQarchiveFolder = @"D:\MessageQs\Archives\";
        public static string messageQOutputFolder = @"D:\MessageQs\Outputs\";


        public static readonly string[] valuesName = new string[]
        { "ALL", "NASDAQ_BOOK", "TIMESALE_EQUITY", "CHART_EQUITY", "OPTION",
          "QUOTE", "ACTIVES_NYSE", "ACTIVES_NASDAQ", "ACTIVES_OPTIONS", "getIncrementalRatioFrames",
          "GaugeScore","PrintsPies","BookColsData","BookPiesData","BookBigPieData" };

        public static string TIMESALE_EQUITY = valuesName[2];
        public static string TimeAndSales = valuesName[9];
        public static string NASDAQ_BOOK = valuesName[1];
        public static string QUOTE = valuesName[5];

        static long now = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
        public static Dictionary<string, BookDataItem[]> newBookColumnsData = new Dictionary<string, BookDataItem[]>()
            { { "bids", new BookDataItem[] {new BookDataItem{ Price = 0, Size = 0, time = now, dateTime = DateTime.Now}}},
              { "asks", new BookDataItem[] { new BookDataItem { Price = 0, Size = 0, time = now, dateTime = DateTime.Now } }},
              { "salesAtBid", new BookDataItem[] { new BookDataItem { Price = 0, Size = 0, time = now, dateTime = DateTime.Now } }},
              { "salesAtAsk", new BookDataItem[] { new BookDataItem { Price = 0, Size = 0, time = now, dateTime = DateTime.Now } }}
            };

        //public static readonly IEnumerable<object> printSeconds;

        static public int[]
                printSeconds
        { get; set; } = new int[] { 5, 10, 30, 60, 120, 240, 600 };


        /// <summary>
        /// "#7cb5ec", "#434348", "#90ed7d", "#f7a35c", "#8085e9", "#f15c80", "#e4d354", "#2b908f", "#f45b5b", "#91e8e1"
        /// </summary>

        public static string asksColor = "#e7934c"; //  "#e4d354";
        public static string bidsColor = "#0479cc";
        public static string buysColor = "#90ed7d";
        public static string asksLtColor = "#5055b9"; //"#cb6992";
        public static string bidsLtColor = "#0439ac";

        public static string sellsColor = "#f45b5b";
        public static string spreadColor = "#8085e9";
    }
}

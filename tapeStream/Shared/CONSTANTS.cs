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
          "QUOTE", "ACTIVES_NYSE", "ACTIVES_NASDAQ", "ACTIVES_OPTIONS", "TimeAndSales",
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


    }
}

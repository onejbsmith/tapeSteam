using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Shared.Data
{


    public class TDABook
    {

        public static List<RatioFrame> lstRatioFrames = new List<RatioFrame>();



        public static int ratiosDepth = 150;
        public static int ratiosBack = 30;
        public static bool? isCurrentEndTime = true;
        public static DateTime? startTime = DateTime.Now;
        public static DateTime? endTime = DateTime.Now;
        public static int seconds = 30;
        public static bool? showRegressionCurves;

        //public static Dictionary<string,BookDataItem[]> getBookColumnsData()
        //{
        //    var asksData = new BookDataItem[0];
        //    asksData = TDAStreamerData.lstAsks.ToArray();

        //    var bidsData = new BookDataItem[0];
        //    bidsData = TDAStreamerData.lstBids.ToArray();

        //    return new Dictionary<string, BookDataItem[]>()
        //    { { "asks", asksData }, { "bids", bidsData } };
        //}

        //public static BookDataItem[] getBookPieData(int seconds)
        //{
        //    var bookData = new BookDataItem[2];

        //    TDAStreamerData.lstAllBids.RemoveAll(t => t.time < DateTime.Now.AddSeconds(-600));
        //    TDAStreamerData.lstAllAsks.RemoveAll(t => t.time < DateTime.Now.AddSeconds(-600));

        //    if (TDAStreamerData.lstAllBids.Count == 0 || TDAStreamerData.lstAllAsks.Count == 0) 
        //        return bookData;

        //    double bidSize =  TDAStreamerData.lstAllBids
        //        .Where(t => t.time >= DateTime.Now.AddSeconds(-seconds))
        //        .Sum(t => t.Size);

        //    double askSize =  TDAStreamerData.lstAllAsks
        //        .Where(t => t.time >= DateTime.Now.AddSeconds(-seconds))
        //        .Sum(t => t.Size);

        //    var allBids = new BookDataItem() 
        //    { Price = TDAStreamerData. lstAllBids[0].Price, Size = bidSize };

        //    var allAsks = new BookDataItem() 
        //    { Price =  TDAStreamerData.lstAllAsks[0].Price, Size = askSize };

        //    bookData[1] = allBids;  // Bids Sum in last seconds Slice
        //    bookData[0] = allAsks;  // Asks Sum in last seconds Slice is size

        //    return bookData;
        //}

        //public static BookDataItem[] getBookCompositePieData()
        //{
        //    var bookData = new BookDataItem[2];
        //    if ( TDAStreamerData.lstAllAsks.Count == 0) 
        //        return bookData;

        //    double bidSize2 =  TDAStreamerData.lstAllBids.Where(t => t.time >= DateTime.Now.AddSeconds(-2)).Sum(t => t.Size) * 8;
        //    double askSize2 =  TDAStreamerData.lstAllAsks.Where(t => t.time >= DateTime.Now.AddSeconds(-2)).Sum(t => t.Size) * 8;
        //    double bidSize10 =  TDAStreamerData.lstAllBids.Where(t => t.time >= DateTime.Now.AddSeconds(-10)).Sum(t => t.Size) * 4;
        //    double askSize10 =  TDAStreamerData.lstAllAsks.Where(t => t.time >= DateTime.Now.AddSeconds(-10)).Sum(t => t.Size) * 4;
        //    double bidSize30 =  TDAStreamerData.lstAllBids.Where(t => t.time >= DateTime.Now.AddSeconds(-30)).Sum(t => t.Size) * 2;
        //    double askSize30 =  TDAStreamerData.lstAllAsks.Where(t => t.time >= DateTime.Now.AddSeconds(-30)).Sum(t => t.Size) * 2;
        //    double bidSize60 =  TDAStreamerData.lstAllBids.Where(t => t.time >= DateTime.Now.AddSeconds(-60)).Sum(t => t.Size);
        //    double askSize60 =  TDAStreamerData.lstAllAsks.Where(t => t.time >= DateTime.Now.AddSeconds(-60)).Sum(t => t.Size);

        //    var allBids = new BookDataItem() 
        //    { 
        //        Price =  TDAStreamerData.lstAllBids[0].Price, 
        //        Size = bidSize2 + bidSize10 + bidSize30 + bidSize60 
        //    };
        //    var allAsks = new BookDataItem() 
        //    { 
        //        Price = TDAStreamerData. lstAllAsks[0].Price, 
        //        Size = askSize2 + askSize10 + askSize30 + askSize60 
        //    };

        //    bookData[1] = allBids;  // Bids Sum over all times Slice
        //    bookData[0] = allAsks;  // Asks Slice

        //    return bookData;
        //}

    }
    public class BookDataItem
    {
        public decimal Price { get; set; }
        public double Size { get; set; }
        public long time { get; set; }
        public DateTime dateTime { get; set; }
    }

    public class AverageSizes
    {
        public Dictionary<string, double> averageSize { get; set; }
    }

    public class RatioFrame
    {
        public double sellsAbove { get; set; }
        public double bidsBookSizes { get; set; }
        public double buysSumSizes { get; set; }
        public double sellsSumSizes { get; set; }
        public double sellsInSpread { get; set; }
        public double sellsBelow { get; set; }
        public double buysInSpread { get; set; }
        public double buysAbove { get; set; }
        public double buysBelow { get; set; }
        public double asksBookSizes { get; set; }

        public DateTime dateTime { get; set; }
        public double buysRatio { get; set; }
        public double buysAltRatio { get; set; }
        public double sellsRatio { get; set; }
        public double sellsAltRatio { get; set; }
        public decimal markPrice { get; set; }
        //public double sellsR { get; set; }
        //public double buysR { get; set; }
        public double buysTradeSizes { get; set; }
        public double sellsTradeSizes { get; set; }
        public int buysPriceCount { get; set; }
        public int sellsPriceCount { get; set; }



        [NotMapped]
        public int seconds { get; set; }
        [NotMapped]
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

    }

    public class JsonArrayOfArrays
    {
        public object[][] datum { get; set; }
    }

}

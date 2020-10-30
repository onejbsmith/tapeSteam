using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Shared.Data
{
    public class TDABook
    {

        public static Dictionary<string,BookDataItem[]> getBookColumnsData()
        {
            var asksData = new BookDataItem[0];
            asksData = TDAStreamerData.lstAsks.ToArray();

            var bidsData = new BookDataItem[0];
            bidsData = TDAStreamerData.lstBids.ToArray();

            return new Dictionary<string, BookDataItem[]>()
            { { "asks", asksData }, { "bids", bidsData } };
        }

        public static BookDataItem[] getBookPieData(int seconds)
        {
            var bookData = new BookDataItem[2];

            TDAStreamerData.lstAllBids.RemoveAll(t => t.time < DateTime.Now.AddSeconds(-600));
            TDAStreamerData.lstAllAsks.RemoveAll(t => t.time < DateTime.Now.AddSeconds(-600));

            if (TDAStreamerData.lstAllBids.Count == 0 || TDAStreamerData.lstAllAsks.Count == 0) 
                return bookData;

            double bidSize =  TDAStreamerData.lstAllBids
                .Where(t => t.time >= DateTime.Now.AddSeconds(-seconds))
                .Sum(t => t.Size);

            double askSize =  TDAStreamerData.lstAllAsks
                .Where(t => t.time >= DateTime.Now.AddSeconds(-seconds))
                .Sum(t => t.Size);

            var allBids = new BookDataItem() 
            { Price = TDAStreamerData. lstAllBids[0].Price, Size = bidSize };
            
            var allAsks = new BookDataItem() 
            { Price =  TDAStreamerData.lstAllAsks[0].Price, Size = askSize };

            bookData[1] = allBids;  // Bids Sum in last seconds Slice
            bookData[0] = allAsks;  // Asks Sum in last seconds Slice is size

            return bookData;
        }

        public static BookDataItem[] getBookCompositePieData()
        {
            var bookData = new BookDataItem[2];
            if ( TDAStreamerData.lstAllAsks.Count == 0) 
                return bookData;

            double bidSize2 =  TDAStreamerData.lstAllBids.Where(t => t.time >= DateTime.Now.AddSeconds(-2)).Sum(t => t.Size) * 8;
            double askSize2 =  TDAStreamerData.lstAllAsks.Where(t => t.time >= DateTime.Now.AddSeconds(-2)).Sum(t => t.Size) * 8;
            double bidSize10 =  TDAStreamerData.lstAllBids.Where(t => t.time >= DateTime.Now.AddSeconds(-10)).Sum(t => t.Size) * 4;
            double askSize10 =  TDAStreamerData.lstAllAsks.Where(t => t.time >= DateTime.Now.AddSeconds(-10)).Sum(t => t.Size) * 4;
            double bidSize30 =  TDAStreamerData.lstAllBids.Where(t => t.time >= DateTime.Now.AddSeconds(-30)).Sum(t => t.Size) * 2;
            double askSize30 =  TDAStreamerData.lstAllAsks.Where(t => t.time >= DateTime.Now.AddSeconds(-30)).Sum(t => t.Size) * 2;
            double bidSize60 =  TDAStreamerData.lstAllBids.Where(t => t.time >= DateTime.Now.AddSeconds(-60)).Sum(t => t.Size);
            double askSize60 =  TDAStreamerData.lstAllAsks.Where(t => t.time >= DateTime.Now.AddSeconds(-60)).Sum(t => t.Size);

            var allBids = new BookDataItem() 
            { 
                Price =  TDAStreamerData.lstAllBids[0].Price, 
                Size = bidSize2 + bidSize10 + bidSize30 + bidSize60 
            };
            var allAsks = new BookDataItem() 
            { 
                Price = TDAStreamerData. lstAllAsks[0].Price, 
                Size = askSize2 + askSize10 + askSize30 + askSize60 
            };

            bookData[1] = allBids;  // Bids Sum over all times Slice
            bookData[0] = allAsks;  // Asks Slice

            return bookData;
        }

    }
}

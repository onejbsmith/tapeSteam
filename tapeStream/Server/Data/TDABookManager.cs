using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Shared;

namespace tdaStreamHub.Data
{
    public class TDABookManager

    {
        public static List<BookEntry> lstBookEntry = new List<BookEntry>();
        public static async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData()
        {
            var asksData = new BookDataItem[0];
            asksData = TDAStreamerData.lstAsks.ToArray();

            var bidsData = new BookDataItem[0];
            bidsData = TDAStreamerData.lstBids.ToArray();

            return new Dictionary<string, BookDataItem[]>()
            { { "asks", asksData }, { "bids", bidsData } };
        }

        public static async Task<Dictionary<int, List<BookDataItem>>> getBookPiesData()
        {
            Dictionary<int, List<BookDataItem>> dictBookPies = new Dictionary<int, List<BookDataItem>>();
            foreach (var seconds in CONSTANTS.printSeconds)
            {
                var newItem = await getBookPieData(seconds);
                dictBookPies.Add(seconds, newItem.ToList());
            }
            await Task.CompletedTask;
            return dictBookPies;
        }
        private static async Task<BookDataItem[]> getBookPieData(int seconds)
        {
            var bookData = new BookDataItem[2];

            TDAStreamerData.lstAllBids.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));
            TDAStreamerData.lstAllAsks.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));

            if (TDAStreamerData.lstAllBids.Count == 0 || TDAStreamerData.lstAllAsks.Count == 0)
                return bookData;

            double bidSize = TDAStreamerData.lstAllBids
                .Where(t => t.dateTime >= DateTime.Now.AddSeconds(-seconds))
                .Sum(t => t.Size);

            double askSize = TDAStreamerData.lstAllAsks
                .Where(t => t.dateTime >= DateTime.Now.AddSeconds(-seconds))
                .Sum(t => t.Size);

            var allBids = new BookDataItem()
            { Price = TDAStreamerData.lstAllBids[0].Price, Size = bidSize };

            var allAsks = new BookDataItem()
            { Price = TDAStreamerData.lstAllAsks[0].Price, Size = askSize };

            bookData[1] = allBids;  // Bids Sum in last seconds Slice
            bookData[0] = allAsks;  // Asks Sum in last seconds Slice is size

            return bookData;
        }
        public static async Task<BookDataItem[]> getBookCompositePieData()
        {
            var bookData = new BookDataItem[2];
            if (TDAStreamerData.lstAllAsks.Count == 0)
                return bookData;

            double bidSize2 = TDAStreamerData.lstAllBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-2)).Sum(t => t.Size) * 8;
            double askSize2 = TDAStreamerData.lstAllAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-2)).Sum(t => t.Size) * 8;
            double bidSize10 = TDAStreamerData.lstAllBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-10)).Sum(t => t.Size) * 4;
            double askSize10 = TDAStreamerData.lstAllAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-10)).Sum(t => t.Size) * 4;
            double bidSize30 = TDAStreamerData.lstAllBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-30)).Sum(t => t.Size) * 2;
            double askSize30 = TDAStreamerData.lstAllAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-30)).Sum(t => t.Size) * 2;
            double bidSize60 = TDAStreamerData.lstAllBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-60)).Sum(t => t.Size);
            double askSize60 = TDAStreamerData.lstAllAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-60)).Sum(t => t.Size);

            var allBids = new BookDataItem()
            {
                Price = TDAStreamerData.lstAllBids[0].Price,
                Size = bidSize2 + bidSize10 + bidSize30 + bidSize60
            };
            var allAsks = new BookDataItem()
            {
                Price = TDAStreamerData.lstAllAsks[0].Price,
                Size = askSize2 + askSize10 + askSize30 + askSize60
            };

            bookData[1] = allBids;  // Bids Sum over all times Slice
            bookData[0] = allAsks;  // Asks Slice

            await Task.CompletedTask;
            return bookData;
        }


        public class BookEntry
        {
            public long time { get; set; }
            public float bid { get; set; }
            public float ask { get; set; }
            public int bidSize { get; set; }
            public int askSize { get; set; }
            public int[] printsSize { get; set; } = new int[5];
        }

        public static List<BookDataItem> lstAsks = new List<BookDataItem>();
        public static List<BookDataItem> lstBids = new List<BookDataItem>();
        public static List<BookDataItem> lstAllAsks = new List<BookDataItem>();
        public static List<BookDataItem> lstAllBids = new List<BookDataItem>();
        public static async Task Decode(string symbol, string content)
        {
            var all = JObject.Parse(content);
            var bids = all["2"];
            var asks = all["3"];
            lstBids.Clear();
            lstAsks.Clear();
            lstAllBids.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));
            lstAllAsks.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));


            /// Grab all raw bids
            /// Cosolidate into three bid groups
            /// do same for asks
            /// Add bids then asks to display set
            /// 
            var n = bids.Count();

            if (n == 0) return;
            long now = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
            var basePrice = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)bids[0]["0"]).Value);
            for (int i = 0; i < n; i++)
            {
                var price = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)bids[i]["0"]).Value);
                var size = Convert.ToDouble(((Newtonsoft.Json.Linq.JValue)bids[i]["1"]).Value);

                if (Math.Abs(price - basePrice) < 0.30m)
                {
                    var bid = new BookDataItem() { Price = price, Size = size, time = now, dateTime = DateTime.Now };
                    lstBids.Add(bid);
                    lstAllBids.Add(bid);
                    //sumBidSize += size;
                }
            }

            n = asks.Count();
            if (n == 0) return;
            var baseAskPrice = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)asks[0]["0"]).Value);
            for (int i = 0; i < n; i++)
            {
                var price = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)asks[i]["0"]).Value);
                var size = Convert.ToDouble(((Newtonsoft.Json.Linq.JValue)asks[i]["1"]).Value);
                if (Math.Abs(price - baseAskPrice) < 0.30m)
                {
                    var ask = new BookDataItem() { Price = price, Size = size, time = now, dateTime = DateTime.Now };
                    lstAsks.Add(ask);
                    lstAllAsks.Add(ask);
                    //sumAskSize += size;
                }
            }
            try
            {
                var maxBid = lstBids.Max(bids => bids.Price);
                var bidEntry = lstBids.Where(bid => bid.Price == maxBid).First();

                var minAsk = lstAsks.Min(asks => asks.Price);
                var askEntry = lstAsks.Where(ask => ask.Price == minAsk).First();
                var lastTime = 0d;
                try
                {
                    lastTime = lstBookEntry.Last().time;
                }// is this right or should you query for last time before 
                catch { }


                int[] printsSizes = new int[5];
                var prints = TDAStreamerData.timeSales[symbol].Where(ts => ts.bookTime >= lastTime && ts.bookTime < bidEntry.time);

                for (var level = 0; level < 5; level++)
                {
                    try
                    {
                        printsSizes[level] = (int)prints.Where(pr => pr.level == level).Sum(ts => ts.size);
                    }
                    catch { }
                }

                lstBookEntry.Add(
                      new BookEntry()
                      {
                          time = bidEntry.time,
                          bid = (float)bidEntry.Price,
                          bidSize = (int)bidEntry.Size,
                          ask = (float)askEntry.Price,
                          askSize = (int)askEntry.Size,
                          printsSize = printsSizes
                      });

                string json = JsonSerializer.Serialize<List<BookEntry>>(lstBookEntry);
                await FilesManager.SendToMessageQueue("NasdaqBook", DateTime.Now, json);

            }
            catch { }  // in case 


            //lstAllBids.Add(new BookDataItem() { Price = baseAskPrice, Size = sumAskSize });
            //lstAllBids.Add(new BookDataItem() { Price = basePrice, Size = sumBidSize });

            // var bookData = TDAPrintsManager.getBookColumnsData();

            //string json = JsonSerializer.Serialize<Dictionary<string, BookDataItem[]>>(bookData);
            //await FilesManager.SendToMessageQueue("NasdaqBook", DateTime.Now, json);


            await Task.CompletedTask;
        }


    }
}

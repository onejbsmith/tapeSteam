using Microsoft.CodeAnalysis.FindSymbols;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Shared;
using tapeStream.Shared.Data;

namespace tapeStream.Server.Data
{
    public class TDABookManager

    {
        public static List<BookEntry> lstBookEntry = new List<BookEntry>();
        public static List<BookDataItem> lstAsks = new List<BookDataItem>();
        public static List<BookDataItem> lstBids = new List<BookDataItem>();
        public static List<BookDataItem> lstAllAsks = new List<BookDataItem>();
        public static List<BookDataItem> lstAllBids = new List<BookDataItem>();

        public static List<BookDataItem> lstSalesAtAsk = new List<BookDataItem>();
        public static List<BookDataItem> lstSalesAtBid = new List<BookDataItem>();
        public static List<BookDataItem> lstAllSalesAtAsk = new List<BookDataItem>();
        public static List<BookDataItem> lstAllSalesAtBid = new List<BookDataItem>();

        public static async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData()
        {
            Dictionary<string, BookDataItem[]> it = getBookData();
            await Task.CompletedTask;
            return it;
        }

        public static async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData(int seconds)
        {
            Dictionary<string, BookDataItem[]> it = getBookData(seconds);
            await Task.CompletedTask;
            return it;
        }

        static Dictionary<string, BookDataItem[]> getBookData()
        {
            var asksData = new BookDataItem[0];
            asksData = lstAsks.ToArray();

            var bidsData = new BookDataItem[0];
            bidsData = lstBids.ToArray();

            var salesAtAskData = new BookDataItem[0];
            salesAtAskData = lstSalesAtAsk.ToArray();

            var salesAtBidData = new BookDataItem[0];
            salesAtBidData = lstSalesAtBid.ToArray();

            var it = new Dictionary<string, BookDataItem[]>()
            { { "asks", asksData }, { "bids", bidsData } , { "salesAtAsk",salesAtAskData}, {"salesAtBid", salesAtBidData} };
            return it;
        }


        static Dictionary<string, BookDataItem[]> getBookData(int seconds)
        {
            long now = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;

            var asksData = new BookDataItem[0];
            asksData = getBookDataItemArray(seconds, now, lstAllAsks);

            var bidsData = new BookDataItem[0];
            bidsData = getBookDataItemArray(seconds, now, lstAllBids);

            var salesAtAskData = new BookDataItem[0];
            salesAtAskData = getBookDataItemArray(seconds, now, lstAllSalesAtAsk); ;

            var salesAtBidData = new BookDataItem[0];
            salesAtBidData = getBookDataItemArray(seconds, now, lstAllSalesAtBid); ;

            var it = new Dictionary<string, BookDataItem[]>()
            { { "asks", asksData }, { "bids", bidsData } , { "salesAtAsk",salesAtAskData}, {"salesAtBid", salesAtBidData} };
            return it;
        }

        private static BookDataItem[] getBookDataItemArray(int seconds, long now, List<BookDataItem> lstItems)
        {
            var lstBookItems = lstItems;
            lstBookItems.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-seconds));
            var lstBookItemsData = lstBookItems
                .GroupBy(lstBookItems => lstBookItems.Price)
                .Select(lstBookItems => new BookDataItem()
                {
                    Price = lstBookItems.Key,
                    dateTime = DateTime.Now,
                    time = now,
                    Size = lstBookItems.Sum(item => item.Size)
                }
                ).ToArray();
            return lstBookItemsData;
        }
        public static async Task<Dictionary<string, BookDataItem[]>> getBookPiesData()
        {
            Dictionary<string, BookDataItem[]> dictBookPies = new Dictionary<string, BookDataItem[]>();
            foreach (var seconds in CONSTANTS.printSeconds)
            {
                var newItem = await getBookPieData(seconds);
                if (newItem[0] == null)
                    newItem = new BookDataItem[2]
                    {
                        new BookDataItem() {
                            dateTime=DateTime.Now,
                            Price = 0,
                            Size = 0,
                            time = 0
                        },
                        new BookDataItem()
                        {
                            dateTime = DateTime.Now,
                            Price = 0,
                            Size = 0,
                            time = 0
                        }
                    };

                dictBookPies.Add(seconds.ToString(), newItem);
            }
            await Task.CompletedTask;
            return dictBookPies;
        }

        private static async Task<BookDataItem[]> getBookPieData(int seconds)
        {
            var bookData = new BookDataItem[2];

            lstAllBids.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));
            lstAllAsks.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));

            if (lstAllBids.Count == 0 || lstAllAsks.Count == 0)
                return bookData;

            double bidSize = lstAllBids
                .Where(t => t.dateTime >= DateTime.Now.AddSeconds(-seconds))
                .Sum(t => t.Size);

            double askSize = lstAllAsks
                .Where(t => t.dateTime >= DateTime.Now.AddSeconds(-seconds))
                .Sum(t => t.Size);

            var allBids = new BookDataItem()
            { Price = lstAllBids[0].Price, Size = bidSize };

            var allAsks = new BookDataItem()
            { Price = lstAllAsks[0].Price, Size = askSize };

            bookData[1] = allBids;  // Bids Sum in last seconds Slice
            bookData[0] = allAsks;  // Asks Sum in last seconds Slice is size

            await Task.CompletedTask;
            return bookData;
        }
        public static async Task<BookDataItem[]> getBookCompositePieData()
        {
            var bookData = new BookDataItem[]
            {
                 CONSTANTS.newBookDataItem,
                 CONSTANTS.newBookDataItem
            };

            if (lstAllAsks.Count == 0)
                return bookData;

            double bidSize2 = lstAllBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-2)).Sum(t => t.Size) * 8;
            double askSize2 = lstAllAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-2)).Sum(t => t.Size) * 8;
            double bidSize10 = lstAllBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-10)).Sum(t => t.Size) * 4;
            double askSize10 = lstAllAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-10)).Sum(t => t.Size) * 4;
            double bidSize30 = lstAllBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-30)).Sum(t => t.Size) * 2;
            double askSize30 = lstAllAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-30)).Sum(t => t.Size) * 2;
            double bidSize60 = lstAllBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-60)).Sum(t => t.Size);
            double askSize60 = lstAllAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-60)).Sum(t => t.Size);

            var allBids = new BookDataItem()
            {
                Price = lstAllBids[0].Price,
                Size = bidSize2 + bidSize10 + bidSize30 + bidSize60
            };
            var allAsks = new BookDataItem()
            {
                Price = lstAllAsks[0].Price,
                Size = askSize2 + askSize10 + askSize30 + askSize60
            };

            bookData[1] = allBids;  // Bids Sum over all times Slice
            bookData[0] = allAsks;  // Asks Slice

            await Task.CompletedTask;
            return bookData;
        }


        public class BookEntry
        {
            public DateTime dateTime { get; set; }
            public long time { get; set; }
            public float bid { get; set; }
            public float ask { get; set; }
            public int bidSize { get; set; }
            public int askSize { get; set; }
            public int[] printsSize { get; set; } = new int[5];
        }

        public static async Task Decode(string symbol, string content)
        {
            var all = JObject.Parse(content);
            var bids = all["2"];
            var asks = all["3"];

            lstBids.Clear();
            lstAsks.Clear();

            lstAllBids.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));
            lstAllAsks.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));

            lstSalesAtAsk.Clear();
            lstSalesAtBid.Clear();

            lstAllSalesAtBid.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));
            lstAllSalesAtAsk.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));

            /// Grab all raw bids
            /// Cosolidate into three bid groups
            /// do same for asks
            /// Add bids then asks to display set
            /// 
            var n = bids.Count();

            if (n == 0) return;
            long now = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
            var baseBidPrice = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)bids[0]["0"]).Value);
            for (int i = 0; i < n; i++)
            {
                var price = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)bids[i]["0"]).Value);
                var size = Convert.ToDouble(((Newtonsoft.Json.Linq.JValue)bids[i]["1"]).Value);

                if (Math.Abs(price - baseBidPrice) < 0.30m)
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
                    //lstSalesAtAsk(sales)
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

                var lastSale = TDAStreamerData.timeSales[symbol].Last();

                try
                {
                    /// This sb the last book entry before the last time and sales
                    if (lastSale != null)
                    {
                        var bookEntries = lstBookEntry.Where(be => be.time < lastSale.time);
                        if (bookEntries.Any())
                            lastTime = bookEntries.Last().time;
                        else
                            lastTime = now;
                    }
                    else
                        lastTime = now;
                }// is this right or should you query for last time before 
                catch { }


                /// Sum the sales by level and distribute sales from the middle level to the 2 levels on either side of the middle.
                int[] printsSizes = new int[5];
                var prints = TDAStreamerData.timeSales[symbol].Where(ts => ts.bookTime >= lastTime && ts.bookTime < bidEntry.time);
                var printsByPriceAtLevel = prints.GroupBy(sale => new { sale.price, sale.level, sale.TimeDate })
                .Select(sales => new PrintSale()
                {
                    price = sales.Key.price,
                    level = sales.Key.level,
                    size = sales.Sum(sale => sale.size)
                });

                var salesAtMid = printsByPriceAtLevel.Where(sale => sale.level == 3);

                var salesAtBidByPriceAtLevel = printsByPriceAtLevel.Where(sale => sale.level == 1 || sale.level == 2);
                foreach (var sale in salesAtBidByPriceAtLevel)
                {
                    var mids = salesAtMid.Where(mids => mids.price == sale.price);
                    if (mids.Any()) sale.size += mids.First().size / 2;
                }

                lstSalesAtBid = salesAtBidByPriceAtLevel.GroupBy(sale => sale.price)
                .Select(sales => new BookDataItem
                {
                    Price = (decimal)sales.Key,
                    dateTime = DateTime.Now,
                    Size = sales.Sum(sale => sale.size)
                }).ToList();
                lstAllSalesAtBid.AddRange(lstSalesAtBid);

                var salesAtAskByPriceAtLevel = printsByPriceAtLevel.Where(sale => sale.level == 4 || sale.level == 5);
                foreach (var sale in salesAtAskByPriceAtLevel)
                {
                    var mids = salesAtMid.Where(mids => mids.price == sale.price);
                    if (mids.Any()) sale.size += mids.First().size / 2;
                }

                lstSalesAtAsk = salesAtAskByPriceAtLevel.GroupBy(sale => sale.price)
                .Select(sales => new BookDataItem
                {
                    Price = (decimal)sales.Key,
                    dateTime = DateTime.Now,
                    Size = sales.Sum(sale => sale.size)
                }).ToList();
                lstAllSalesAtAsk.AddRange(lstSalesAtAsk);

                //Dictionary<string, BookDataItem[]> it = getBookData();
                //string json = JsonSerializer.Serialize<Dictionary<string, BookDataItem[]>>(it);
                //await FilesManager.SendToMessageQueue("BookedTimeSales", DateTime.Now, json);

                /// We need the sales by Price for the chart
                /// Need two series, salesAtBid, salesAtAsk
                ///

                var newBookEntry = new BookEntry()
                {
                    time = bidEntry.time,
                    dateTime = bidEntry.dateTime,
                    bid = (float)bidEntry.Price,
                    bidSize = (int)bidEntry.Size,
                    ask = (float)askEntry.Price,
                    askSize = (int)askEntry.Size,
                    printsSize = printsSizes
                };
                lstBookEntry.Add(newBookEntry);


                //json = JsonSerializer.Serialize<BookEntry>(newBookEntry);
                //await FilesManager.SendToMessageQueue("NasdaqBook", DateTime.Now, json);

            }
            catch { }  // in case 




            //lstAllBids.Add(new BookDataItem() { Price = baseAskPrice, Size = sumAskSize });
            //lstAllBids.Add(new BookDataItem() { Price = basePrice, Size = sumBidSize });

            // var bookData = TDAPrintsManager.getBookColumnsData();

            //string json = JsonSerializer.Serialize<Dictionary<string, BookDataItem[]>>(bookData);
            //await FilesManager.SendToMessageQueue("NasdaqBook", DateTime.Now, json);


            await Task.CompletedTask;
        }

        private class PrintSale
        {
            public float price { get; set; }
            public int level { get; set; }
            public float size { get; set; }
            public DateTime dateTime { get; set; }
        }
    }
}

using Microsoft.CodeAnalysis.FindSymbols;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Server.Managers;
using tapeStream.Shared;
using tapeStream.Shared.Data;

namespace tapeStream.Server.Data
{
    public class TDABookManager

    {
        public static List<BookEntry> lstBookEntry = new List<BookEntry>();

        public static List<BookDataItem> lstAsks = new List<BookDataItem>();
        public static List<BookDataItem> lstBids = new List<BookDataItem>();
        public static List<BookDataItem> lstSalesAtAsk = new List<BookDataItem>();
        public static List<BookDataItem> lstSalesAtBid = new List<BookDataItem>();

        public static object lstBookData { get; private set; }

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
            var asksData = lstAsks.ToArray();

            var bidsData = lstBids.ToArray();

            var salesAtAskData = lstSalesAtAsk.ToArray();

            var salesAtBidData = lstSalesAtBid.ToArray();


            var it = new Dictionary<string, BookDataItem[]>()
            { { "asks", asksData }, { "bids", bidsData } , { "salesAtAsk",salesAtAskData}, {"salesAtBid", salesAtBidData} };
            return it;
        }


        static Dictionary<string, BookDataItem[]> getBookData(int seconds)
        {
            long now = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;

            var asksData = getBookDataItemArray(seconds, now, TDAStreamerData.lstAllAsks);

            var bidsData = getBookDataItemArray(seconds, now, TDAStreamerData.lstAllBids);

            var salesAtAskData = getBookDataItemArray(seconds, now, TDAStreamerData.lstAllSalesAtAsk); ;

            var salesAtBidData = getBookDataItemArray(seconds, now, TDAStreamerData.lstAllSalesAtBid); ;

            var it = new Dictionary<string, BookDataItem[]>()
            { { "asks", asksData }, { "bids", bidsData } , { "salesAtAsk",salesAtAskData}, {"salesAtBid", salesAtBidData}  };


            return it;
        }


        static Dictionary<string, BookDataItem[]> getLTBookData(int seconds)
        {
            long now = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;


            var asksData = TDAStreamerData.lstALLAsks.ToArray();

            var bidsData = TDAStreamerData.lstALLBids.ToArray();

            var salesAtAskData = TDAStreamerData.lstALLSalesAtAsk.ToArray();

            var salesAtBidData = TDAStreamerData.lstALLSalesAtBid.ToArray();

            if (seconds > 0)
            {
                asksData = TDAStreamerData.lstALLAsks.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-seconds)).ToArray();
                bidsData = TDAStreamerData.lstALLBids.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-seconds)).ToArray();
                salesAtAskData = TDAStreamerData.lstALLSalesAtAsk.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-seconds)).ToArray();
                salesAtBidData = TDAStreamerData.lstALLSalesAtBid.Where(t => t.dateTime >= DateTime.Now.AddSeconds(-seconds)).ToArray();
            }

            var it = new Dictionary<string, BookDataItem[]>()
            { { "asks", asksData.ToArray() }, { "bids", bidsData } , { "salesAtAsk",salesAtAskData}, {"salesAtBid", salesAtBidData}  };


            return it;
        }
        public static async Task<AverageSizes> getAverages(int seconds)
        {

            /// Get the book data for the number of seconds
            Dictionary<string, BookDataItem[]> dictBookDataItem = getBookData(seconds);
            var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };
            var avgSizes = new AverageSizes()
            {
                averageSize = new Dictionary<string, double>()
            };
#if tracing
            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, dictBookDataItem, $"dictBookDataItem ({seconds} seconds)");
            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, avgSizes, "init avgSizes");
#endif
            /// Calc average for each data type

            try
            {
                foreach (var name in seriesOrder)
                {

                    var avgSize = 0d;

                    BookDataItem[] items = dictBookDataItem[name];
                    if (items.Length > 0)
                    {
                        avgSize = items.Average(item => item.Size);
                    }
                    avgSizes.averageSize.Add(name, avgSize);

                }

                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, avgSizes, "filled avgSizes");

            }
            catch
            {
                //JsConsole.JsConsole.Confirm(TDAStreamerData.jSRuntime, ex.ToString());

            }
            await Task.CompletedTask;
            return avgSizes;
        }

        public static async Task<AverageSizes> getLtAverages(int seconds)
        {

            /// Get the book data for the number of seconds
            Dictionary<string, BookDataItem[]> dictBookDataItem = getLTBookData(seconds);
            var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };
            var avgSizes = new AverageSizes()
            {
                averageSize = new Dictionary<string, double>()
            };

            // JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, dictBookDataItem, $"LT dictBookDataItem ({seconds} seconds)",false);
            /// Calc average for each data type

            try
            {
                foreach (var name in seriesOrder)
                {

                    var avgSize = 0d;

                    BookDataItem[] items = dictBookDataItem[name];
                    if (items.Length > 0)
                    {
                        avgSize = items.Average(item => item.Size);
                    }
                    avgSizes.averageSize.Add(name, avgSize);

                }

                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, avgSizes, "LT filled avgSizes");

            }
            catch
            {
                //JsConsole.JsConsole.Confirm(TDAStreamerData.jSRuntime, ex.ToString());

            }
            await Task.CompletedTask;
            return avgSizes;
        }

        private static BookDataItem[] getBookDataItemArray(int seconds, long now, List<BookDataItem> lstItems)
        {

            var lstBookItems = lstItems;
            if (seconds > 0)
                lstBookItems.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-seconds));

            var lstBookItemsData = lstBookItems
                .GroupBy(lstBookItems => lstBookItems.Price)
                .Select(lstBookItems => new BookDataItem()
                {
                    Price = lstBookItems.Key,
                    dateTime = DateTime.Now,
                    time = now,
                    Size = lstBookItems.Sum(item => item.Size),
                }
                ).ToArray();

            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, lstBookItemsData, "lstBookItemsData");

            return lstBookItemsData;
        }
        private static BookDataItem[] getLtBookDataItemArray(List<BookDataItem> lstItems)
        {

            var lstBookItemsData = lstItems
                .GroupBy(lstBookItems => lstBookItems.Price)
                .Select(lstBookItems => new BookDataItem()
                {
                    Price = lstBookItems.Key,
                    dateTime = DateTime.Now,
                    time = (long)DateTime.Now.ToOADate(),
                    Size = lstBookItems.Sum(item => item.Size),
                }
                ).ToArray();

            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, lstBookItemsData, "LT lstBookItemsData");

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
                            time = 0,

                        },
                        new BookDataItem()
                        {
                            dateTime = DateTime.Now,
                            Price = 0,
                            Size = 0,
                            time = 0,
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

            //lstAllBids.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));
            //lstAllAsks.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));

            lstSalesAtAsk.Clear();
            lstSalesAtBid.Clear();

            //lstAllSalesAtBid.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));
            //lstAllSalesAtAsk.RemoveAll(t => t.dateTime < DateTime.Now.AddSeconds(-600));

            /// Grab all raw bids
            /// Cosolidate into three bid groups
            /// do same for asks
            /// Add bids then asks to display set
            /// 
            var n = bids.Count();

            if (n == 0) return;
            //now = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
            long now = (long)((Newtonsoft.Json.Linq.JValue)bids.Root["1"]).Value;

            var baseBidPrice = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)bids[0]["0"]).Value);
            for (int i = 0; i < n; i++)
            {
                var price = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)bids[i]["0"]).Value);
                var size = Convert.ToDouble(((Newtonsoft.Json.Linq.JValue)bids[i]["1"]).Value);

                /// Only points within 30 cents of the spread 
                if (Math.Abs(price - baseBidPrice) < 0.30m)
                {

                    var bid = new BookDataItem() { Price = price, Size = size, time = now, dateTime = DateTime.Now };

                    /// Collect the bid
                    lstBids.Add(bid);
                    TDAStreamerData.lstAllBids.Add(bid);
                    TDAStreamerData.lstALLBids.Add(bid);
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

                    /// Collect the ask
                    lstAsks.Add(ask);
                    TDAStreamerData.lstAllAsks.Add(ask);
                    TDAStreamerData.lstALLAsks.Add(ask);
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
                TDAStreamerData.lstAllSalesAtBid.AddRange(lstSalesAtBid);
                TDAStreamerData.lstALLSalesAtBid.AddRange(lstSalesAtBid);

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
                TDAStreamerData.lstAllSalesAtAsk.AddRange(lstSalesAtAsk);
                TDAStreamerData.lstALLSalesAtAsk.AddRange(lstSalesAtAsk);


                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.lstAllAsks, $"lstAllAsks", false);

                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.lstAllBids, $"lstAllBids", false);

                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.lstAllSalesAtAsk, $"lstAllSalesAtAsk", false);

                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.lstAllSalesAtBid, $"lstAllSalesAtBid", false);

                if (!TDAStreamerData.simulatorSettings.isSimulated)
                {
                    Dictionary<string, BookDataItem[]> it = getBookData();


                    string json = JsonSerializer.Serialize<Dictionary<string, BookDataItem[]>>(it);
                    await FilesManager.SendToMessageQueue("BookedTimeSales", DateTime.Now, json);

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


                    json = JsonSerializer.Serialize<BookEntry>(newBookEntry);
                    await FilesManager.SendToMessageQueue("NasdaqBook", DateTime.Now, json);
                }
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



        ///// <summary>
        ///// Produces and maintains a json file that the chart  polls once a second for changes
        ///// </summary>
        ///// <param name="bookDataItems"></param>
        ///// <param name="seriesOrder"></param>
        ///// <param name="categories"></param>
        ///// <param name="seriesList"></param>
        //List<Surface.Series1> lstNewSeries;

        //private void Chart_BuildSeriesData(Dictionary<string, BookDataItem[]> bookDataItems, string[] seriesOrder, string[] categories, List<Surface.Series1> seriesList)
        //{



        //    ////jsruntime.Confirm("Chart_BuildSeriesData: 1");
        //    lstNewSeries = new List<Surface.Series1>();
        //    /// Remove the two empty series at start // will be at end now
        //    if (seriesList.Count > 1)
        //    {
        //        seriesList.RemoveAt(0);
        //        seriesList.RemoveAt(0);
        //    }

        //    ////jsruntime.Confirm("Chart_BuildSeriesData: 2");

        //    /// Prepare empty series to posit new values into 
        //    var seriesItem = new Surface.Series1()   /// Chart Series1 item
        //    {
        //        // This array needs to be the 100 slots and Size put in slot for Price
        //        data = new Surface.Datum?[lstPrices.Count()],
        //        showInLegend = false
        //    };

        //    ////jsruntime.Confirm("Chart_BuildSeriesData: 3");

        //    /// Populate the new series
        //    if (!SurfaceChartConfigurator.isTimeSalesOnly)
        //    {
        //        for (int i = 0; i < bookDataItems.Count; i++)
        //        {
        //            /// Create the series one series item for all 4 book types
        //            var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]


        //            /// Fill out the series data (needs to be 2d)
        //            /// 
        //            /// 
        //            if (seriesName.Length == 4) /// Bids and Asks the Book
        //                Series_AddItems(bookDataItems, seriesItem, seriesName);

        //            /// Add a 
        //            //item.Size;
        //            /// Add to chart Series1 as first series
        //            /// Set chart Series1
        //        }

        //        /// Add the series to the list
        //        /// 
        //        lstNewSeries.Add(seriesItem);
        //        seriesList.Insert(0, seriesItem);
        //    }
        //    ////jsruntime.Confirm("Chart_BuildSeriesData: 4");

        //    seriesItem = new Surface.Series1()   /// Chart Series1 item
        //    {
        //        // This array needs to be the 100 slots and Size put in slot for Price
        //        data = new Surface.Datum?[lstPrices.Count()],
        //        showInLegend = false
        //    };
        //    ////jsruntime.Confirm("Chart_BuildSeriesData: 5");

        //    for (int i = 0; i < bookDataItems.Count; i++)
        //    {
        //        /// Create the series one series item for all 4 book types
        //        var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]

        //        /// Fill out the series data (needs to be 2d)
        //        /// 
        //        /// 
        //        if (seriesName.Length > 4)  /// Time & Sales
        //            Series_AddItems(bookDataItems, seriesItem, seriesName);

        //        /// Add a 
        //        //item.Size;
        //        /// Add to chart Series1 as first series
        //        /// Set chart Series1
        //    }
        //    ////jsruntime.Confirm("Chart_BuildSeriesData: 6");

        //    lstNewSeries.Add(seriesItem);
        //    seriesList.Insert(0, seriesItem);
        //    ////jsruntime.Confirm("Chart_BuildSeriesData: 7");

        //    seriesItem.selected = true;
        //    seriesItem = new Surface.Series1()   /// Chart Series1 item
        //    {
        //        // This array needs to be the 100 slots and Size put in slot for Price
        //        data = new Surface.Datum?[lstPrices.Count()],
        //        showInLegend = false
        //    };
        //    seriesList.Insert(0, seriesItem);
        //    seriesList.Insert(0, new Surface.Series1());
        //    ////jsruntime.Confirm("Chart_BuildSeriesData: 8");



        //    /// TODO: /// 1. Don't remove any series data, just control how much passed to 
        //    //if (seriesList.Count() > chart.zAxis.max - 2)
        //    //{
        //    //    for (var i = 0; i < 2; i++)
        //    //        seriesList.Remove(seriesList.Last());
        //    //}


        //    //if (seriesList.Count() == chart.zAxis.max - 2 || seriesList.Count() == 10 || seriesList.Count() == 11)
        //    //    SurfaceChartConfigurator.redrawChart = true;
        //    //else if (seriesList.Count() % 10 == 0 || seriesList.Count() % 11 == 0 && seriesList.Count() < chart.zAxis.max - 2)
        //    //    SurfaceChartConfigurator.redrawChart = !SurfaceChartConfigurator.redrawChart;


        //    //jsruntime.Confirm("Chart_BuildSeriesData: 9");

        //    if (SurfaceChartConfigurator.isTimeSalesOnly)
        //    {
        //        /// Hide all non-sales series
        //        //foreach(var series in seriesList)
        //        //{
        //        //    series.
        //        //}
        //    }




        //}

        //private static void Series_AddItems(Dictionary<string, BookDataItem[]> bookDataItems, Surface.Series1 seriesItem, string seriesName)
        //{
        //    foreach (var item in bookDataItems[seriesName])    /// item is one BookDataItem
        //    {

        //        /// Place bookitem Sizes in Series1 data
        //        var index = lstPrices.IndexOf(item.Price.ToString("n2"));

        //        //seriesItem.data[]

        //        var data = new Surface.Datum()
        //        {
        //            x = index,
        //            y = SurfaceChartConfigurator.isFlat ? 0 : item.Size,
        //            z = (int?)Math.Min(Math.Floor((double)seriesList.Count / 2), 100),
        //            color = dictSeriesColor[seriesName],
        //        };

        //        seriesItem.data[index] = data;
        //    }
        //}

    }
}

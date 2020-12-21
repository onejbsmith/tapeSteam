#undef tracing
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
using tapeStream.Shared.Managers;
using MathNet.Numerics;
using static tapeStream.Shared.Data.TDAChart;

using JSconsoleExtensionsLib;



namespace tapeStream.Server.Data
{
    /// <summary>
    /// Need to measure the propensity of the market at the moment 
    /// looking at the magnitude, range and frequency of buys above the spread and sells below the spread
    /// </summary>
    public class TDABookManager

    {

        public static List<BookEntry> lstALLBookEntry = new List<BookEntry>();

        public static List<BookDataItem> lstAsks = new List<BookDataItem>();
        public static List<BookDataItem> lstBids = new List<BookDataItem>();
        public static List<BookDataItem> lstSalesAtAsk = new List<BookDataItem>();
        public static List<BookDataItem> lstSalesAtBid = new List<BookDataItem>();

        public static List<Dictionary<string, BookDataItem[]>> lstALLBookedTimeSales =
            new List<Dictionary<string, BookDataItem[]>>();

        public static Dictionary<DateTime, AverageSizes> dictAverageSizes = new Dictionary<DateTime, AverageSizes>();
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
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);


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
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            long now = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;

            DateTime timeNow = DateTime.Now;
            if (TDAStreamerData.simulatorSettings.isSimulated != null && (bool)TDAStreamerData.simulatorSettings.isSimulated)
            {
                timeNow = TDAChart.svcDateTime;
            }

            var asksData = TDAStreamerData.lstALLAsks.ToArray();

            var bidsData = TDAStreamerData.lstALLBids.ToArray();

            var salesAtAskData = TDAStreamerData.lstALLSalesAtAsk.ToArray();

            var salesAtBidData = TDAStreamerData.lstALLSalesAtBid.ToArray();

            if (seconds > 0)
            {
                asksData = TDAStreamerData.lstALLAsks.Where(t => t.dateTime >= timeNow.AddSeconds(-seconds)).ToArray();
                bidsData = TDAStreamerData.lstALLBids.Where(t => t.dateTime >= timeNow.AddSeconds(-seconds)).ToArray();
                salesAtAskData = TDAStreamerData.lstALLSalesAtAsk.Where(t => t.dateTime >= timeNow.AddSeconds(-seconds)).ToArray();
                salesAtBidData = TDAStreamerData.lstALLSalesAtBid.Where(t => t.dateTime >= timeNow.AddSeconds(-seconds)).ToArray();
            }

            var it = new Dictionary<string, BookDataItem[]>()
            { { "asks", asksData.ToArray() }, { "bids", bidsData } , { "salesAtAsk",salesAtAskData}, {"salesAtBid", salesAtBidData}  };


            return it;
        }


        public static async Task<AverageSizes> getAverages(int seconds)
        {

            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
#if tracing
                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, avgSizes, "filled avgSizes");
#endif
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

            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
#if tracing
                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, avgSizes, "LT filled avgSizes");
#endif
            }
            catch
            {
                //JsConsole.JsConsole.Confirm(TDAStreamerData.jSRuntime, ex.ToString());

            }

            if (seconds > 0)
                dictAverageSizes.Add(DateTime.Now, avgSizes);

            await Task.CompletedTask;
            return avgSizes;
        }


        public static async Task<AverageSizes> getLtRatios(int seconds)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            /// Get the book data for the number of seconds
            Dictionary<string, BookDataItem[]> dictBookDataItem = getLTBookData(seconds);

            var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };
            var ratioSizes = new AverageSizes()
            {
                averageSize = new Dictionary<string, double>()
            };

            // JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, dictBookDataItem, $"LT dictBookDataItem ({seconds} seconds)",false);
            /// Calc average for each data type
            try
            {

                var buysRatio = 100 * dictBookDataItem["salesAtAsk"].Sum(t => t.Size) / dictBookDataItem["asks"].Sum(t => t.Size);
                var sellsRatio = 100 * dictBookDataItem["salesAtBid"].Sum(t => t.Size) / dictBookDataItem["bids"].Sum(t => t.Size);

                var buysAltRatio = 100 * dictBookDataItem["salesAtAsk"].Sum(t => t.Size) / dictBookDataItem["bids"].Sum(t => t.Size);
                var sellsAltRatio = 100 * dictBookDataItem["salesAtBid"].Sum(t => t.Size) / dictBookDataItem["asks"].Sum(t => t.Size);

                var highBidPrice = dictBookDataItem["bids"].LastOrDefault().Price;
                var lowAskPrice = dictBookDataItem["asks"].LastOrDefault().Price;

                var bids = dictBookDataItem["bids"].Sum(t => t.Size);
                var asks = dictBookDataItem["asks"].Sum(t => t.Size);

                var buysInSpread = dictBookDataItem["salesAtAsk"].Where(r => r.Price > highBidPrice && r.Price < lowAskPrice).Sum(t => t.Size);
                var buysAbove = dictBookDataItem["salesAtAsk"].Where(r => r.Price >= lowAskPrice).Sum(t => t.Size);

                var sellsInSpread = dictBookDataItem["salesAtBid"].Where(r => r.Price > highBidPrice && r.Price < lowAskPrice).Sum(t => t.Size);
                var sellsBelow = dictBookDataItem["salesAtBid"].Where(r => r.Price < highBidPrice).Sum(t => t.Size);

                var sellsAbove = dictBookDataItem["salesAtBid"].Where(r => r.Price > lowAskPrice).Sum(t => t.Size);
                var buysBelow = dictBookDataItem["salesAtAsk"].Where(r => r.Price < highBidPrice).Sum(t => t.Size);

                var buysPriceCount = dictBookDataItem["salesAtAsk"].GroupBy(t => t.Price).Count() + dictBookDataItem["asks"].GroupBy(t => t.Price).Count();
                var buysTradeSizes = dictBookDataItem["salesAtAsk"].Sum(t => t.Size);

                var sellsPriceCount = dictBookDataItem["salesAtBid"].GroupBy(t => t.Price).Count() + dictBookDataItem["bids"].GroupBy(t => t.Price).Count();
                var sellsTradeSizes = dictBookDataItem["salesAtBid"].Sum(t => t.Size);
                /// Trades outside of spread added to other side
                /// 

                var sellsSumSizes = dictBookDataItem["salesAtBid"].Sum(t => t.Size) + dictBookDataItem["asks"].GroupBy(t => t.Price).Count();
                var buysSumSizes = dictBookDataItem["salesAtAsk"].Sum(t => t.Size) + dictBookDataItem["bids"].GroupBy(t => t.Price).Count();

                buysRatio = (buysInSpread + sellsBelow) / asks;
                sellsRatio = (sellsInSpread + buysAbove) / bids;

                ratioSizes.averageSize.Add("buys", buysRatio);
                ratioSizes.averageSize.Add("sells", sellsRatio);


                try
                {
                    var ratioFrame = new RatioFrame()
                    {
                        dateTime = dictBookDataItem["bids"].LastOrDefault().time.FromUnixTime().ToLocalTime(),
                        buysRatio = buysRatio,
                        sellsRatio = sellsRatio,
                        markPrice = (highBidPrice + lowAskPrice) / 2,
                        seconds = seconds,
                        bidsBookSizes = bids,
                        sellsInSpread = sellsInSpread,
                        sellsBelow = sellsBelow,
                        sellsAbove = sellsAbove,
                        asksBookSizes = asks,
                        buysInSpread = buysInSpread,
                        buysAbove = buysAbove,
                        buysBelow = buysBelow,
                        buysTradeSizes = buysTradeSizes,
                        buysPriceCount = buysPriceCount,
                        sellsTradeSizes = sellsTradeSizes,
                        sellsPriceCount = sellsPriceCount,
                        buysAltRatio = buysAltRatio,
                        sellsAltRatio = sellsAltRatio,
                        buysSumSizes = buysSumSizes,
                        sellsSumSizes = sellsSumSizes

                    };
                    TDABook.lstRatioFrames.Add(ratioFrame);

                    //// Calc correlation coeffiecients
                    //var _ratioFrames = TDABook.lstRatioFrames;
                    //var n = _ratioFrames.Count;
                    //var sumY = (double)_ratioFrames.Sum(t => t.markPrice);
                    //var sumXBuys = _ratioFrames.Sum(t => t.buysRatio);
                    //var sumXSells = _ratioFrames.Sum(t => t.sellsRatio);
                    //var sumXYBuys = _ratioFrames.Sum(t => t.buysRatio * (double)t.markPrice);
                    //var sumXYSells = _ratioFrames.Sum(t => t.sellsRatio * (double)t.markPrice);
                    //var sumX2Buys = _ratioFrames.Sum(t => t.buysRatio * t.buysRatio);
                    //var sumX2Sells = _ratioFrames.Sum(t => t.sellsRatio * t.sellsRatio);
                    //var sumY2 = (double)_ratioFrames.Sum(t => t.markPrice * t.markPrice);

                    //var rBuys = (n * sumXYBuys - sumXBuys * sumY) / (Math.Sqrt((n * sumX2Buys - sumXBuys * sumXBuys) * (n * sumY2 - sumY * sumY)));
                    //var rSells = (n * sumXYSells - sumXSells * sumY) / (Math.Sqrt((n * sumX2Sells - sumXSells * sumXSells) * (n * sumY2 - sumY * sumY)));


                    //TDABook.lstRatioFrames.Last().sellsR = rSells;
                    //TDABook.lstRatioFrames.Last().buysR = rBuys;
                }
                catch (Exception ex)
                {
                    //JsConsole.JsConsole.Confirm(TDAStreamerData.jSRuntime, ex.ToString());
                }
                //JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDABook.lstRatioFrames, "TDABook.lstRatioFrames");

            }
            catch { }

            await Task.CompletedTask;
            return ratioSizes;
        }

        public static async Task<RatioFrame[]> getIncrementalRatioFrames(int seconds)
        {
            /// Get the book data for the number of seconds
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            Dictionary<string, BookDataItem[]> dictBookDataItem = getLTBookData(seconds);

            RatioFrame ratioFrame = new RatioFrame();
            RatioFrame coefficientFrame = new RatioFrame();
            RatioFrame regressionFrame = new RatioFrame();
            RatioFrame regressionFrame2 = new RatioFrame();

            var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };
            var ratioSizes = new AverageSizes()
            {
                averageSize = new Dictionary<string, double>()
            };

            // JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, dictBookDataItem, $"LT dictBookDataItem ({seconds} seconds)",false);
            /// Calc average for each data type
            try
            {
                var bids = dictBookDataItem["bids"].Sum(t => t.Size);
                var asks = dictBookDataItem["asks"].Sum(t => t.Size);

                var highBidPrice = dictBookDataItem["bids"].LastOrDefault()?.Price;
                var lowAskPrice = dictBookDataItem["asks"].LastOrDefault()?.Price;
                var markPrice = ((decimal)highBidPrice + (decimal)lowAskPrice) / 2;

                var buysTradeSizes = dictBookDataItem["salesAtAsk"].Sum(t => t.Size);
                var sellsTradeSizes = dictBookDataItem["salesAtBid"].Sum(t => t.Size);

                var buysRatio = 100 * buysTradeSizes / asks;
                var sellsRatio = 100 * sellsTradeSizes / bids;

                var buysAltRatio = 100 * buysTradeSizes / bids;
                var sellsAltRatio = 100 * sellsTradeSizes / asks;


                var buysInSpread = dictBookDataItem["salesAtAsk"].Where(r => r.Price > highBidPrice && r.Price < lowAskPrice).Sum(t => t.Size);
                var buysAbove = dictBookDataItem["salesAtAsk"].Where(r => r.Price >= lowAskPrice).Sum(t => t.Size);

                var sellsInSpread = dictBookDataItem["salesAtBid"].Where(r => r.Price > highBidPrice && r.Price < lowAskPrice).Sum(t => t.Size);
                var sellsBelow = dictBookDataItem["salesAtBid"].Where(r => r.Price < highBidPrice).Sum(t => t.Size);

                var sellsAbove = dictBookDataItem["salesAtBid"].Where(r => r.Price > lowAskPrice).Sum(t => t.Size);
                var buysBelow = dictBookDataItem["salesAtAsk"].Where(r => r.Price < highBidPrice).Sum(t => t.Size);

                var sellsSummedAboveBelowLong = SumField("sellsBelow", seconds, TDABook.ratiosDepth) + SumField("sellsAbove", seconds, TDABook.ratiosDepth);
                var sellsSummedAboveBelowMed = SumField("sellsBelow", seconds, TDABook.ratiosBack) + SumField("sellsAbove", seconds, TDABook.ratiosBack);
                var sellsSummedAboveBelowShort = SumField("sellsBelow", seconds, TDABook.ratiosShort) + SumField("sellsAbove", seconds, 30);

                var buysSummedAboveBelowLong = SumField("buysBelow", seconds, TDABook.ratiosDepth) + SumField("buysAbove", seconds, TDABook.ratiosDepth);
                var buysSummedAboveBelowMed = SumField("buysBelow", seconds, TDABook.ratiosBack) + SumField("buysAbove", seconds, TDABook.ratiosBack);
                var buysSummedAboveBelowShort = SumField("buysBelow", seconds, TDABook.ratiosShort) + SumField("buysAbove", seconds, 30);

                var sellsSummedInSpreadLong = SumField("sellsInSpread", seconds, TDABook.ratiosDepth);
                var sellsSummedInSpreadMed = SumField("sellsInSpread", seconds, TDABook.ratiosBack);
                var sellsSummedInSpreadShort = SumField("sellsInSpread", seconds, TDABook.ratiosShort);

                var buysSummedInSpreadLong = SumField("buysInSpread", seconds, TDABook.ratiosDepth);
                var buysSummedInSpreadMed = SumField("buysInSpread", seconds, TDABook.ratiosBack);
                var buysSummedInSpreadShort = SumField("buysInSpread", seconds, TDABook.ratiosShort);

                var buysPriceCount = dictBookDataItem["salesAtAsk"].GroupBy(t => t.Price).Count() + dictBookDataItem["asks"].GroupBy(t => t.Price).Count();

                var sellsPriceCount = dictBookDataItem["salesAtBid"].GroupBy(t => t.Price).Count() + dictBookDataItem["bids"].GroupBy(t => t.Price).Count();

                /// Trades outside of spread added to other side
                /// 

                var sellsSumSizes = dictBookDataItem["salesAtBid"].Sum(t => t.Size) + dictBookDataItem["asks"].GroupBy(t => t.Price).Count();
                var buysSumSizes = dictBookDataItem["salesAtAsk"].Sum(t => t.Size) + dictBookDataItem["bids"].GroupBy(t => t.Price).Count();


                sellsRatio = (buysInSpread + sellsBelow) / asks;
                buysRatio = (sellsInSpread + buysAbove) / bids;

                ratioSizes.averageSize.Add("buys", buysRatio);
                ratioSizes.averageSize.Add("sells", sellsRatio);

                var bollingers =
                await TDAChartManager.GetBollingerBands();

                var it = dictBookDataItem["bids"].LastOrDefault();
                if (it != null)
                    try
                    {
                        var now = it.time.FromUnixTime().ToLocalTime();


                        ratioFrame = new RatioFrame()
                        {
                            seconds = seconds,
                            dateTime = now,

                            /// Time & Sales
                            markPrice = markPrice,
                            buysTradeSizes = buysTradeSizes,
                            sellsTradeSizes = sellsTradeSizes,

                            /// Level II
                            bidsBookSizes = bids,
                            asksBookSizes = asks,

                            /// Stat based on Mark Price
                            bollingerHigh = bollingers.high,
                            bollingerLow = bollingers.low,
                            bollingerMid = bollingers.mid,

                            /// Level II + Time & Sales
                            buysSumSizes = buysSumSizes,
                            sellsSumSizes = sellsSumSizes,

                            /// Level II Range
                            buysPriceCount = buysPriceCount,
                            sellsPriceCount = sellsPriceCount,

                            /// T & S / Level II
                            buysRatio = buysRatio,
                            sellsRatio = sellsRatio,
                            buysAltRatio = buysAltRatio,
                            sellsAltRatio = sellsAltRatio,

                            /// T & S Inside Spread
                            buysInSpread = buysInSpread,
                            sellsInSpread = sellsInSpread,

                            /// T & S Outside Spread
                            buysAbove = buysAbove,
                            buysBelow = buysBelow,
                            sellsAbove = sellsAbove,
                            sellsBelow = sellsBelow,

                            /// Cumulative T & S 
                            buysSummedInSpreadLong = buysSummedInSpreadLong,
                            buysSummedInSpreadMed = buysSummedInSpreadMed,
                            buysSummedInSpreadShort = buysSummedInSpreadShort,
                            buysSummedAboveBelowLong = buysSummedAboveBelowLong,
                            buysSummedAboveBelowMed = buysSummedAboveBelowMed,
                            buysSummedAboveBelowShort = buysSummedAboveBelowShort,

                            sellsSummedInSpreadLong = sellsSummedInSpreadLong,
                            sellsSummedInSpreadMed = sellsSummedInSpreadMed,
                            sellsSummedInSpreadShort = sellsSummedInSpreadShort,
                            sellsSummedAboveBelowLong = sellsSummedAboveBelowLong,
                            sellsSummedAboveBelowMed = sellsSummedAboveBelowMed,
                            sellsSummedAboveBelowShort = sellsSummedAboveBelowShort

                        };
                        TDABook.lstRatioFrames.Add(ratioFrame);


                        coefficientFrame = new RatioFrame()
                        {
                            seconds = seconds,
                            dateTime = now,

                            /// Time & Sales
                            markPrice = (decimal)CalcCorrelationCoeffiecients("markPrice", seconds),
                            buysTradeSizes = CalcCorrelationCoeffiecients("buysTradeSizes", seconds),
                            sellsTradeSizes = CalcCorrelationCoeffiecients("sellsTradeSizes", seconds),

                            /// Level II
                            bidsBookSizes = CalcCorrelationCoeffiecients("bidsBookSizes", seconds),
                            asksBookSizes = CalcCorrelationCoeffiecients("asksBookSizes", seconds),

                            /// Stat based on Mark Price
                            bollingerHigh = CalcCorrelationCoeffiecients("bollingerHigh", seconds),
                            bollingerLow = CalcCorrelationCoeffiecients("bollingerLow", seconds),
                            bollingerMid = CalcCorrelationCoeffiecients("bollingerMid", seconds),

                            /// Level II + Time & Sales
                            buysSumSizes = CalcCorrelationCoeffiecients("buysSumSizes", seconds),
                            sellsSumSizes = CalcCorrelationCoeffiecients("sellsSumSizes", seconds),

                            /// Level II Range
                            buysPriceCount = (int)CalcCorrelationCoeffiecients("buysPriceCount", seconds),
                            sellsPriceCount = (int)CalcCorrelationCoeffiecients("sellsPriceCount", seconds),

                            /// T & S / Level II
                            buysRatio = CalcCorrelationCoeffiecients("buysRatio", seconds),
                            sellsRatio = CalcCorrelationCoeffiecients("sellsRatio", seconds),
                            buysAltRatio = CalcCorrelationCoeffiecients("buysAltRatio", seconds),
                            sellsAltRatio = CalcCorrelationCoeffiecients("sellsAltRatio", seconds),

                            /// T & S Inside Spread
                            buysInSpread = CalcCorrelationCoeffiecients("buysInSpread", seconds),
                            sellsInSpread = CalcCorrelationCoeffiecients("sellsInSpread", seconds),

                            /// T & S Outside Spread
                            buysAbove = CalcCorrelationCoeffiecients("buysAbove", seconds),
                            buysBelow = CalcCorrelationCoeffiecients("buysBelow", seconds),
                            sellsAbove = CalcCorrelationCoeffiecients("sellsAbove", seconds),
                            sellsBelow = CalcCorrelationCoeffiecients("sellsBelow", seconds),

                            /// Cumulative T & S 
                            buysSummedInSpreadLong = CalcCorrelationCoeffiecients("buysSummedInSpreadLong", seconds),
                            buysSummedInSpreadMed = CalcCorrelationCoeffiecients("buysSummedInSpreadMed", seconds),
                            buysSummedInSpreadShort = CalcCorrelationCoeffiecients("buysSummedInSpreadShort", seconds),
                            buysSummedAboveBelowLong = CalcCorrelationCoeffiecients("buysSummedAboveBelowLong", seconds),
                            buysSummedAboveBelowMed = CalcCorrelationCoeffiecients("buysSummedAboveBelowMed", seconds),
                            buysSummedAboveBelowShort = CalcCorrelationCoeffiecients("buysSummedAboveBelowShort", seconds),

                            sellsSummedInSpreadLong = CalcCorrelationCoeffiecients("sellsSummedInSpreadLong", seconds),
                            sellsSummedInSpreadMed = CalcCorrelationCoeffiecients("sellsSummedInSpreadMed", seconds),
                            sellsSummedInSpreadShort = CalcCorrelationCoeffiecients("sellsSummedInSpreadShort", seconds),
                            sellsSummedAboveBelowLong = CalcCorrelationCoeffiecients("sellsSummedAboveBelowLong", seconds),
                            sellsSummedAboveBelowMed = CalcCorrelationCoeffiecients("sellsSummedAboveBelowMed", seconds),
                            sellsSummedAboveBelowShort = CalcCorrelationCoeffiecients("sellsSummedAboveBelowShort", seconds)
                        };

                        //var secondsBack = TDABook.ratiosDepth;
                        //coefficientFrame = new RatioFrame()
                        //{
                        //    dateTime = now,
                        //    //buysRatio = SumField("buysRatio", seconds, secondsBack),
                        //    //sellsRatio = SumField("sellsRatio", seconds, secondsBack),
                        //    //bidsBookSizes = SumField("bidsBookSizes", seconds, secondsBack),
                        //    //sellsInSpread = SumField("sellsInSpread", seconds, secondsBack),
                        //    sellsBelow = SumField("sellsBelow", seconds, secondsBack),
                        //    sellsAbove = SumField("sellsAbove", seconds, secondsBack),
                        //    //asksBookSizes = SumField("asksBookSizes", seconds, secondsBack),
                        //    //buysInSpread = SumField("buysInSpread", seconds, secondsBack),
                        //    buysAbove = SumField("buysAbove", seconds, secondsBack),
                        //    buysBelow = SumField("buysBelow", seconds, secondsBack),
                        //    //buysTradeSizes = SumField("buysTradeSizes", seconds, secondsBack),
                        //    //buysPriceCount = (int)SumField("buysPriceCount", seconds, secondsBack),
                        //    //sellsTradeSizes = SumField("sellsTradeSizes", seconds, secondsBack),
                        //    //sellsPriceCount = (int)SumField("sellsPriceCount", seconds, secondsBack),
                        //    //buysAltRatio = SumField("buysAltRatio", seconds, secondsBack),
                        //    //sellsAltRatio = SumField("sellsAltRatio", seconds, secondsBack),
                        //    //buysSumSizes = SumField("buysSumSizes", seconds, secondsBack),
                        //    //sellsSumSizes = SumField("sellsSumSizes", seconds, secondsBack)

                        //};


                        regressionFrame = new RatioFrame()
                        {
                            dateTime = now,
                            //buysRatio = CalcPolynomialRegression("buysRatio", seconds),
                            //sellsRatio = CalcPolynomialRegression("sellsRatio", seconds),
                            //bidsBookSizes = CalcPolynomialRegression("bidsBookSizes", seconds),
                            //sellsInSpread = CalcPolynomialRegression("sellsInSpread", seconds),
                            //sellsBelow = CalcPolynomialRegression("sellsBelow", seconds),
                            //sellsAbove = CalcPolynomialRegression("sellsAbove", seconds),
                            //asksBookSizes = CalcPolynomialRegression("asksBookSizes", seconds),
                            //buysInSpread = CalcPolynomialRegression("buysInSpread", seconds),
                            //buysAbove = CalcPolynomialRegression("buysAbove", seconds),
                            //buysBelow = CalcPolynomialRegression("buysBelow", seconds),
                            //buysTradeSizes = CalcPolynomialRegression("buysTradeSizes", seconds),
                            //buysPriceCount = (int)CalcPolynomialRegression("buysPriceCount", seconds),
                            //sellsTradeSizes = CalcPolynomialRegression("sellsTradeSizes", seconds),
                            //sellsPriceCount = (int)CalcPolynomialRegression("sellsPriceCount", seconds),
                            //buysAltRatio = CalcPolynomialRegression("buysAltRatio", seconds),
                            //sellsAltRatio = CalcPolynomialRegression("sellsAltRatio", seconds),
                            //buysSumSizes = CalcPolynomialRegression("buysSumSizes", seconds),
                            //sellsSumSizes = CalcPolynomialRegression("sellsSumSizes", seconds)
                        };

                        //secondsBack = TDABook.ratiosBack;
                        //regressionFrame = new RatioFrame()
                        //{
                        //    dateTime = now,
                        //    //buysRatio = SumField("buysRatio", seconds, secondsBack),
                        //    //sellsRatio = SumField("sellsRatio", seconds, secondsBack),
                        //    //bidsBookSizes = SumField("bidsBookSizes", seconds, secondsBack),
                        //    //sellsInSpread = SumField("sellsInSpread", seconds, secondsBack),
                        //    sellsBelow = SumField("sellsBelow", seconds, secondsBack),
                        //    sellsAbove = SumField("sellsAbove", seconds, secondsBack),
                        //    //asksBookSizes = SumField("asksBookSizes", seconds, secondsBack),
                        //    //buysInSpread = SumField("buysInSpread", seconds, secondsBack),
                        //    buysAbove = SumField("buysAbove", seconds, secondsBack),
                        //    buysBelow = SumField("buysBelow", seconds, secondsBack),
                        //    //buysTradeSizes = SumField("buysTradeSizes", seconds, secondsBack),
                        //    //buysPriceCount = (int)SumField("buysPriceCount", seconds, secondsBack),
                        //    //sellsTradeSizes = SumField("sellsTradeSizes", seconds, secondsBack),
                        //    //sellsPriceCount = (int)SumField("sellsPriceCount", seconds, secondsBack),
                        //    //buysAltRatio = SumField("buysAltRatio", seconds, secondsBack),
                        //    //sellsAltRatio = SumField("sellsAltRatio", seconds, secondsBack),
                        //    //buysSumSizes = SumField("buysSumSizes", seconds, secondsBack),
                        //    //sellsSumSizes = SumField("sellsSumSizes", seconds, secondsBack)
                        //};

                        //secondsBack = 30;
                        //regressionFrame2 = new RatioFrame()
                        //{
                        //    dateTime = now,
                        //    //buysRatio = SumField("buysRatio", seconds, secondsBack),
                        //    //sellsRatio = SumField("sellsRatio", seconds, secondsBack),
                        //    //bidsBookSizes = SumField("bidsBookSizes", seconds, secondsBack),
                        //    //sellsInSpread = SumField("sellsInSpread", seconds, secondsBack),
                        //    sellsBelow = SumField("sellsBelow", seconds, secondsBack),
                        //    sellsAbove = SumField("sellsAbove", seconds, secondsBack),
                        //    //asksBookSizes = SumField("asksBookSizes", seconds, secondsBack),
                        //    //buysInSpread = SumField("buysInSpread", seconds, secondsBack),
                        //    buysAbove = SumField("buysAbove", seconds, secondsBack),
                        //    buysBelow = SumField("buysBelow", seconds, secondsBack),
                        //    //buysTradeSizes = SumField("buysTradeSizes", seconds, secondsBack),
                        //    //buysPriceCount = (int)SumField("buysPriceCount", seconds, secondsBack),
                        //    //sellsTradeSizes = SumField("sellsTradeSizes", seconds, secondsBack),
                        //    //sellsPriceCount = (int)SumField("sellsPriceCount", seconds, secondsBack),
                        //    //buysAltRatio = SumField("buysAltRatio", seconds, secondsBack),
                        //    //sellsAltRatio = SumField("sellsAltRatio", seconds, secondsBack),
                        //    //buysSumSizes = SumField("buysSumSizes", seconds, secondsBack),
                        //    //sellsSumSizes = SumField("sellsSumSizes", seconds, secondsBack)
                        //};

                        //regressionFrame = new RatioFrame()
                        //{
                        //    dateTime = now,
                        //    buysRatio = CalcDiffCorrelationCoeffiecients("buysRatio", seconds),
                        //    asksBookSizes = CalcDiffCorrelationCoeffiecients("asksBookSizes", seconds),
                        //    buysInSpread = CalcDiffCorrelationCoeffiecients("buysInSpread", seconds),
                        //    buysAbove = CalcDiffCorrelationCoeffiecients("buysAbove", seconds),
                        //    buysBelow = CalcDiffCorrelationCoeffiecients("buysBelow", seconds),
                        //    buysTradeSizes = CalcDiffCorrelationCoeffiecients("buysTradeSizes", seconds),
                        //    buysPriceCount = (int)CalcDiffCorrelationCoeffiecients("buysPriceCount", seconds),
                        //    buysSumSizes = CalcDiffCorrelationCoeffiecients("buysSumSizes", seconds),
                        //    buysAltRatio = CalcDiffCorrelationCoeffiecients("buysAltRatio", seconds),

                        //    bidsBookSizes = SumDiffs("bidsBookSizes", seconds),
                        //    sellsRatio = SumDiffs("sellsRatio", seconds),
                        //    sellsInSpread = SumDiffs("sellsInSpread", seconds),
                        //    sellsBelow = SumDiffs("sellsBelow", seconds),
                        //    sellsAbove = SumDiffs("sellsAbove", seconds),
                        //    sellsTradeSizes = SumDiffs("sellsTradeSizes", seconds),
                        //    sellsPriceCount = (int)SumDiffs("sellsPriceCount", seconds),
                        //    sellsAltRatio = SumDiffs("sellsAltRatio", seconds),
                        //    sellsSumSizes = SumDiffs("sellsSumSizes", seconds)
                        //};
                        //// Calc correlation coeffiecients



                    }
                    catch (Exception ex)
                    {
                        TDAStreamerData.jSRuntime.confirm(ex.ToString());
                    }
                //JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDABook.lstRatioFrames, "TDABook.lstRatioFrames");

            }
            catch { }

            await Task.CompletedTask;
            return new RatioFrame[] { ratioFrame, coefficientFrame };
        }

        private static double CalcPolynomialRegression(string v, int seconds)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            return TDABook.CalcPolynomialRegression(v, seconds);
        }

        private static double SumDiffs(string sellsField, int seconds)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            var buysField = sellsField.Replace("bids", "asks").Replace("sells", "buys");
            var _ratioFrames = TDABook.lstRatioFrames.Where(frame => frame.seconds == seconds && TDAChart.svcDateTime.Subtract(frame.dateTime).TotalSeconds <= seconds).ToList();
            var sumXBuys = _ratioFrames.Sum(t => Convert.ToDouble(t[buysField]) - Convert.ToDouble(t[sellsField]));

            return sumXBuys;
        }

        private static double SumField(string fieldName, int seconds, int secondsBack)
        {
            var _ratioFrames = TDABook.lstRatioFrames.Where(frame => frame.seconds == seconds && TDAChart.svcDateTime.Subtract(frame.dateTime).TotalSeconds <= secondsBack).ToList();
            var sum = _ratioFrames.Sum(t => Convert.ToDouble(t[fieldName]));
            return sum;
        }

        /// <summary>
        /// Calc correlation coeffiecients
        /// </summary>
        /// <param name="buysField"></param>
        /// <returns></returns>
        public static double CalcCorrelationCoeffiecients(string buysField, int seconds, int lead = 0)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            var rBuys = 0d;
            /// get frames in the last n seconds 
            try
            {
                var _ratioFrames = TDABook.lstRatioFrames.Where(frame => frame.seconds == seconds && TDAChart.svcDateTime.Subtract(frame.dateTime).TotalSeconds <= seconds).ToList();
                var n = _ratioFrames.Count;

                if (n > 1)
                {
                    var sumY = (double)_ratioFrames.Sum(t => t.markPrice);
                    var sumXBuys = _ratioFrames.Sum(t => Convert.ToDouble(t[buysField]));
                    //var sumXSells = _ratioFrames.Sum(t => Convert.ToDouble(t[sellsField]));
                    var sumXYBuys = _ratioFrames.Sum(t => Convert.ToDouble(t[buysField]) * (double)t.markPrice);
                    //var sumXYSells = _ratioFrames.Sum(t => Convert.ToDouble(t[sellsField]) * (double)t.markPrice);
                    var sumX2Buys = _ratioFrames.Sum(t => Convert.ToDouble(t[buysField]) * Convert.ToDouble(t[buysField]));
                    //var sumX2Sells = _ratioFrames.Sum(t => Convert.ToDouble(t[sellsField]) * Convert.ToDouble(t[sellsField]));
                    var sumY2 = (double)_ratioFrames.Sum(t => t.markPrice * t.markPrice);

                    rBuys = (n * sumXYBuys - sumXBuys * sumY) / (Math.Sqrt((n * sumX2Buys - sumXBuys * sumXBuys) * (n * sumY2 - sumY * sumY)));
                    //var rSells = (n * sumXYSells - sumXSells * sumY) / (Math.Sqrt((n * sumX2Sells - sumXSells * sumXSells) * (n * sumY2 - sumY * sumY)));
                    if (Double.IsNaN(rBuys)) rBuys = 0;
                }

            }
            catch
            {
                TDAStreamerData.jSRuntime.error(buysField);
            }
            return 100 * rBuys;

        }
        public static double CalcDiffCorrelationCoeffiecients(string buysField, int seconds, int lead = 0)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            /// get frames in the last n seconds 

            var sellsField = buysField.Replace("asks", "bids").Replace("buys", "sells");
            var _ratioFrames = TDABook.lstRatioFrames.Where(frame => frame.seconds == seconds && TDAChart.svcDateTime.Subtract(frame.dateTime).TotalSeconds <= seconds).ToList();
            var n = _ratioFrames.Count;

            var sumY = (double)_ratioFrames.Sum(t => t.markPrice);
            var sumXBuys = _ratioFrames.Sum(t => Convert.ToDouble(t[buysField]) - Convert.ToDouble(t[sellsField]));
            //var sumXSells = _ratioFrames.Sum(t => Convert.ToDouble(t[sellsField]));
            var sumXYBuys = _ratioFrames.Sum(t => (Convert.ToDouble(t[buysField]) - Convert.ToDouble(t[sellsField])) * (double)t.markPrice);
            //var sumXYSells = _ratioFrames.Sum(t => Convert.ToDouble(t[sellsField]) * (double)t.markPrice);
            var sumX2Buys = _ratioFrames.Sum(t => (Convert.ToDouble(t[buysField]) - Convert.ToDouble(t[sellsField])) * (Convert.ToDouble(t[buysField]) - Convert.ToDouble(t[sellsField])));
            //var sumX2Sells = _ratioFrames.Sum(t => Convert.ToDouble(t[sellsField]) * Convert.ToDouble(t[sellsField]));
            var sumY2 = (double)_ratioFrames.Sum(t => t.markPrice * t.markPrice);

            var rBuys = (n * sumXYBuys - sumXBuys * sumY) / (Math.Sqrt((n * sumX2Buys - sumXBuys * sumXBuys) * (n * sumY2 - sumY * sumY)));
            //var rSells = (n * sumXYSells - sumXSells * sumY) / (Math.Sqrt((n * sumX2Sells - sumXSells * sumXSells) * (n * sumY2 - sumY * sumY)));
            if (Double.IsNaN(rBuys)) rBuys = 0;
            return 100 * rBuys;
        }
        public static async Task<List<RatioFrame>> getListLtRatios(int seconds, int last)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            var takeLast = last == 0 ? TDABook.lstRatioFrames.Count : last;
            await Task.CompletedTask;
            return TDABook.lstRatioFrames.Where(frame => frame.seconds == seconds).TakeLast(takeLast).ToList();
        }

        public static async Task<List<RatioFrame>> getRatioFrames(int seconds, int last)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            var takeLast = last == 0 ? TDABook.lstRatioFrames.Count : last;

            var data = TDABook.lstRatioFrames.Where(frame => frame.seconds == seconds).TakeLast(takeLast).ToList();
            FilesManager.WriteNewCsvFile($"wwwroot/files/ratioFrames{seconds}.txt", data.Select(rec => new { dateTime = rec.dateTime, rec.buysRatio, rec.markPrice, rec.sellsRatio }).ToList());

            await Task.CompletedTask;
            return data;
        }

        public static async Task<string> getRatioFramesCSV(int seconds, int last)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            var takeLast = last == 0 ? TDABook.lstRatioFrames.Count : last;
            var data = TDABook.lstRatioFrames.Where(frame => frame.seconds == seconds).TakeLast(takeLast).ToList();

            var text = CsvFilesManager.GetNewCsvString(data.Select(rec => new { rec.dateTime, rec.buysRatio, rec.sellsRatio }).ToList());

            await Task.CompletedTask;
            return text;
        }

        internal static async Task<string> getAllRatioFrames(string symbol, DateTime svcDateTime)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            var text = await FilesManager.GetMessageQueueFiles(symbol, "AllRatioFrames", svcDateTime);
            return "[" + text + "]";
        }

        private static BookDataItem[] getBookDataItemArray(int seconds, long now, List<BookDataItem> lstItems)
        {

            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
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

#if tracing
            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, lstBookItemsData, "lstBookItemsData");
#endif
            return lstBookItemsData;
        }
        private static BookDataItem[] getLtBookDataItemArray(List<BookDataItem> lstItems)
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);

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

#if tracing
            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, lstBookItemsData, "LT lstBookItemsData");
#endif
            return lstBookItemsData;
        }
        public static async Task<Dictionary<string, BookDataItem[]>> getBookPiesData()
        {
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
            TDAStreamerData.jSRuntime.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
            /// TODO √ DB: Add to Streamed table & get streamId
            if (TDAStreamerData.isNotSimulated())
            {
                var dateTime = now.FromUnixTime().ToLocalTime();
                if (!DatabaseManager.dictRunIds.ContainsKey(symbol))
                    DatabaseManager.Runs_Add(symbol, dateTime);

                DatabaseManager.Streamed_Add(symbol, dateTime);
            }

            var baseBidPrice = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)bids[0]["0"]).Value);
            for (int i = 0; i < n; i++)
            {
                var price = Convert.ToDecimal(((Newtonsoft.Json.Linq.JValue)bids[i]["0"]).Value);
                var size = Convert.ToDouble(((Newtonsoft.Json.Linq.JValue)bids[i]["1"]).Value);

                /// Only points within 30 cents of the spread 
                /// Chnage this to a setting BookPriceMaxDiff 
                if (Math.Abs(price - baseBidPrice) < 0.30m)
                {

                    var bid = new BookDataItem() { Price = price, Size = size, time = now, dateTime = now.FromUnixTime().ToLocalTime() };

                    /// Collect the bid
                    /// TODO √ DB: Add to Bids table
                    lstBids.Add(bid);
                    if (TDAStreamerData.isNotSimulated())
                    {
                        DatabaseManager.Bids_Add(symbol, bid);
                    }
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
                    var ask = new BookDataItem() { Price = price, Size = size, time = now, dateTime = now.FromUnixTime().ToLocalTime() };

                    /// Collect the ask
                    /// TODO √ DB: Add to Asks table
                    lstAsks.Add(ask);
                    if (TDAStreamerData.isNotSimulated())
                    {
                        DatabaseManager.Asks_Add(symbol, ask);
                    }
                    TDAStreamerData.lstAllAsks.Add(ask);
                    TDAStreamerData.lstALLAsks.Add(ask);
                    //lstSalesAtAsk(sales)
                    //sumAskSize += size;
                }
            }

            try
            {

                var bidEntry = lstBids.First();
                //var maxBid = lstBids.Max(bids => bids.Price);

                //var minAsk = lstAsks.Min(asks => asks.Price);
                var askEntry = lstAsks.First();

                /// TODO DB: Add to Marks Table
                /// 

                if (TDAStreamerData.isNotSimulated())
                {
                    var mark = new BookDataItem()
                    {
                        Price = (bidEntry.Price + askEntry.Price) / 2,
                        Size = bidEntry.Size - askEntry.Size,
                        dateTime = bidEntry.dateTime,
                        time = bidEntry.time
                    };
                    DatabaseManager.Marks_Add(symbol, mark);
                }
                var lastTime = 0d;
                var lastSale = TDAStreamerData.timeSales[symbol].Last();

                try
                {
                    /// This sb the last book entry before the last time and sales
                    if (lastSale != null)
                    {
                        var bookEntries = lstALLBookEntry.Where(be => be.time < lastSale.time);
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

                TDAStreamerData.jSRuntime.groupCollapsed("TDAStreamerData.timeSales[symbol]");
                TDAStreamerData.jSRuntime.table(TDAStreamerData.timeSales[symbol]);
                TDAStreamerData.jSRuntime.groupEnd();

                TDAStreamerData.jSRuntime.groupCollapsed("prints");
                TDAStreamerData.jSRuntime.table(prints);
                TDAStreamerData.jSRuntime.log($"lastTime: {lastTime} | bidEntry.time: {bidEntry.time}");
                TDAStreamerData.jSRuntime.groupEnd();

                var printsByPriceAtLevel = prints.GroupBy(sale => new { sale.price, sale.level, sale.TimeDate })
                .Select(sales => new PrintSale()
                {
                    price = sales.Key.price,
                    level = sales.Key.level,
                    size = sales.Sum(sale => sale.size)
                });

                TDAStreamerData.jSRuntime.groupCollapsed("printsByPriceAtLevel");
                TDAStreamerData.jSRuntime.table(printsByPriceAtLevel);
                TDAStreamerData.jSRuntime.groupEnd();

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
                    dateTime = TDAChart.svcDateTime,
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
                    dateTime = TDAChart.svcDateTime,
                    Size = sales.Sum(sale => sale.size)
                }).ToList();
                TDAStreamerData.lstAllSalesAtAsk.AddRange(lstSalesAtAsk);
                TDAStreamerData.lstALLSalesAtAsk.AddRange(lstSalesAtAsk);

#if tracing
                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.lstAllAsks, $"lstAllAsks", false);

                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.lstAllBids, $"lstAllBids", false);

                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.lstAllSalesAtAsk, $"lstAllSalesAtAsk", false);

                JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.lstAllSalesAtBid, $"lstAllSalesAtBid", false);
#endif
                Dictionary<string, BookDataItem[]> dictBook = getBookData();

                lstALLBookedTimeSales.Add(dictBook);

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
                lstALLBookEntry.Add(newBookEntry);

                if (!TDAStreamerData.simulatorSettings.isSimulated != null && (bool)TDAStreamerData.simulatorSettings.isSimulated)
                {
                    string json = JsonSerializer.Serialize<Dictionary<string, BookDataItem[]>>(dictBook);
                    await FilesManager.SendToMessageQueue(symbol, "BookedTimeSales", DateTime.Now, json);

                    /// We need the sales by Price for the chart
                    /// Need two series, salesAtBid, salesAtAsk
                    ///
                    json = JsonSerializer.Serialize<BookEntry>(newBookEntry);
                    await FilesManager.SendToMessageQueue(symbol, "NasdaqBook", DateTime.Now, json);
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

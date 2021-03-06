﻿#undef tracing
#undef tracingFine
#undef bollinger
using Blazorise.Utils;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Client.Managers;
using tapeStream.Shared;
using tapeStream.Shared.Data;

namespace tapeStream.Client.Components.HighCharts
{
    public partial class StackedColumns3DSurface
    {
        [Inject]
        public Microsoft.JSInterop.IJSRuntime jsruntime { get; set; }

        [Inject]
        public Blazored.LocalStorage.ISyncLocalStorageService localStorage { get; set; }



        private string[] _categories;

        public string[] categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                /// So we keep state of chart current in case we need to redraw whole
                /// Make chart model observable then call js when prop change detected?
                //Surface3dManager.chart.xAxis.categories = _categories;
                //jsruntime.SetCategories(_categories);
            }
        }




        string id = "Surface3D";



        [Parameter]
        public string symbol { get; set; }

        [Parameter]
        public Dictionary<string, BookDataItem[]> bookData
        {
            get { return _bookData; }
            set
            {
                _bookData = value;
#if tracing
                Console.WriteLine("3. Surface ChartSetData");
#endif

                ChartSetData(value);



            }
        }
        Dictionary<string, BookDataItem[]> _bookData = new Dictionary<string, BookDataItem[]>();
        public string chartJsFilename { get; set; } = "js/StackedColumns3DSurfaceChart.js?id=" + DateTime.Now.ToOADate().ToString();

        static string chart3Djson = "";


        public int seconds { get; set; }

        string name = "not set";


        static Dictionary<string, string> dictSeriesColor;

        static TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        static List<Surface.Series1> seriesList = new List<Surface.Series1>();

        private string _chartSeriesJson;

        private DateTime _clockDateTime = DateTime.Now;

        public string chartSeriesJson
        {
            get { return _chartSeriesJson; }
            set
            {
                _chartSeriesJson = value;

            }
        }


        MatChip[] selectedChips = null;
        MatChip selectedChip = null;

        protected override async Task OnInitializedAsync()
        {
            dictSeriesColor = SetSeriesColors();
            var dotNetReference = DotNetObjectReference.Create(this);
            await jsruntime.InvokeVoidAsync("Initialize", dotNetReference, id);
            //SurfaceChartConfigurator.seconds = 3;
            if (SurfaceChartConfigurator.redrawChart == true) SurfaceChartConfigurator.redrawChart = false;
            await Task.CompletedTask;
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //await jsruntime.InvokeVoidAsync("onBlazorReady");
                await jsruntime.InvokeVoidAsync("renderjQueryComponents");
            }
            //ExtensionMethodsDemo.Console.WriteBlueLine("");

            await base.OnAfterRenderAsync(firstRender);
        }

        void ResetXAxis()
        {
            SurfaceChartConfigurator.resetXAxis = true;
        }

        [JSInvokable("getChartJson")]
        public async Task getSurfaceChartJson(string jsonResponse)
        {
            /// get the chart as a POCO 
#if tracing
            Console.WriteLine("1. Surface getChartJson");
            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
#endif

            Surface3dManager.Initialize(jsonResponse);
            chart3Djson = JsonSerializer.Serialize<Surface.StackedColumns3DSurface>(Surface3dManager.chart);

            Surface3dManager.InitializePrices(localStorage, id);



#if tracing            
            Console.WriteLine("2. Surface getChartJson");
            Console.WriteLine(chart3Djson);
#endif


            await Task.Yield();
        }


        List<Surface.Series1> lstNewSeries;

        private async Task ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        {

            //await jsruntime.GroupTableAsync(bookDataItems, "bookDataItems");

            try

            {
#if tracing
                await jsruntime.InvokeVoidAsync("Dump", bookDataItems.Dumps(), "bookDataItems");
                await jsruntime.InvokeVoidAsync("Dump", Surface3dManager.chart.Dumps(), "chart");
                Console.WriteLine("4. Surface   ChartSetData");
#endif
                //Surface3dManager.chart.title.text = "Surface";
                //Surface3dManager.chart.plotOptions.column.depth = 1;

                Surface3dManager.PreSetProperties();

#if tracing
                Console.WriteLine("5. Surface ChartSetData");
#endif
                Surface3dManager.MaintainPriceAxis(bookDataItems, localStorage, id);
                /// Get the price range min and max over all items in the dictionary

#if tracing
                Console.WriteLine("6. Surface ChartSetData");

#endif
                Chart_AdjustYaxis(bookDataItems);

                var categories = Surface3dManager.lstPrices.ToArray();
                //Surface3dManager.lstPrices.Where(it => Convert.ToDouble(it) > minX - 0.05 && Convert.ToDouble(it) < maxX + 0.05).ToArray();


                //Surface3dManager.chart.xAxis.max = categories.Count();
                Surface3dManager.chart.xAxis.categories = categories;

                //Surface3dManager.chart.title.text = SurfaceChartConfigurator.showTitle;
                Surface3dManager.chart.title.text = TDAChart.LongDateString;


                //TDAChart.svcDateTime.ToLongDateString() + " " + TDAChart.svcDateTime.ToLongTimeString();
#if tracing                
                await jsruntime.GroupTableAsync(Surface3dManager.chart.xAxis.categories, "Surface3dManager.chart.xAxis.categories");
                await jsruntime.GroupTableAsync(Surface3dManager.chart.subtitle.text, "Surface3dManager.chart.subtitle.text");

                ;

                /// 

                Console.WriteLine("7. Surface ChartSetData");
#endif
                Chart_AddCandlePlotBands(bookDataItems, TDAChart.lastCandle, categories);

                //Chart_AddSpreadPlotBand(bookDataItems, categories);


                Chart_AddBollingerPlotLines(bookDataItems, categories);
                /// Convert BookDataItem[] to Series1[]
                Chart_BuildSeriesData(bookDataItems, categories, seriesList);

                /// TODO: /// 2. Control which items in series get passed to chart
                /// TODO: /// 3. Better just send new series to chart and let chart roll off stale data, it has series in memory.
                /// TODO: /// 4. Should only have to redraw if resize event, all else should just update specific chart property
                /// TODO: /// 5.  Just direct jsRuntime calls for each property
                /// TODO: /// 6. Use jsRuntime extension methods so can name the calls and strong type parameters
                Surface3dManager.chart.series = seriesList.Take(46).ToArray();

#if tracing
                await jsruntime.GroupTableAsync(chart, "chart");
#endif 

                //Surface3dManager.chart.Surface3dManager.chart.options3d.depth = Math.Max(100, Surface3dManager.chart.series.Count() * Surface3dManager.chart.plotOptions.column.depth);

                /// Send the new data and settings to the HighChart3D component


                /// Update the clock (current time only at this point)
                _clockDateTime = TDAChart.svcDateTime;

                var wholeChartJson = JsonSerializer.Serialize<Surface.StackedColumns3DSurface>(Surface3dManager.chart);
                chart3Djson = wholeChartJson;
                //if (seriesList.Count < 10)
                //    chart3Djson = wholeChartJson;
                //else
                //{
                //    /// Instead, send just the data

                //    var lstArray = JsonSerializer.Serialize<Surface.Series1[]>(lstNewSeries.ToArray());
                //    jsruntime.GroupTable(lstArray, "lstArray");
                //    chartSeriesJson = lstArray;
                //}
#if tracing

                Console.WriteLine("8. Surface ChartSetData chart3Djson");
                //Console.WriteLine(chart3Djson);

#endif                    ///
                /// We should only send the series and the categories, not the whole chart
                /// But minor amouunt of config data involved in redraw
                /// Shuld only send series i case of a rolling series, 
                /// instead of sending the whole matrix of data, just send the new row.              /// chartSeriesJson = System.Text.Json.JsonSerializer.Serialize<Series1[]>(Surface3dManager.chart.series);


                //var chart3DseriesJson = JsonSerializer.Serialize<Series1[]>(Surface3dManager.chart.series); ;
                //var chart3DxAxisJson = JsonSerializer.Serialize<Xaxis>(Surface3dManager.chart.xAxis);

                StateHasChanged();

                //await jsruntime.InvokeVoidAsync("setChart3Dseries", new object[] { chart3DseriesJson, chart3DxAxisJson });
                await Task.CompletedTask;

            }
            catch
            {

            }
        }



        //static Dictionary<string, double> dictAvgSizes = new Dictionary<string, double>();
        //static Dictionary<string, double> dictSumSizes = new Dictionary<string, double>();
        //static Dictionary<string, int> dictNumsizes = new Dictionary<string, int>();

        //static Dictionary<string, double> dictStAvgSizes = new Dictionary<string, double>();
        //static Dictionary<string, double> dictStSumSizes = new Dictionary<string, double>();
        //static Dictionary<string, int> dictStNumsizes = new Dictionary<string, int>();

        private void Chart_AdjustYaxis(Dictionary<string, BookDataItem[]> bookDataItems)
        {
            try
            {
                Surface3dManager.chart.yAxis.max = SurfaceChartConfigurator.yAxisHigh;
                if (SurfaceChartConfigurator.yAxisMaxAutoReset == false) return;

                //var highestSize = bookDataItems.Max(dict => dict.Value.Max(it => it.Size));
                var highestSize = Surface3dManager.maxSize;
                if (highestSize > Surface3dManager.chart.yAxis.max)
                {
                    Surface3dManager.chart.yAxis.max = (int)Math.Ceiling(highestSize / 5000) * 5000;
                    SurfaceChartConfigurator.yAxisHigh = (int)Surface3dManager.chart.yAxis.max;
                }
            }
            catch (Exception ex)
            { }
        }

        private static void Chart_AddSpreadPlotBand(Dictionary<string, BookDataItem[]> bookDataItems, string[] categories)
        {

            var highBidPrice = bookDataItems["bids"][0].Price;
            var lowAskPrice = bookDataItems["asks"][0].Price;
            var highBid = categories.ToList().IndexOf(highBidPrice.ToString("n2"));
            var lowAsk = categories.ToList().IndexOf(lowAskPrice.ToString("n2"));

            Surface3dManager.chart.xAxis.plotBands = new Surface.Plotband[]
            {
                    new Surface.Plotband()
                    { from=highBid,
                        to =lowAsk,
                        color="#888888",
                        label= new Label()
                        {
                            text = ((highBidPrice+lowAskPrice)/2).ToString("n2")
                        }

                    }
            };
        }

        private static void Chart_AddCandlePlotBands(Dictionary<string, BookDataItem[]> bookDataItems, TDAChart.Chart_Content chartEntry, string[] categories)
        {
            var open = categories.ToList().IndexOf(chartEntry.open.ToString("n2"));
            var close = categories.ToList().IndexOf(chartEntry.close.ToString("n2"));
            var low = categories.ToList().IndexOf(chartEntry.low.ToString("n2"));
            var high = categories.ToList().IndexOf(chartEntry.high.ToString("n2"));


            var highBidPrice = bookDataItems["bids"][0].Price;
            var lowAskPrice = bookDataItems["asks"][0].Price;
            var highBid = categories.ToList().IndexOf(highBidPrice.ToString("n2"));
            var lowAsk = categories.ToList().IndexOf(lowAskPrice.ToString("n2"));

            var mark = ((highBidPrice + lowAskPrice) / 2).ToString("n2");
            //Surface3dManager.chart.xAxis.plotBands = new Surface.Plotband[]
            //{

            //};

            /// Candle body
            if (chartEntry.open < chartEntry.close) // green bar
                Surface3dManager.chart.xAxis.plotBands = new Surface.Plotband[]
                {
                    new Surface.Plotband()
                    {
                        from=(decimal)close,
                        to =(decimal)open,
                        color="#3CDD71"
                    },
                    new Surface.Plotband()
                    {
                        from=(decimal)low,
                        to =(decimal)open,
                        color="MEDIUMSEAGREEN"
                    },
                    new Surface.Plotband()
                    {
                        from=(decimal)close,
                        to =(decimal)high,
                        color="MEDIUMSEAGREEN"
                    },

                    new Surface.Plotband()
                    { from=highBid,
                        to =lowAsk,
                        color="#0",
                        label= new Label()
                        {
                            text = $"{mark}",
                            y = -10
                        }

                    }
                };
            else

            {
                Surface3dManager.chart.xAxis.plotBands = new Surface.Plotband[]
                {
                    new Surface.Plotband()
                    {
                        from=(decimal)open,
                        to =(decimal)close,
                        color="#99143C"
                    },
                    new Surface.Plotband()
                    {
                        from=(decimal)low,
                        to =(decimal)close,
                        color="#995C5C"
                    },
                    new Surface.Plotband()
                    {
                        from=(decimal)open,
                        to =(decimal)high,
                        color="#995C5C"
                    },
                    new Surface.Plotband()
                    { from=highBid,
                        to =lowAsk,
                        color="#0",
                        label= new Label()
                        {
                            text = $"{mark}",
                        },


                    }
                };
            }

            //Surface3dManager.chart.Surface3dManager.chart.annotations[0].labels[0].text = mark.ToString();
            //Surface3dManager.chart.Surface3dManager.chart.annotations[0].labels[0].x = highBid;
            //Surface3dManager.chart.Surface3dManager.chart.annotations[0].labels[0].y = 0;


        }

        private void Chart_AddBollingerPlotLines(Dictionary<string, BookDataItem[]> bookDataItems, string[] categories)
        {
            var bollingerBands = TDAChart.bollingerBands;
            //var bollingerBands = bookDataItems["asks"][0].bollingerBand;

            var lowPrice = bollingerBands.low.ToString("n2");
            var midlowPrice = bollingerBands.midlow.ToString("n2");
            var midPrice = bollingerBands.mid.ToString("n2");
            var midhighPrice = bollingerBands.midhigh.ToString("n2");
            var highPrice = bollingerBands.high.ToString("n2");




#if bollinger
            Console.WriteLine("7a. Columns Chart_AddBollingerPlotLines");
            Console.WriteLine("7a. Columns bollingerBands.mid=" + midPrice);

#endif
            //var lowAsk = categories.ToList().IndexOf(bookDataItems["asks"][0].Price.ToString("n2"));

            var low = categories.ToList().IndexOf(lowPrice); ;
            var midlow = categories.ToList().IndexOf(midlowPrice); ;
            var mid = categories.ToList().IndexOf(midPrice); ;
            var midhigh = categories.ToList().IndexOf(midhighPrice);
            var high = categories.ToList().IndexOf(highPrice);
#if bollinger
            Console.WriteLine("7a1. Columns Chart_AddBollingerPlotLines");

#endif
            var avgPriceIndex = categories.ToList().IndexOf(Surface3dManager.avgPrice.ToString("n2"));
            var midCategory = categories.Length / 2;

            //if (!dictAvgSizes.ContainsKey("salesAtBid"))
            //{ dictAvgSizes.Add("salesAtBid", 0); dictStAvgSizes.Add("salesAtBid", 0); }
            //if (!dictAvgSizes.ContainsKey("salesAtAsk"))
            //{ dictAvgSizes.Add("salesAtAsk", 0); dictStAvgSizes.Add("salesAtAsk", 0); }

#if bollinger
            Console.WriteLine("7a1. Columns Chart_AddBollingerPlotLines");
            jsruntime.GroupTable(dictAvgSizes, nameof(dictAvgSizes));

#endif
            var n = 3;
            var avgSells = (decimal)(TDAChart.avgSizes.averageSize["asks"] + TDAChart.avgSizes.averageSize["salesAtBid"]); ;
            var avgBuys = (decimal)(TDAChart.avgSizes.averageSize["bids"] + TDAChart.avgSizes.averageSize["salesAtAsk"]);
            var avgStSells = (decimal)(TDAChart.avgStSizes.averageSize["asks"] + TDAChart.avgStSizes.averageSize["salesAtBid"]);
            var avgStBuys = (decimal)(TDAChart.avgStSizes.averageSize["bids"] + TDAChart.avgStSizes.averageSize["salesAtAsk"]);
            var avgLtSells = n * (decimal)(TDAChart.avgLtSizes.averageSize["asks"] + TDAChart.avgLtSizes.averageSize["salesAtBid"]); ;
            var avgLtBuys = n * (decimal)(TDAChart.avgLtSizes.averageSize["bids"] + TDAChart.avgLtSizes.averageSize["salesAtAsk"]);

            TDAChart.avgBuys = avgBuys;
            TDAChart.avgSells = avgSells;
            TDAChart.avgStBuys = avgStBuys;
            TDAChart.avgStSells = avgStSells;
            TDAChart.avgLtBuys = avgLtBuys / n;
            TDAChart.avgLtSells = avgLtSells / n;

            TDAChart.countBuysUp += avgBuys > avgSells ? 1 : 0;
            TDAChart.countSellRatioUp += avgSells > avgBuys ? 1 : 0;


#if bollinger
            Console.WriteLine("7b. Columns Chart_AddBollingerPlotLines");

#endif
            try
            {
                Surface3dManager.chart.yAxis.plotLines = new Surface.Plotline[]
                    {
                    new Surface.Plotline()
                    {  value=avgSells, // asks want to sell, sold at bid
                        color=CONSTANTS.asksColor,
                        width=8,
                        zIndex = 1
                    },
                    new Surface.Plotline()
                    {  value= avgBuys,  // bids want to buy, bought at ask
                        color=CONSTANTS.bidsColor,
                        width=8,
                        zIndex = 1
                    },
                    new Surface.Plotline()
                    {  value=avgStSells,
                        color =CONSTANTS.sellsColor,
                        width=4,
                        zIndex = 2
                    },
                    new Surface.Plotline()
                    {  value=avgStBuys,
                        color=CONSTANTS.buysColor,
                        width=4,
                        zIndex = 2
                    },
                      new Surface.Plotline()
                    {  value=avgLtSells,
                        color =CONSTANTS.asksLtColor,
                        width=12,
                        zIndex =0
                    },
                    new Surface.Plotline()
                    {  value=avgLtBuys,
                        color=CONSTANTS.bidsLtColor,
                        width=12,
                        zIndex = 0
                    },
                  };
            }
            catch (Exception ex)
            {
                jsruntime.Confirm(ex.ToString());
            }

#if bollinger
            Console.WriteLine("7c. Columns Chart_AddBollingerPlotLines");

#endif
            //if (avgSells < avgBuys)
            //    Surface3dManager.chart.Surface3dManager.chart.options3d.beta = -SurfaceChartConfigurator.beta;
            //else
            //    Surface3dManager.chart.Surface3dManager.chart.options3d.beta = SurfaceChartConfigurator.beta;

#if bollinger
            Console.WriteLine("7d. Columns Chart_AddBollingerPlotLines");

#endif
            Surface3dManager.chart.xAxis.plotLines = new Surface.Plotline[]
            {
                new Surface.Plotline()
                {  value=low,
                    color="magenta",
                    width=4,
                    zIndex = 2
                },
                 new Surface.Plotline()
                {  value=midlow,
                    color="magenta",
                    width=2,
                    zIndex = 2
                },
                new Surface.Plotline()
                {  value=mid,
                    color="cyan",
                    width=4,
                    zIndex = 2
                },
                new Surface.Plotline()
                {  value=midhigh,
                    color="red",
                    width=2,
                    zIndex = 2
                },
                new Surface.Plotline()
                {  value=high,
                    color="red",
                    width=4,
                    zIndex = 2
                },
                new Surface.Plotline()
                {  value=midCategory,
                    color="#666666",
                    width=8,
                    zIndex = 1
                },
                new Surface.Plotline()
                {  value=avgPriceIndex,
                    color="green",
                    width=8,
                    zIndex = 1
                },
            };

#if bollinger
            Console.WriteLine("7e. Columns Chart_AddBollingerPlotLines");

#endif
        }


        private void Chart_BuildSeriesData(Dictionary<string, BookDataItem[]> bookDataItems, string[] categories, List<Surface.Series1> seriesList)
        {

            var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };

            ///// Remove any over 600*4 series (10 minutes)
            ///// 
            //var minutesToKeep = 10;
            //var seriesToKeep = minutesToKeep * seriesOrder.Length * 60;

            //seriesToKeep = (int)Surface3dManager.chart.zAxis.max - 1;
            // seriesList = seriesList.Take(seriesToKeep).ToList();

            ///// Remove previous 4 series from the legend
            ///// They will be the first four series
            /////             
            //foreach (var series in seriesList.Take(seriesOrder.Length))
            //{
            //    series.showInLegend = false;
            //}

            ////jsruntime.Confirm("Chart_BuildSeriesData: 1");
            lstNewSeries = new List<Surface.Series1>();
            /// Remove the two empty series at start // will be at end now
            if (seriesList.Count > 1)
            {
                seriesList.RemoveAt(0);
                seriesList.RemoveAt(0);
            }

            ////jsruntime.Confirm("Chart_BuildSeriesData: 2");

            /// Prepare empty series to posit new values into 
            var seriesItem = new Surface.Series1()   /// Chart Series1 item
            {
                // This array needs to be the 100 slots and Size put in slot for Price
                data = new Surface.Datum?[Surface3dManager.lstPrices.Count()],
                showInLegend = false
            };

            ////jsruntime.Confirm("Chart_BuildSeriesData: 3");

            /// Populate the new series
            if (!SurfaceChartConfigurator.isTimeSalesOnly)
            {
                for (int i = 0; i < bookDataItems.Count; i++)
                {
                    /// Create the series one series item for all 4 book types
                    var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]


                    /// Fill out the series data (needs to be 2d)
                    /// 
                    /// 
                    if (seriesName.Length > 4) /// Bids and Asks the Book
                        Series_AddItems(bookDataItems, seriesItem, seriesName);

                    /// Add a 
                    //item.Size;
                    /// Add to chart Series1 as first series
                    /// Set chart Series1
                }

                /// Add the series to the list
                /// 
                lstNewSeries.Add(seriesItem);
                seriesList.Insert(0, seriesItem);
            }
            ////jsruntime.Confirm("Chart_BuildSeriesData: 4");

            seriesItem = new Surface.Series1()   /// Chart Series1 item
            {
                // This array needs to be the 100 slots and Size put in slot for Price
                data = new Surface.Datum?[Surface3dManager.lstPrices.Count()],
                showInLegend = false
            };
            ////jsruntime.Confirm("Chart_BuildSeriesData: 5");

            for (int i = 0; i < bookDataItems.Count; i++)
            {
                /// Create the series one series item for all 4 book types
                var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]

                /// Fill out the series data (needs to be 2d)
                /// 
                /// 
                if (seriesName.Length == 4)  /// Time & Sales
                    Series_AddItems(bookDataItems, seriesItem, seriesName);

                /// Add a 
                //item.Size;
                /// Add to chart Series1 as first series
                /// Set chart Series1
            }
            ////jsruntime.Confirm("Chart_BuildSeriesData: 6");

            lstNewSeries.Add(seriesItem);
            seriesList.Insert(0, seriesItem);
            ////jsruntime.Confirm("Chart_BuildSeriesData: 7");

            seriesItem.selected = true;
            seriesItem = new Surface.Series1()   /// Chart Series1 item
            {
                // This array needs to be the 100 slots and Size put in slot for Price
                data = new Surface.Datum?[Surface3dManager.lstPrices.Count()],
                showInLegend = false
            };
            seriesList.Insert(0, seriesItem);
            seriesList.Insert(0, new Surface.Series1());
            ////jsruntime.Confirm("Chart_BuildSeriesData: 8");



            /// TODO: /// 1. Don't remove any series data, just control how much passed to 
            //if (seriesList.Count() > Surface3dManager.chart.zAxis.max - 2)
            //{
            //    for (var i = 0; i < 2; i++)
            //        seriesList.Remove(seriesList.Last());
            //}


            if (seriesList.Count() == Surface3dManager.chart.zAxis.max - 2)
            {
                SurfaceChartConfigurator.redrawChart = true;
                StateHasChanged();
            }
            //else if (seriesList.Count() % 10 == 0 || seriesList.Count() % 11 == 0 && seriesList.Count() < Surface3dManager.chart.zAxis.max - 2)
            //    SurfaceChartConfigurator.redrawChart = !SurfaceChartConfigurator.redrawChart;


            //jsruntime.Confirm("Chart_BuildSeriesData: 9");

            if (SurfaceChartConfigurator.isTimeSalesOnly)
            {
                /// Hide all non-sales series
                //foreach(var series in seriesList)
                //{
                //    series.
                //}
            }




        }

        private static void Series_AddItems(Dictionary<string, BookDataItem[]> bookDataItems, Surface.Series1 seriesItem, string seriesName)
        {
            foreach (var item in bookDataItems[seriesName])    /// item is one BookDataItem
            {

                /// Place bookitem Sizes in Series1 data
                var index = Surface3dManager.lstPrices.IndexOf(item.Price.ToString("n2"));

                //seriesItem.data[]

                var data = new Surface.Datum()
                {
                    x = index,
                    y = SurfaceChartConfigurator.isFlat ? 0 : item.Size,
                    z = (int?)Math.Min(Math.Floor((double)seriesList.Count / 2), 100),
                    color = dictSeriesColor[seriesName],
                };

                seriesItem.data[index] = data;
            }
        }
        double? minX = 9999999, maxX = 0;


        //static string asksColor = "#8085e9"; //"#cb6992";
        //static string bidsColor = "#0479cc";
        //static string asksLtColor = "#5055b9"; //"#cb6992";
        //static string bidsLtColor = "#0439ac";
        //static string buysColor = "#90ed7d";
        //static string sellsColor = "#f45b5b";
        //static string spreadColor = "#8085e9";


        private static Dictionary<string, string> SetSeriesColors()
        {
            Dictionary<string, string> dictSeriesColor = new Dictionary<string, string>();

            dictSeriesColor = new Dictionary<string, string>()
            {
                { "asks",  CONSTANTS.asksColor } ,
                { "bids", CONSTANTS.bidsColor } ,
                { "salesAtAsk", CONSTANTS.buysColor } ,
                { "salesAtBid", CONSTANTS.sellsColor },
                { "spread", CONSTANTS.spreadColor }
            };
            return dictSeriesColor;
        }



    }

    /// <summary>
    /// Using this class to strong-type calls to jsRuntime for chart property changes
    /// </summary>
    public static class jsRuntimeHighchartExtensions
    {
        public static async Task<string> ConfirmAsync(this Microsoft.JSInterop.IJSRuntime jSRuntime, string message)
        {
            return await jSRuntime.InvokeAsync<string>("Confirm", new object[] { message });
        }

        /// <summary>
        /// Should this be called in the property set of the model?
        /// </summary>
        /// <param name="jSRuntime"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static void SetCategories(this Microsoft.JSInterop.IJSRuntime jSRuntime, string[] categories)
        {
            jSRuntime.InvokeAsync<string>("SetCategories", new object[] { categories });
        }


    }
}

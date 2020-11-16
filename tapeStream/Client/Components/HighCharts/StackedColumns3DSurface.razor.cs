#define tracing
#define bollinger
using Blazorise.Utils;
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
using tapeStream.Shared.Data;

namespace tapeStream.Client.Components.HighCharts
{
    public partial class StackedColumns3DSurface
    {
        string id = "Surface3D";
        static List<string> lstPrices = new List<string>();


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
        public string chartJsFilename { get; set; } = "js/StackedColumns3DSurfaceChart.js";

        static string chart3Djson = "";

        static double maxSize;

        public int seconds { get; set; }

        string name = "not set";

        static Surface.StackedColumns3DSurface chart = new Surface.StackedColumns3DSurface();

        static Dictionary<string, string> dictSeriesColor;

        static TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        static List<Surface.Series1> seriesList = new List<Surface.Series1>();

        string chartSeriesJson = "";

        protected override async Task OnInitializedAsync()
        {
            dictSeriesColor = SetSeriesColors();
            var dotNetReference = DotNetObjectReference.Create(this);
            await jsruntime.InvokeVoidAsync("Initialize", dotNetReference, id);
            //SurfaceChartConfigurator.seconds = 3;

            await Task.CompletedTask;
        }

        [JSInvokable("getChartJson")]
        public async Task getChartJson(string jsonResponse)
        {
            /// get the chart as a POCO 
#if tracing
            Console.WriteLine("1. Surface getChartJson");
#endif

            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
            chart = JsonSerializer.Deserialize<Surface.StackedColumns3DSurface>(jsonResponse);

            /// We set some static chart Properties here and pass back to js
            chart.title.text = "Surface";
            chart.chart.options3d.enabled = true;
            chart.yAxis.title.text = "Size";
            chart.xAxis.title.text = "Price";
            //chart.plotOptions.series.pointWidth = 100;

            chart3Djson = JsonSerializer.Serialize<Surface.StackedColumns3DSurface>(chart);

            lstPrices = localStorage.GetItem<List<string>>("lstPrices").Take(20).ToList();

#if tracing            
            Console.WriteLine("2. Surface getChartJson");
#endif
            Console.WriteLine(chart3Djson);


            await Task.Yield();
        }

        private async Task ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        {

            await jsruntime.InvokeVoidAsync("Dump", bookDataItems.Dumps(), "bookDataItems");

            try

            {
#if tracing
                Console.WriteLine("4. Surface ChartSetData");
#endif
                //chart.title.text = "Surface";
                //chart.plotOptions.column.depth = 1;

                chart.yAxis.max = SurfaceChartConfigurator.yAxisHigh;
                chart.chart.options3d.alpha = SurfaceChartConfigurator.alpha;
                chart.chart.options3d.beta = SurfaceChartConfigurator.beta;

#if tracing
                Console.WriteLine("5. Surface ChartSetData");
#endif
                var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };
                /// Get the price range min and max over all items in the dictionary
                Chart_MaintainPriceAxis(bookDataItems, seriesOrder);                //AddSpreadPointsToBookData(bookDataItems);
#if tracing
                Console.WriteLine("6. Surface ChartSetData");

#endif

                var categories = lstPrices.ToArray();
      


                chart.xAxis.categories = categories;
                chart.xAxis.max = categories.Count();

                await jsruntime.InvokeVoidAsync("Dump", chart.xAxis.categories.Dumps(), "chart.xAxis.categories");

                ;

                /// 
                Chart_AddSpreadPlotBand(bookDataItems, categories);

                Chart_AddBollingerPlotLines(bookDataItems, categories);
                /// Convert BookDataItem[] to Series1[]
                Chart_BuildSeriesData(bookDataItems, seriesOrder, categories, seriesList);
#if tracing
                Console.WriteLine("7. Surface ChartSetData");
#endif
                chart.series = seriesList.ToArray();
                await jsruntime.InvokeVoidAsync("Dump", chart.series.Dumps(), "chart.series");

                //chart.chart.options3d.depth = Math.Max(100, chart.series.Count() * chart.plotOptions.column.depth);

                /// Send the new data and settings to the HighChart3D component

                chart3Djson = JsonSerializer.Serialize<Surface.StackedColumns3DSurface>(chart);
#if tracing

                Console.WriteLine("8. Surface ChartSetData chart3Djson");
                Console.WriteLine(chart3Djson);

#endif                    ///
                /// We should only send the series and the categories, not the whole chart
                /// But minor amouunt of config data involved in redraw
                /// Shuld only send series i case of a rolling series, 
                /// instead of sending the whole matrix of data, just send the new row.              /// chartSeriesJson = System.Text.Json.JsonSerializer.Serialize<Series1[]>(chart.series);


                //var chart3DseriesJson = JsonSerializer.Serialize<Series1[]>(chart.series); ;
                //var chart3DxAxisJson = JsonSerializer.Serialize<Xaxis>(chart.xAxis);

                StateHasChanged();

                //await jsruntime.InvokeVoidAsync("setChart3Dseries", new object[] { chart3DseriesJson, chart3DxAxisJson });
                await Task.CompletedTask;

            }
            catch
            {

            }
        }

       private void Chart_AdjustYaxis(Dictionary<string, BookDataItem[]> bookDataItems)
        {
            try
            {
                chart.yAxis.max = SurfaceChartConfigurator.yAxisHigh;
                if (SurfaceChartConfigurator.yAxisMaxAutoReset == false) return;

                //var highestSize = bookDataItems.Max(dict => dict.Value.Max(it => it.Size));
                var highestSize = maxSize;
                if (highestSize > chart.yAxis.max)
                {
                    chart.yAxis.max = (int)Math.Ceiling(highestSize / 5000) * 5000;
                    SurfaceChartConfigurator.yAxisHigh = (int)chart.yAxis.max;
                }
            }
            catch (Exception ex)
            { }
        }

        private static void Chart_AddSpreadPlotBand(Dictionary<string, BookDataItem[]> bookDataItems, string[] categories)
        {
            var highBid = categories.ToList().IndexOf(bookDataItems["bids"][0].Price.ToString("n2"));
            var lowAsk = categories.ToList().IndexOf(bookDataItems["asks"][0].Price.ToString("n2"));

            chart.xAxis.plotBands = new Surface.Plotband[]
            {
                    new Surface.Plotband()
                    { from=highBid,
                        to =lowAsk,
                        color="#888888",
                        //label= new Label()
                        //{
                        //    text="Spread"
                        //}

                    }
            };
        }

        private static void Chart_AddBollingerPlotLines(Dictionary<string, BookDataItem[]> bookDataItems, string[] categories)
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

            var midCategory = categories.Length / 2;

            chart.xAxis.plotLines = new Surface.Plotline[]
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
            };
        }


        private static void Chart_BuildSeriesData(Dictionary<string, BookDataItem[]> bookDataItems, string[] seriesOrder, string[] categories, List<Surface.Series1> seriesList)
        {


            ///// Remove any over 600*4 series (10 minutes)
            ///// 
            //var minutesToKeep = 10;
            //var seriesToKeep = minutesToKeep * seriesOrder.Length * 60;
            //seriesList = seriesList.Take(seriesToKeep).ToList();

            ///// Remove previous 4 series from the legend
            ///// They will be the first four series
            /////             
            //foreach (var series in seriesList.Take(seriesOrder.Length))
            //{
            //    series.showInLegend = false;
            //}

            var seriesItem = new Surface.Series1()   /// Chart Series1 item
            {
                // This array needs to be the 100 slots and Size put in slot for Price
                data = new Surface.Datum?[lstPrices.Count()],
                showInLegend = false
            };

            for (int i = 0; i < bookDataItems.Count; i++)
            {
                /// Create the series one series item for all 4 book types
                var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]

                /// Fill out the series data (needs to be 2d)
                foreach (var item in bookDataItems[seriesName])    /// item is one BookDataItem
                {

                    /// Place bookitem Sizes in Series1 data
                    var index = lstPrices.IndexOf(item.Price.ToString("n2"));

                    //seriesItem.data[]

                    var data = new Surface.Datum()
                    {
                        x = index,
                        y = item.Size,
                        color = dictSeriesColor[seriesName],
                    };

                    seriesItem.data[index] = data;
                    seriesItem.selected = true;
                }
                //item.Size;
                /// Add to chart Series1 as first series
                /// Set chart Series1
            }
            seriesList.Insert(0, seriesItem);
        }

        private void Chart_MaintainPriceAxis(Dictionary<string, BookDataItem[]> bookDataItems, string[] seriesOrder)
        {
            var minPrice = 999999m; // bookDataItems.Min(items => items.Value.Min(item => item.Price));
            var maxPrice = 0m; // bookDataItems.Max(items => items.Value.Max(item => item.Price));


            /// Set up the Categories list
            //var lstPrices = new List<string>();
            foreach (var name in seriesOrder)
            {
                foreach (var item in bookDataItems[name])
                {
                    if (!lstPrices.Contains(item.Price.ToString("n2")))
                        lstPrices.Add(item.Price.ToString("n2"));

                    minPrice = Math.Min(item.Price, minPrice);
                    maxPrice = Math.Max(item.Price, maxPrice);
                }
            }
            lstPrices.Sort();

                localStorage.SetItem("lstPrices", lstPrices);


            //var midPrice = (minPrice + maxPrice) / 2;
            //var n = SurfaceChartConfigurator.xAxisMaxCategories;
            ///// Cull the list of prices if it's more than 100
            //if (lstPrices.Count > n)
            //{
            //    var avgPrice = (Convert.ToDecimal(lstPrices.First()) + Convert.ToDecimal(lstPrices.Last())) / 2;
            //    if (midPrice < avgPrice)
            //        /// Remove higher prices
            //        lstPrices = lstPrices.ToArray().Take(n).ToList();
            //    else
            //        /// Remove lower prices
            //        lstPrices = lstPrices.ToArray().Skip(lstPrices.Count - n).ToList();
            //}
            /// Picture the spread as two 0 points, one at high bid, one at low ask
        }

        private static Dictionary<string, string> SetSeriesColors()
        {
            Dictionary<string, string> dictSeriesColor = new Dictionary<string, string>();
            string asksColor = "#8085e9"; //"#cb6992";
            string bidsColor = "#0479cc";
            string buysColor = "#90ed7d";
            string sellsColor = "#f45b5b";
            string spreadColor = "#8085e9";
            dictSeriesColor = new Dictionary<string, string>()
            {
                { "asks", asksColor } ,
                { "bids", bidsColor } ,
                { "salesAtAsk", buysColor } ,
                { "salesAtBid", sellsColor },
                { "spread", spreadColor }
            };
            return dictSeriesColor;
        }
    }
}

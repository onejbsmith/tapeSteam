#undef tracing
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
    public partial class StackedColumns3D
    {
        List<string> lstPrices = new List<string>();

        [Parameter]
        public Dictionary<string, BookDataItem[]> bookData
        {
            get { return _bookData; }
            set
            {
                _bookData = value;
#if tracing
                Console.WriteLine("3. ChartSetData");
#endif
                ChartSetData(value);
            }
        }
        Dictionary<string, BookDataItem[]> _bookData = new Dictionary<string, BookDataItem[]>();

        string chartJsFilename = "js/StackedColumns3DChart.js";

        static string chartJson = "";
        //@"[{
        //    name: 'John',
        //    data: [5, 3, 4, 7, 2],
        //    stack: 'male'
        //}, {
        //    name: 'Joe',
        //    data: [3, 4, 4, 2, 5],
        //    stack: 'male'
        //}, {
        //    name: 'Jane',
        //    data: [2, 5, 6, 2, 1],
        //    stack: 'female'
        //}, {
        //    name: 'Janet',
        //    data: [3, 0, 4, 4, 3],
        //    stack: 'female'
        //}]";

        public int seconds { get; set; }

        string name = "not set";

        static StackedColumns3DChart chart = new StackedColumns3DChart();

        static Dictionary<string, string> dictSeriesColor;

        static TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        string chartSeriesJson = "";

        static double maxSize;

        protected override async Task OnInitializedAsync()
        {
            dictSeriesColor = SetSeriesColors();
            //ChartConfigure.seconds = 3;
            await Task.CompletedTask;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotNetReference = DotNetObjectReference.Create(this);
                await jsruntime.InvokeVoidAsync("Initialize", dotNetReference);
                //await jsruntime.InvokeVoidAsync("getChartJson");
                //await jsruntime.InvokeVoidAsync("getChartSeriesJson");


            }
        }

        [JSInvokable("getChartJson")]
        public async Task getChartJson(string jsonResponse)
        {
            /// get the chart as a POCO 
#if tracing
            Console.WriteLine("1. getChartJson");
#endif

            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
            chart = JsonSerializer.Deserialize<StackedColumns3DChart>(jsonResponse);

            /// We set some static chart Properties here and pass back to js
            chart.title.text = "";
            chart.chart.options3d.enabled = true;
            chart.yAxis.title.text = "Size";
            chart.plotOptions.column.depth = 100;
            chart.plotOptions.column.grouping = false;
            chart.yAxis.max = ChartConfigure.yAxisHigh;

            //chart.plotOptions.series.pointWidth = 100;

            chartJson = JsonSerializer.Serialize<StackedColumns3DChart>(chart);
            Console.WriteLine("2. getChartJson");

            Console.WriteLine(chartJson); /// to capture the chart object's json from js
            await Task.Yield();
        }

        private async Task ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        {

            try
            {
#if tracing
                Console.WriteLine("4. ChartSetData");
#endif

#if tracing
                Console.WriteLine("5. ChartSetData");
#endif
                var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };
                /// Get the price range min and max over all items in the dictionary
                /// only if they changed set isDirty = true
                Chart_MaintainPriceAxis(bookDataItems, seriesOrder);
                //AddSpreadPointsToBookData(bookDataItems);
#if tracing
                Console.WriteLine("6. ChartSetData");
#endif
                var categories = lstPrices.ToArray();

                /// only if they changed set isDirty = true
                chart.xAxis.categories = categories;

                /// 
                Chart_AddSpreadPlotBand(bookDataItems, categories);
#if tracing
                Console.WriteLine("7. ChartSetData");
#endif

                Chart_AddBollingerPlotLines(bookDataItems, categories);
#if tracing
                Console.WriteLine("8. ChartSetData");
#endif
                /// Convert BookDataItem[] to Series1[]
                var seriesList = new List<Series1>();
                Chart_BuildSeriesData(bookDataItems, seriesOrder, categories, seriesList);
#if tracing
                Console.WriteLine("9. ChartSetData");
#endif
                chart.series = seriesList.ToArray();

                Chart_AdjustYaxis(bookDataItems);
#if tracing

                Console.WriteLine("10. ChartSetData");
#endif
                /// Send the new data and settings to the HighChart3D component

                chartJson = JsonSerializer.Serialize<StackedColumns3DChart>(chart);
#if tracing
                Console.WriteLine("11. ChartSetData");
#endif
                ///
                /// We should only send the series and the categories, not the whole chart
                /// But minor amouunt of config data involved in redraw
                /// Shuld only send series i case of a rolling series, 
                /// instead of sending the whole matrix of data, just send the new row.              /// chartSeriesJson = System.Text.Json.JsonSerializer.Serialize<Series1[]>(chart.series);


                //var chart3DseriesJson = JsonSerializer.Serialize<Series1[]>(chart.series); ;
                //var chart3DxAxisJson = JsonSerializer.Serialize<Xaxis>(chart.xAxis);


                //await jsruntime.InvokeVoidAsync("setChart3Dseries", new object[] { chart3DseriesJson, chart3DxAxisJson });
                await Task.CompletedTask;
                StateHasChanged();

            }
            catch
            {

            }
        }

        private void Chart_AdjustYaxis(Dictionary<string, BookDataItem[]> bookDataItems)
        {
            try
            {
                chart.yAxis.max = ChartConfigure.yAxisHigh;
                if (ChartConfigure.yAxisMaxAutoReset == false) return;

                //var highestSize = bookDataItems.Max(dict => dict.Value.Max(it => it.Size));
                var highestSize = maxSize;
                if (highestSize > chart.yAxis.max)
                {
                    chart.yAxis.max = (int)Math.Ceiling(highestSize / 5000) * 5000;
                    ChartConfigure.yAxisHigh = chart.yAxis.max;
                }
            }
            catch (Exception ex)
            { }
        }

        private static void Chart_AddSpreadPlotBand(Dictionary<string, BookDataItem[]> bookDataItems, string[] categories)
        {
            var highBid = categories.ToList().IndexOf(bookDataItems["bids"][0].Price.ToString("n2"));
            var lowAsk = categories.ToList().IndexOf(bookDataItems["asks"][0].Price.ToString("n2"));

            chart.xAxis.plotBands = new Plotband[]
            {
                    new Plotband()
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
            Console.WriteLine("7a. Chart_AddBollingerPlotLines");
            Console.WriteLine("7a. bollingerBands.mid=" + midPrice);

#endif
            //var lowAsk = categories.ToList().IndexOf(bookDataItems["asks"][0].Price.ToString("n2"));

            var low = categories.ToList().IndexOf(lowPrice); ;
            var midlow = categories.ToList().IndexOf(midlowPrice); ;
            var mid = categories.ToList().IndexOf(midPrice); ;
            var midhigh = categories.ToList().IndexOf(midhighPrice);
            var high = categories.ToList().IndexOf(highPrice);

            var midCategory = categories.Length / 2;

            chart.xAxis.plotLines = new Plotline[]
            {
                new Plotline()
                {  value=low,
                    color="magenta",
                    width=4,
                    zIndex = 2
                },
                 new Plotline()
                {  value=midlow,
                    color="magenta",
                    width=2,
                    zIndex = 2
                },
                new Plotline()
                {  value=mid,
                    color="cyan",
                    width=4,
                    zIndex = 2
                },
                new Plotline()
                {  value=midhigh,
                    color="red",
                    width=2,
                    zIndex = 2
                },
                new Plotline()
                {  value=high,
                    color="red",
                    width=4,
                    zIndex = 2
                },
                new Plotline()
                {  value=midCategory,
                    color="#666666",
                    width=8,
                    zIndex = 1
                },
            };
        }

        private static void Chart_BuildSeriesData(Dictionary<string, BookDataItem[]> bookDataItems, string[] seriesOrder, string[] categories, List<Series1> seriesList)
        {
            for (int i = 0; i < bookDataItems.Count; i++)
            {
                /// Create the series
                var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]
                var seriesItem = new Series1()   /// Chart Series1 item
                {
                    name = (seriesName == "salesAtAsk") ? "Buys" : seriesName == "salesAtBid" ? "Sells" : textInfo.ToTitleCase(seriesName),
                    // This array needs to be the 100 slots and Size put in slot for Price
                    data = new int?[categories.Length],
                    color = dictSeriesColor[seriesName],
                    stack = seriesName == "asks" || seriesName == "salesAtAsk" ? "Sell Demand" : "Buy Demand"
                };

                /// Fill out the series data 
                foreach (var item in bookDataItems[seriesName])    /// item is one BookDataItem
                {
                    /// Place bookitem Sizes in Series1 data
                    var index = categories.ToList().IndexOf(item.Price.ToString("n2"));

                    seriesItem.data[index] = (int)item.Size;
                }
                /// Add to chart Series1
                seriesList.Add(seriesItem);
                /// Set chart Series1
            }
        }

        private void Chart_MaintainPriceAxis(Dictionary<string, BookDataItem[]> bookDataItems, string[] seriesOrder)
        {
            var minPrice = 999999m; // bookDataItems.Min(items => items.Value.Min(item => item.Price));
            var maxPrice = 0m; // bookDataItems.Max(items => items.Value.Max(item => item.Price));

            var x = new List<string>();
            if(lstPrices.Count==0)
                lstPrices = localStorage.GetItem<List<string>>("lstPrices");
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
                    maxSize = Math.Max(item.Size, maxSize);
                }
            }
            lstPrices.Sort();

            var midPrice = (minPrice + maxPrice) / 2;
            var n = ChartConfigure.xAxisMaxCategories;
            /// Cull the list of prices if it's more than 100
            if (lstPrices.Count > n)
            {
                var avgPrice = (Convert.ToDecimal(lstPrices.First()) + Convert.ToDecimal(lstPrices.Last())) / 2;
                if (midPrice < avgPrice)
                    /// Remove higher prices
                    lstPrices = lstPrices.ToArray().Take(n).ToList();
                else
                    /// Remove lower prices
                    lstPrices = lstPrices.ToArray().Skip(lstPrices.Count - n).ToList();
            }
            localStorage.SetItem("lstPrices", lstPrices);
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

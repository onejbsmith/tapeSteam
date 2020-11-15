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
        string id = "StackedColumns3DSurface";
        List<string> lstPrices = new List<string>();


        [Parameter]
        public Dictionary<string, BookDataItem[]> bookData
        {
            get { return _bookData; }
            set
            {
                _bookData = value;
                ChartSetData(value);
            }
        }
        Dictionary<string, BookDataItem[]> _bookData = new Dictionary<string, BookDataItem[]>();

        static string chart3Djson = "";
        //    @"[{
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

        static Surface.StackedColumns3DSurface chart = new Surface.StackedColumns3DSurface();

        static Dictionary<string, string> dictSeriesColor;

        static TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        static List<Surface.Series1> seriesList = new List<Surface.Series1>();

        string chartSeriesJson = "";

        protected override async Task OnInitializedAsync()
        {
            dictSeriesColor = SetSeriesColors();
            //SurfaceChartConfigurator.seconds = 3;

        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotNetReference = DotNetObjectReference.Create(this);
                await jsruntime.InvokeVoidAsync("Initialize", dotNetReference, id);
                //await jsruntime.InvokeVoidAsync("getChartJson");
                //await jsruntime.InvokeVoidAsync("getChartSeriesJson");


            }
        }

        [JSInvokable("getChartJson")]
        public async Task getChartJson(string jsonResponse)
        {
            /// get the chart as a POCO 
            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
            Console.WriteLine("1. StackedColumns3DSurface getChartJson");
            chart = JsonSerializer.Deserialize<Surface.StackedColumns3DSurface>(jsonResponse);

            /// We set some static chart Properties here and pass back to js
            chart.title.text = "";
            chart.chart.options3d.enabled = true;
            //chart.yAxis.title.text = "Size";
            //chart.plotOptions.series.pointWidth = 100;

            chart3Djson = JsonSerializer.Serialize<Surface.StackedColumns3DSurface>(chart);

            Console.WriteLine(jsonResponse); /// to capture the chart object's json from js
            Console.WriteLine("2. StackedColumns3DSurface getChartJson");
            await Task.Yield();
        }

        private async Task ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        {

            try

            {
                Console.WriteLine("3. StackedColumns3DSurface ChartSetData");
                chart.title.text = "Surface";
                //chart.plotOptions.column.depth = 1;

                chart.yAxis.max = SurfaceChartConfigurator.yAxisHigh;

                var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };
                /// Get the price range min and max over all items in the dictionary
                Chart_MaintainPriceAxis(bookDataItems, seriesOrder);                //AddSpreadPointsToBookData(bookDataItems);
                Console.WriteLine("4. StackedColumns3DSurface ChartSetData");
                var categories = lstPrices.ToArray();
                chart.zAxis.categories = categories;

                /// 
                //Chart_AddSpreadPlotBand(bookDataItems, categories);

                //Chart_AddBollingerPlotLines(bookDataItems, categories);
                /// Convert BookDataItem[] to Series1[]
                Chart_BuildSeriesData(bookDataItems, seriesOrder, categories, seriesList);
                Console.WriteLine("5. StackedColumns3DSurface ChartSetData");
                chart.series = seriesList.ToArray();

                //chart.chart.options3d.depth = Math.Max(100, chart.series.Count() * chart.plotOptions.column.depth);

                /// Send the new data and settings to the HighChart3D component

                chart3Djson = JsonSerializer.Serialize<Surface.StackedColumns3DSurface>(chart);
                Console.WriteLine("6. StackedColumns3DSurface ChartSetData");
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

        private static void Chart_AddSpreadPlotBand(Dictionary<string, BookDataItem[]> bookDataItems, string[] categories)
        {
            var highBid = categories.ToList().IndexOf(bookDataItems["bids"][0].Price.ToString("n2"));
            var lowAsk = categories.ToList().IndexOf(bookDataItems["asks"][0].Price.ToString("n2"));

        //    chart.xAxis.plotBands = new Plotband[]
        //    {
        //            new Plotband()
        //            { from=highBid,
        //                to =lowAsk,
        //                color="#888888",
        //                //label= new Label()
        //                //{
        //                //    text="Spread"
        //                //}

        //            }
        //    };
        }
        private static void Chart_AddBollingerPlotLines(Dictionary<string, BookDataItem[]> bookDataItems, string[] categories)
        {
            var lowPrice = 288.93.ToString("n2");
            var midPrice = 289.27.ToString("n2");
            var highPrice = 289.65.ToString("n2");

            //var lowAsk = categories.ToList().IndexOf(bookDataItems["asks"][0].Price.ToString("n2"));

            var low = categories.ToList().IndexOf(lowPrice); ;
            var mid = categories.ToList().IndexOf(midPrice); ;
            var high = categories.ToList().IndexOf(highPrice);

            var midCategory = categories.Length / 2;

            //chart.xAxis.plotLines = new Plotline[]
            //{
            //    new Plotline()
            //    {  value=low,
            //        color="purple",
            //        width=4
            //    },
            //    new Plotline()
            //    {  value=mid,
            //        color="cyan",
            //        width=4
            //    },
            //    new Plotline()
            //    {  value=high,
            //        color="red",
            //        width=4
            //    },
            //    new Plotline()
            //    {  value=midCategory,
            //        color="#666666",
            //        width=6
            //    },
            //};
        }

        private static void Chart_BuildSeriesData(Dictionary<string, BookDataItem[]> bookDataItems, string[] seriesOrder, string[] categories, List<Surface.Series1> seriesList)
        {


            /// Remove any over 600*4 series (10 minutes)
            /// 
            var minutesToKeep = 10;
            var seriesToKeep = minutesToKeep * seriesOrder.Length * 60;
            seriesList = seriesList.Take(seriesToKeep).ToList();

            /// Remove previous 4 series from the legend
            /// They will be the first four series
            ///             
            foreach (var series in seriesList.Take(seriesOrder.Length))
            {
                series.showInLegend = false;
            }


            for (int i = 0; i < bookDataItems.Count; i++)
            {
                /// Create the series
                //var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]
                //var seriesItem = new Surface.Series1()   /// Chart Series1 item
                //{
                //    name = (seriesName == "salesAtAsk") ? "Buys" : seriesName == "salesAtBid" ? "Sells" : textInfo.ToTitleCase(seriesName),
                //    // This array needs to be the 100 slots and Size put in slot for Price
                //    data = new int?[categories.Length],
                //    color = dictSeriesColor[seriesName],
                //    stack = seriesName == "asks" || seriesName == "salesAtAsk" ? "Sell Demand" : "Buy Demand",
                //    showInLegend = true
                //};

                ///// Fill out the series data (needs to be 2d)
                //foreach (var item in bookDataItems[seriesName])    /// item is one BookDataItem
                //{
                //    /// Place bookitem Sizes in Series1 data
                //    var index = categories.ToList().IndexOf(item.Price.ToString("n2"));

                //    seriesItem.data[index] = (int)item.Size;
                //}


                ///// Add to chart Series1 as first series
                //seriesList.Insert(0, seriesItem);
                /// Set chart Series1
            }
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

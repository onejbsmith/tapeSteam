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

        static string chart3Djson = @"[{
            name: 'John',
            data: [5, 3, 4, 7, 2],
            stack: 'male'
        }, {
            name: 'Joe',
            data: [3, 4, 4, 2, 5],
            stack: 'male'
        }, {
            name: 'Jane',
            data: [2, 5, 6, 2, 1],
            stack: 'female'
        }, {
            name: 'Janet',
            data: [3, 0, 4, 4, 3],
            stack: 'female'
        }]";

        string name = "not set";

        static StackedColumns3DChart chart = new StackedColumns3DChart();

        static Dictionary<string, string> dictSeriesColor;
        
        static TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        protected override async Task OnInitializedAsync()
        {
            dictSeriesColor = SetSeriesColors();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotNetReference = DotNetObjectReference.Create(this);
                await jsruntime.InvokeVoidAsync("Initialize", dotNetReference);
                await jsruntime.InvokeVoidAsync("getChartJson");
                //await jsruntime.InvokeVoidAsync("getChartSeriesJson");


            }
        }

        [JSInvokable("getChartJson")]
        public async Task getChartJson(string jsonResponse)
        {
            /// get the chart as a POCO 
            chart = JsonSerializer.Deserialize<StackedColumns3DChart>(jsonResponse);

            /// We set some static chart Properties here and pass back to js
            chart.title.text = ChartConfigure.seconds.ToString();
            chart.chart.options3d.enabled = true;
            chart.yAxis.title.text = "Size";
            chart.plotOptions.series.pointWidth = 100;

            chart3Djson = JsonSerializer.Serialize<StackedColumns3DChart>(chart);

            Console.WriteLine(jsonResponse); /// to capture the chart object's json from js
            await Task.Yield();
        }

        private async Task ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        {

            try
            {

                /// Get the price range min and max over all items in the dictionary
                var minPrice = 999999m; // bookDataItems.Min(items => items.Value.Min(item => item.Price));
                var maxPrice = 0m; // bookDataItems.Max(items => items.Value.Max(item => item.Price));
                var seriesOrder = new string[] { "bids", "salesAtBid", "spread", "salesAtAsk", "asks" };

                /// Set up the Categories list
                var lstPrices = new List<string>();
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

                /// Picture the spread as two 0 points, one at high bid, one at low ask
                AddSpreadPointsToBookData(bookDataItems);
                
                var categories = lstPrices;

                /// Convert BookDataItem[] to Series[]
                var seriesList = new List<Series>();
                for (int i = 0; i < bookDataItems.Count; i++)
                {
                    /// Create the series
                    var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]
                    var seriesItem = new Series()   /// Chart Series item
                    {
                        name = (seriesName == "salesAtAsk") ? "Buys" : seriesName == "salesAtBid" ? "Sells" : textInfo.ToTitleCase(seriesName),
                        // This array needs to be the 100 slots and Size put in slot for Price
                        data = new int?[categories.Count],
                        color = dictSeriesColor[seriesName],
                        stack = seriesName == "asks" || seriesName == "salesAtAsk" ? "Sell Demand" : "Buy Demand"
                    };

                    /// Fill out the series data 
                    foreach (var item in bookDataItems[seriesName])    /// item is one BookDataItem
                    {
                        /// Place bookitem Sizes in Series data
                        var index = categories.IndexOf(item.Price.ToString("n2"));

                        seriesItem.data[index] = (int)item.Size;
                    }
                    /// Add to chart Series
                    seriesList.Add(seriesItem);
                    /// Set chart Series
                    chart.series = seriesList.ToArray();
                }

                /// Send the new data to the HighChart3D component
                /// We should only send the series and the categories, not the whole chart
                chart3Djson = JsonSerializer.Serialize<StackedColumns3DChart>(chart);

                //var chart3DseriesJson = JsonSerializer.Serialize<Series[]>(chart.series); ;
                //var chart3DxAxisJson = JsonSerializer.Serialize<Xaxis>(chart.xAxis);


                //await jsruntime.InvokeVoidAsync("setChart3Dseries", new object[] { chart3DseriesJson, chart3DxAxisJson });
                await Task.CompletedTask;
                StateHasChanged();

            }
            catch
            {

            }
        }

        private static void AddSpreadPointsToBookData(Dictionary<string, BookDataItem[]> bookDataItems)
        {
            /// Picture the spread one at high bid one at low ask
            var highBid = bookDataItems["bids"][0].Price;
            var lowAsk = bookDataItems["asks"][0].Price;

            /// Create a series for spread with the two points above at 0 SIze
            /// and add to bookDataItems that just came in
            /// 
            var spreadPoints = new BookDataItem[2]
            {
                    new BookDataItem() { Price=highBid, dateTime=DateTime.Now, Size=0 },
                    new BookDataItem() { Price=lowAsk, dateTime=DateTime.Now, Size=0 },
            };
            bookDataItems.Add("spread", spreadPoints);
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
    #region HighChart Classes
    public class StackedColumns3DChart
    {
        public Chart chart { get; set; }
        public Title title { get; set; }
        public Xaxis xAxis { get; set; }
        public Yaxis yAxis { get; set; }
        public Plotoptions plotOptions { get; set; }
        public Exporting exporting { get; set; }
        public Series[] series { get; set; }
    }
    public class Series
    {
        public string name { get; set; }
        public int?[] data { get; set; }
        public string stack { get; set; }
        public string color { get; set; }
    }

    public class Chart
    {
        public string type { get; set; }
        public Options3d options3d { get; set; }
        public string zoomType { get; set; }
        public bool panning { get; set; }
        public string panKey { get; set; }
    }
    public class Plotoptions
    {
        public Column column { get; set; }
        public Series1 series { get; set; }

    }

    public class Series1
    {
        public int pointWidth { get; set; }
    }

    public class Exporting
    {
        public Menuitemdefinitions menuItemDefinitions { get; set; }
        public Buttons buttons { get; set; }
    }
    public class Options3d
    {
        public bool enabled { get; set; }
        public int alpha { get; set; }
        public int beta { get; set; }
        public int viewDistance { get; set; }
        public int depth { get; set; }
    }

    public class Title
    {
        public string text { get; set; }
    }

    public class Xaxis
    {
        public string[] categories { get; set; }
        public Labels labels { get; set; }
    }

    public class Labels
    {
        public bool skew3d { get; set; }
        public Style style { get; set; }
    }

    public class Style
    {
        public string fontSize { get; set; }
    }

    public class Yaxis
    {
        public bool allowDecimals { get; set; }
        public int min { get; set; }
        public Title1 title { get; set; }
    }

    public class Title1
    {
        public string text { get; set; }
        public bool skew3d { get; set; }
    }

    public class Column
    {
        public string stacking { get; set; }
        public int depth { get; set; }
    }


    public class Menuitemdefinitions
    {
        public Label label { get; set; }
    }

    public class Label
    {
        public string text { get; set; }
    }

    public class Buttons
    {
        public Contextbutton contextButton { get; set; }
    }

    public class Contextbutton
    {
        public string[] menuItems { get; set; }
    }
    #endregion

}

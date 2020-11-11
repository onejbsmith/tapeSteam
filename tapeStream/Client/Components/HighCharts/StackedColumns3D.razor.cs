﻿using Blazorise.Utils;
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

        public int seconds { get; set; }

        string name = "not set";

        static StackedColumns3DChart chart = new StackedColumns3DChart();

        static Dictionary<string, string> dictSeriesColor;

        static TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        protected override async Task OnInitializedAsync()
        {
            dictSeriesColor = SetSeriesColors();
            //ChartConfigure.seconds = 3;
 
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
            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
            chart = JsonSerializer.Deserialize<StackedColumns3DChart>(jsonResponse);

            /// We set some static chart Properties here and pass back to js
            chart.title.text = "";
            chart.chart.options3d.enabled = true;
            chart.yAxis.title.text = "Size";
            //chart.plotOptions.series.pointWidth = 100;

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
                var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };

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
                var midPrice = (minPrice + maxPrice) / 2;
                var n = 75;
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
                /// Picture the spread as two 0 points, one at high bid, one at low ask
                //AddSpreadPointsToBookData(bookDataItems);

                var categories = lstPrices;

                chart.xAxis.categories = categories.ToArray();
              
                /// Convert BookDataItem[] to Series1[]
                var seriesList = new List<Series1>();
                for (int i = 0; i < bookDataItems.Count; i++)
                {
                    /// Create the series
                    var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]
                    var seriesItem = new Series1()   /// Chart Series1 item
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
                        /// Place bookitem Sizes in Series1 data
                        var index = categories.IndexOf(item.Price.ToString("n2"));

                        seriesItem.data[index] = (int)item.Size;
                    }
                    /// Add to chart Series1
                    seriesList.Add(seriesItem);
                    /// Set chart Series1
                }
                chart.series = seriesList.ToArray();

                /// Send the new data to the HighChart3D component
                /// We should only send the series and the categories, not the whole chart
                chart3Djson = JsonSerializer.Serialize<StackedColumns3DChart>(chart);

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
}

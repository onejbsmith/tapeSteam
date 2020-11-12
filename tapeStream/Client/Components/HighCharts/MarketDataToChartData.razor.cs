#define tracing

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Client.Data.highcharts;
using tapeStream.Shared.Data;
using static tapeStream.Client.Data.highcharts.Highcharts3DMarketColumnChartData;

namespace tapeStream.Client.Components.HighCharts
{
    /// <summary>
    /// Takes the data coming from the tapeStream.Shared BookColumnsService.cs
    /// and converts it to the Json form needed by the Highcharts3DMarketColumnChart.razor.cs component
    /// </summary>
    public partial class MarketDataToChartData
    {
        private bool isChartCreated;

        /// <summary>
        /// =============================================================================================
        ///  FOR THE Highcharts3DMarketColumnChart component
        ///  ============================================================================================
        /// </summary>
        /// 

        HighchartChart market3DColumnChart;

        [Parameter]
        public Dictionary<string, BookDataItem[]> bookData
        {
            get { return _bookData; }
            set
            {
                _bookData = value;
#if tracing
                               Console.WriteLine(MethodBase.GetCurrentMethod().Module.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif
                ChartSetData(value).Wait();
            }
        }

        //Highcharts3DMarketColumnChart chart;

        Data.highcharts.HighCharts_Enums.DataUpdateType updateSeries =
                Data.highcharts.HighCharts_Enums.DataUpdateType.Replace;

        Dictionary<string, BookDataItem[]> _bookData = new Dictionary<string, BookDataItem[]>();
        List<string> lstPrices = new List<string>();

        static Dictionary<string, string> dictSeriesColor;
        static TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        [Parameter]
        public string chartJson { get; set; } = @"[{
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

        protected override async Task OnInitializedAsync()
        {
#if tracing
                               Console.WriteLine(MethodBase.GetCurrentMethod().Module.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif           
            dictSeriesColor = InitSeriesColors();
            Highchart.OnChartCreated += async () => await Highchart_OnChartCreated();
            await Task.CompletedTask;
        }

        private Task Highchart_OnChartCreated()
        {
#if tracing
                               Console.WriteLine(MethodBase.GetCurrentMethod().Module.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif            /// Chart has been drawn for the first time
            /// Setting static properties
            /// 
            isChartCreated = true;

            market3DColumnChart = Highcharts3DMarketColumnChart.market3DColumnChart;

            market3DColumnChart.title.text = "QQQ Synched Level II / Time & Sales";
            market3DColumnChart.chart.options3d.enabled = true;
            market3DColumnChart.yAxis.title.text = "Size";
            market3DColumnChart.plotOptions.series.pointWidth = 100;

            return Task.CompletedTask;
        }
        static List<MarketColumnSeries> seriesList = new List<MarketColumnSeries>();

        private async Task ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        {
            try
            {
                if (!isChartCreated) return;
                //if (isChartCreated)
                //    chartJson = await _client.GetStringAsync(Highcharts3DMarketColumnChartData.chartJsFilename);

#if tracing
                               Console.WriteLine(MethodBase.GetCurrentMethod().Module.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif
                /// Get the price range min and max over all items in the dictionary
                var minPrice = 999999m; // bookDataItems.Min(items => items.Value.Min(item => item.Price));
                var maxPrice = 0m; // bookDataItems.Max(items => items.Value.Max(item => item.Price));
                var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };

                /// Set up the Categories list
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
                /// Cull the list of prices if it's more than 100
                if (lstPrices.Count > 75)
                {
                    var avgPrice = (Convert.ToDecimal(lstPrices.First()) + Convert.ToDecimal(lstPrices.Last())) / 2;
                    if (midPrice < avgPrice)
                        /// Remove higher prices
                        lstPrices = lstPrices.ToArray().Take(100).ToList();
                    else
                        /// Remove lower prices
                        lstPrices = lstPrices.ToArray().Skip(lstPrices.Count - 100).ToList();
                }
                /// Picture the spread as two 0 points, one at high bid, one at low ask
                //AddSpreadPointsToBookData(bookDataItems);

                var categories = lstPrices;

                /// Convert BookDataItem[] to Series[]
                for (int i = 0; i < bookDataItems.Count; i++)
                {
                    /// Create the series
                    var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]

                    var newName = (seriesName == "salesAtAsk") ? "Buys" : seriesName == "salesAtBid" ? "Sells" : textInfo.ToTitleCase(seriesName);
                    var newData = new int?[categories.Count];
                    var newStack = seriesName == "asks" || seriesName == "salesAtAsk" ? "Sell Demand" : "Buy Demand";
                    var newColor = dictSeriesColor[seriesName];

                    var seriesItem = new MarketColumnSeries()   /// Chart Series item
                    {
                        name = newName,
                        data = newData,
                        color = newColor,
                        stack = newStack
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
                    /// 

                    /// Somegow get tg
                    market3DColumnChart.series = seriesList.ToArray();
                    market3DColumnChart.xAxis.categories = categories.ToArray();
                }

                /// Send the new data to the HighChart3D component
                /// We should only send the series and the categories, not the whole chart
                chartJson = JsonSerializer.Serialize<HighchartChart>(market3DColumnChart);

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

        private static Dictionary<string, string> InitSeriesColors()
        {
#if tracing
                               Console.WriteLine(MethodBase.GetCurrentMethod().Module.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif
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

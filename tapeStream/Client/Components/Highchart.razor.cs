#define tracing


using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using static tapeStream.Client.Data.highcharts.HighCharts_Enums;

namespace tapeStream.Client.Components
{
    public partial class Highchart
    {
        //private bool isUpdate;
        [Parameter]
        public string chartJsFilename { get; set; }

        [Parameter]
        public string chartJson
        {
            get { return _chartJson; }
            set
            {
                if (value != _chartJson)
                {
                    _chartJson = value;
#if tracing
                    Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif 
                    DrawChart();
                }
            }
        }
        private string _chartJson;

        [Parameter]
        public string chartSeriesJson
        {
            get { return seriesJson; }
            set
            {
                if (value != seriesJson)
                {
                    seriesJson = value;
#if tracing
                    Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif                  
                    updateHighchartSeries();
                }
            }
        }
        private string seriesJson;

        [Parameter]
        public DataUpdateType isSeriesShifted { get; set; }

        private string id { get; set; } = "Highchart" + Guid.NewGuid().ToString();
        //private string id { get; set; } = "Highchart" + Guid.NewGuid().ToString();

        static public event Action OnChartCreated;
        private static void ChartCreated() => OnChartCreated?.Invoke();

        /// =========================================================================================================================================================

        /// Get the chart json from the passed in filename
        protected override async Task OnInitializedAsync()
        {
#if tracing
            Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif
            chartJson = await _client.GetStringAsync(chartJsFilename);
            Console.WriteLine($"OnInitializedAsync chartJson => " + chartJson);
        }

  
        private async Task DrawChart()
        {
#if tracing
            Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif          
            if (!string.IsNullOrEmpty(chartJson))
            {
                /// Display the chart
                await jsruntime.InvokeAsync<string>("loadHighchart", new object[] { id, chartJson });
            }
        }

        private async Task updateHighchartSeries()
        {
#if tracing
            Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " +  MethodBase.GetCurrentMethod().Name);
#endif          
            if (!string.IsNullOrEmpty(seriesJson))
            {
                /// Display the chart with new series data
                if (isSeriesShifted == DataUpdateType.Replace)
                    await jsruntime.InvokeAsync<string>("updateHighchartSeries", new object[] { seriesJson });
                else
                    await jsruntime.InvokeAsync<string>("appendHighchartSeries", new object[] { seriesJson, isSeriesShifted == DataUpdateType.Shift });
            }
        }



        //private async Task ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        //{

        //    try
        //    {

        //        /// Get the price range min and max over all items in the dictionary
        //        var minPrice = 999999m; // bookDataItems.Min(items => items.Value.Min(item => item.Price));
        //        var maxPrice = 0m; // bookDataItems.Max(items => items.Value.Max(item => item.Price));
        //        var seriesOrder = new string[] { "bids", "salesAtBid", "spread", "salesAtAsk", "asks" };

        //        /// Set up the Categories list
        //        var lstPrices = new List<string>();
        //        foreach (var name in seriesOrder)
        //        {
        //            foreach (var item in bookDataItems[name])
        //            {
        //                if (!lstPrices.Contains(item.Price.ToString("n2")))
        //                    lstPrices.Add(item.Price.ToString("n2"));

        //                minPrice = Math.Min(item.Price, minPrice);
        //                maxPrice = Math.Max(item.Price, maxPrice);
        //            }
        //        }
        //        lstPrices.Sort();

        //        /// Picture the spread as two 0 points, one at high bid, one at low ask
        //        AddSpreadPointsToBookData(bookDataItems);

        //        var categories = lstPrices;

        //        /// Convert BookDataItem[] to Series[]
        //        var seriesList = new List<Series>();
        //        for (int i = 0; i < bookDataItems.Count; i++)
        //        {
        //            /// Create the series
        //            var seriesName = seriesOrder[i];  /// Dictionary where values are BookDataItem[]
        //            var seriesItem = new Series()   /// Chart Series item
        //            {
        //                name = (seriesName == "salesAtAsk") ? "Buys" : seriesName == "salesAtBid" ? "Sells" : textInfo.ToTitleCase(seriesName),
        //                // This array needs to be the 100 slots and Size put in slot for Price
        //                data = new int?[categories.Count],
        //                color = dictSeriesColor[seriesName],
        //                stack = seriesName == "asks" || seriesName == "salesAtAsk" ? "Sell Demand" : "Buy Demand"
        //            };

        //            /// Fill out the series data 
        //            foreach (var item in bookDataItems[seriesName])    /// item is one BookDataItem
        //            {
        //                /// Place bookitem Sizes in Series data
        //                var index = categories.IndexOf(item.Price.ToString("n2"));

        //                seriesItem.data[index] = (int)item.Size;
        //            }
        //            /// Add to chart Series
        //            seriesList.Add(seriesItem);
        //            /// Set chart Series
        //            chart.series = seriesList.ToArray();
        //        }

        //        /// Send the new data to the HighChart3D component
        //        /// We should only send the series and the categories, not the whole chart
        //        chart3Djson = JsonSerializer.Serialize<StackedColumns3DChart>(chart);

        //        //var chart3DseriesJson = JsonSerializer.Serialize<Series[]>(chart.series); ;
        //        //var chart3DxAxisJson = JsonSerializer.Serialize<Xaxis>(chart.xAxis);


        //        //await jsruntime.InvokeVoidAsync("setChart3Dseries", new object[] { chart3DseriesJson, chart3DxAxisJson });
        //        await Task.CompletedTask;
        //        StateHasChanged();

        //    }
        //    catch
        //    {

        //    }
        //}


    }
}

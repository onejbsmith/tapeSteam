using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
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


        public class ChartSeries
        {
            public Series[] series { get; set; }
        }

        public class Series
        {
            public string name { get; set; }
            public int[] data { get; set; }
            public string stack { get; set; }
        }

        static ChartSeries chartSeries = new ChartSeries();

        protected override async Task OnInitializedAsync()
        {

            /// Connect to the web socket, passing it a ref to this page, so it can call methods from javascript
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotNetReference = DotNetObjectReference.Create(this);
                await jsruntime.InvokeVoidAsync("Initialize", dotNetReference);
                await jsruntime.InvokeVoidAsync("getChartSeriesJson");
            }
        }

        private void ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        {
            /// Convert BookDataItem[] to Series[]
            var seriesList = new List<Series>();
            for (int i = 0; i < bookDataItems.Count; i++)
            {
                var seriesName = bookDataItems.Keys.ElementAt(i);
                foreach (var item in bookDataItems[seriesName])
                {
                    var seriesItem = new Series()
                    {
                        name = seriesName,
                        data = bookDataItems[seriesName].Select(item => (int)item.Size).ToArray(),  
                        // This array needs to be the 100 slots and Size put in slot for Price
                        stack = seriesName == "asks" || seriesName == "sells" ? "Sell Demand" : "Buy Demand"
                    };
                    seriesList.Add(seriesItem);
                }
            }
            /// Convert the Series[] to a jsonString
            /// Send the new data to the HighChart3D component
            //chart3Djson = JsonSerializer.Serialize<Series[]>(seriesList.ToArray());
            StateHasChanged();
        }


        [JSInvokable("getChartSeriesJson")]
        public async Task getChartSeriesJson(string jsonResponse)
        {
            chartSeries.series = JsonSerializer.Deserialize<Series[]>(jsonResponse);

            name = chartSeries.series[0].name;

            await Task.Yield();

            StateHasChanged();
        }
    }
}

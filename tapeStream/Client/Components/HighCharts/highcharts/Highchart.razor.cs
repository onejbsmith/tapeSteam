using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

using System.Threading.Tasks;
using static tapeStream.Client.Data.highcharts.HighCharts_Enums;

namespace tapeStream.Client.Components.HighCharts.highcharts

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
            chartJson = await _client.GetStringAsync(chartJsFilename);
            //Console.WriteLine($"OnInitializedAsync chartJson => " + chartJson);
        }

        /// Give the js a reference back to this cs
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotNetReference = Microsoft.JSInterop.DotNetObjectReference.Create(this);
                await jsruntime.InvokeVoidAsync("Initialize", dotNetReference);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task DrawChart()
        {
            if (!string.IsNullOrEmpty(chartJson))
            {
                /// Display the chart
                await jsruntime.InvokeAsync<string>("loadHighchart", new object[] { id, chartJson });
            }
        }

        private async Task updateHighchartSeries()
        {
            if (!string.IsNullOrEmpty(seriesJson))
            {
                /// Display the chart with new series data
                if (isSeriesShifted == DataUpdateType.Replace)
                    await jsruntime.InvokeAsync<string>("updateHighchartSeries", new object[] { seriesJson });
                else
                    await jsruntime.InvokeAsync<string>("appendHighchartSeries", new object[] { seriesJson, isSeriesShifted == DataUpdateType.Shift });
            }
        }


        [JSInvokable("getChartJson")]
        public async Task getChartJson(string jsonResponse)
        {
            /// initialize the cs chart object in the specific chart component
            //Console.WriteLine($"Highchart getChartJson chartJson => " + chartJson);
            ChartCreated();

            StateHasChanged();
            await Task.Yield();
        }
    }
}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Components.HighCharts.Base
{
    public partial class HighChart
    {

        [Parameter]
        public string chartJsFilename { get; set; }

        public static DataUpdateMode dataUpdateMode;

        public enum DataUpdateMode
        {
            Replace,
            Append,
            Shift
        }

        [Parameter]
        public string id { get; set; } //= "Highchart" + Guid.NewGuid().ToString();

        [Parameter] public string chartJson { get; set; }

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
        public DataUpdateMode updateMode { get; set; }


        [Parameter]
        public bool redrawChart
        {
            get { return _redrawChart; }
            set
            {
                _redrawChart = value;
                redrawChart2 = value;
            }
        }
        private bool _redrawChart;

        public static bool redrawChart2 { get; set; }

        protected async override Task OnParametersSetAsync()
        {
            base.OnParametersSet();
            StateHasChanged();
        }

        /// Get the chart json from the passed in filename
        protected override async Task OnInitializedAsync()
        {
            chartJson = await _client.GetStringAsync(chartJsFilename);

            StateHasChanged();

            //Console.WriteLine($"OnInitializedAsync chartJson => " + chartJson);
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!string.IsNullOrEmpty(chartJson))
            {
                await jsruntime.InvokeAsync<string>("loadHighchart", new object[] { id, chartJson, redrawChart2 });
               // if (redrawChart2 == false) redrawChart2 = true;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task updateHighchartSeries()
        {
            if (!string.IsNullOrEmpty(seriesJson))
            {
                /// Display the chart with new series data
                if (updateMode == DataUpdateMode.Replace)
                    await jsruntime.InvokeAsync<string>("updateHighchartSeries", new object[] { seriesJson });
                else
                    await jsruntime.InvokeAsync<string>("appendHighchartSeries", new object[] { seriesJson, updateMode == DataUpdateMode.Shift });
            }
        }

    }

}

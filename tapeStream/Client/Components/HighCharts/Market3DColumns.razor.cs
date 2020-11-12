using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Client.Data.highcharts;

using static tapeStream.Shared.Data.highcharts.HighCharts_Enums;

namespace tapeStream.Client.Components.HighCharts.highcharts
{
    public partial class Market3DColumns
    {

        Highchart highChartsBase;
        public static HighchartChart market3DColumnsChart;
        static string chartJsFilename = "js/highcharts/Market3DColumns.js";

        [Parameter]
        public string chartJson
        {
            get { return json; }
            set
            {
                if (value != json)
                {
                    json = value;
                }
            }
        }
        private string json;

        [Parameter]
        public string chartSeriesJson
        {
            get { return seriesJson; }
            set
            {
                if (value != seriesJson)
                {
                    seriesJson = value;
                }
            }
        }
        private string seriesJson;

        [Parameter]
        public DataUpdateType updateSeries { get; set; }



        protected override async Task OnInitializedAsync()
        {
            Highchart.OnChartCreated += async () => await Highchart_OnChartCreated();
            await Task.CompletedTask;
        }

        /// Capture the chart object's json from js
        private async Task Highchart_OnChartCreated()
        {
            if (!string.IsNullOrEmpty(highChartsBase.chartJson))
            {
                market3DColumnsChart = JsonSerializer.Deserialize<HighchartChart>(highChartsBase.chartJson);
                await base.InvokeAsync(StateHasChanged);
            }

            StateHasChanged();
            await Task.Yield();
        }
    }
}


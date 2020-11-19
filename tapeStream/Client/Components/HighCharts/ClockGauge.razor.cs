

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
using tapeStream.Client.Data.highcharts;
using tapeStream.Shared.Data;
using static HighCharts.Charts.Basic.Shared.HighCharts_Enums;

namespace tapeStream.Client.Components.HighCharts
{
    public partial class ClockGauge
    {
    //    Highchart highChartsBase;
        public static HighchartChart activityGauge;
        static string chartJsFilename = $"js/ClockGauge.chart.js?id={DateTime.Now.ToOADate()}";

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
            //Highchart.OnChartCreated += async () => await Highchart_OnChartCreated();
            chartJson = "Draw me please";
            await Task.CompletedTask;
        }

        /// Capture the chart object's json from js
        private async Task Highchart_OnChartCreated()
        {
            //if (!string.IsNullOrEmpty(highChartsBase.chartJson))
            //{
            //    activityGauge = JsonSerializer.Deserialize<HighchartChart>(highChartsBase.chartJson);
            //    await base.InvokeAsync(StateHasChanged);
            //}

            StateHasChanged();
            await Task.Yield();
        }
    }
}

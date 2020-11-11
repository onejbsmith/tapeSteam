using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Components.HighCharts.Base
{
    public partial class HighChart
    {
        public static DataUpdateMode dataUpdateMode;

        public enum DataUpdateMode
        {
            Replace,
            Append,
            Shift
        }

        [Parameter] public string Json { get; set; }
        private string id { get; set; } = "Highchart" + Guid.NewGuid().ToString();

        [Parameter]
        public string chartSeriesJson
        {
            get { return seriesJson; }
            set
            {
                if (value != seriesJson)
                {
                    seriesJson = value;
                    updateHighchartSeries().Wait();
                }
            }
        }
        private string seriesJson;

        [Parameter]
        public DataUpdateMode updateMode { get; set; }


        protected override void OnParametersSet()
        {
            StateHasChanged();
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!string.IsNullOrEmpty(Json))
            {
                await jsruntime.InvokeAsync<string>("loadHighchart", new object[] { id, Json });
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

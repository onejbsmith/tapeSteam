#undef tracing

using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Microsoft.JSInterop;
using static tapeStream.Client.Data.ClockGaugeData;
using tapeStream.Client.Data;

namespace tapeStream.Client.Components.HighCharts
{
    public partial class ClockGauge
    {

        [Parameter]

        public DateTime clockDateTime
        {
            get { return _clockDateTime; }
            set
            {
                _clockDateTime = value;
                RenderClock(_clockDateTime);
            }
        }
        private DateTime _clockDateTime = DateTime.Now;

        static string chartJsFilename = $"js/highcharts/ClockGauge.chart.js?id={DateTime.Now.ToOADate()}";
        static string chartJson = "";
        static string id = "ClockGauge";
        static bool redraw = false;

        static ClockGaugeData.HighchartChart chart = new ClockGaugeData.HighchartChart();

        protected override async Task OnInitializedAsync()
        {
            var dotNetReference = DotNetObjectReference.Create(this);
            await jsruntime.InvokeVoidAsync("Initialize", dotNetReference, id);

            //ChartConfigure.seconds = 3;
            await Task.CompletedTask;
        }

        [JSInvokable("getChartJson")]
        public async Task getChartJson(string jsonResponse)
        {
            /// get the chart as a POCO 
#if tracing
            Console.WriteLine("1. ClockGauge getChartJson");
            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
            Console.WriteLine(jsonResponse); /// to capture the chart object's json from js
#endif

            chart = JsonSerializer.Deserialize<ClockGaugeData.HighchartChart>(jsonResponse);
            JsConsole.JsConsole.GroupTable(jsruntime, chart, "getChartJson chart");

            /// We set some static chart Properties here and pass back to js



            //chart.plotOptions.series.pointWidth = 100;

            redraw = true;
            chartJson = JsonSerializer.Serialize<ClockGaugeData.HighchartChart>(chart);

#if tracing
            Console.WriteLine("2. ClockGauge getChartJson");
#endif
            Console.WriteLine(chartJson); /// to capture the chart object's json from js
            await Task.Yield();
        }


        private async Task RenderClock(DateTime value)
        {

#if tracing
            Console.WriteLine("3. ClockGauge RenderClock");
#endif
            chartJson = await _client.GetStringAsync(chartJsFilename);

            //            if (chart.series == null) return;
            //#if tracing
            //            JsConsole.JsConsole.GroupTable(jsruntime, chart, "RenderClock chart");
            //#endif
            //            /// Update the chart's c# object
            //            /// 
            //            chart.series[0].data[0].y = value.Hour;
            //            chart.series[0].data[1].y = value.Minute;
            //            chart.series[0].data[2].y = value.Second;
            //            /// Serialize to Json
            //            /// 
            //            var Json = JsonSerializer.Serialize<ClockGaugeData.HighchartChart>(chart);

            //            /// update ClockGauge's json -- causes a redraw with new chart object
            //            /// 
            //            chartJson = Json;
        }

    }
}

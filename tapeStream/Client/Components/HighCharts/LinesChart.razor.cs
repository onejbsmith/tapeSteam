#define tracing
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Client.Data;

namespace tapeStream.Client.Components.HighCharts
{
    public partial class LinesChart
    {

        static string chartJson = "";
        static string id = "LinesChart";
        static string chartJsFilename = $"js/highcharts/{id}.chart.js?id={DateTime.Now.ToOADate()}";
        static bool redraw = false;

        static LinesChartData.Rootobject chart = new LinesChartData.Rootobject();

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
            Console.WriteLine("1. LinesChart getChartJson");
            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
            Console.WriteLine(jsonResponse); /// to capture the chart object's json from js
#endif

            chart = JsonSerializer.Deserialize<LinesChartData.Rootobject>(jsonResponse);
            JsConsole.JsConsole.GroupTable(jsruntime, chart, $"{id} getChartJson chart");

            /// We set some static chart Properties here and pass back to js



            //chart.plotOptions.series.pointWidth = 100;

            redraw = true;
            chartJson = JsonSerializer.Serialize<LinesChartData.Rootobject>(chart);

#if tracing
            Console.WriteLine("2. ClockGauge getChartJson");
#endif
            Console.WriteLine(chartJson); /// to capture the chart object's json from js
            await Task.Yield();
        }

    }
}

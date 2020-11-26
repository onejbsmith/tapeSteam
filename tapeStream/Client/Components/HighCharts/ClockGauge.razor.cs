

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
            Console.WriteLine("1. Columns getChartJson");
#endif

            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
            chart = JsonSerializer.Deserialize<ClockGaugeData.HighchartChart>(jsonResponse);

            /// We set some static chart Properties here and pass back to js



            //chart.plotOptions.series.pointWidth = 100;

            redraw = true;
            chartJson = JsonSerializer.Serialize<ClockGaugeData.HighchartChart>(chart);
            
#if tracing            
            Console.WriteLine("2. Columns getChartJson");
#endif
            Console.WriteLine(chartJson); /// to capture the chart object's json from js
            await Task.Yield();
        }
    }
}

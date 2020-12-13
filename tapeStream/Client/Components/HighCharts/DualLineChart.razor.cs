using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Client.Data;
using tapeStream.Shared.Data;

namespace tapeStream.Client.Components.HighCharts
{
    public partial class DualLineChart
 
    {

        [Parameter]
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _id;


        [Parameter]
        public List<RatioFrame> ratioFrames
        {
            get { return _ratioFrames; }
            set
            {
                _ratioFrames = value;
#if tracing
                Console.WriteLine($"1. {id} UpdateLinesChart");
#endif
                UpdateLinesChart(_ratioFrames);
            }
        }
        private List<RatioFrame> _ratioFrames;

        public static List<float> lstBuysRatios { get; set; }

        public static List<float> lstSellsRatios { get; set; }

        public static List<string> lstSvcTimes { get; set; } = new List<string>();

        public static string svcDate { get; set; }

        public static List<float> lstMarkPrices { get; set; }

        static string chartJson = "";
        static string idName = "DualLineChart";
        static string chartJsFilename = $"js/highcharts/{idName}.chart.js?id={DateTime.Now.ToOADate()}";
        static bool redraw = true;

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
            JsConsole.JsConsole.GroupCollapsed(jsruntime, $"0.1 {id} getChartJson");
            Console.WriteLine("getChartJson(string jsonResponse)"); /// to capture the chart object's json from js
            Console.WriteLine(jsonResponse); /// to capture the chart object's json from js
            JsConsole.JsConsole.GroupEnd(jsruntime);
#endif

            chart = JsonSerializer.Deserialize<LinesChartData.Rootobject>(jsonResponse);

            chart.subtitle.text = "Hello";// TDAChart.LongDateString;

#if tracing
            JsConsole.JsConsole.GroupCollapsed(jsruntime, $"0.2 {id} getChartJson");
            JsConsole.JsConsole.GroupTable(jsruntime, chart, $"{id} getChartJson chart");
            JsConsole.JsConsole.GroupEnd(jsruntime);
#endif

            /// We set some static chart Properties here and pass back to js

            //redraw = true;
            //chartJson = JsonSerializer.Serialize<LinesChartData.Rootobject>(chart);
            //StateHasChanged();

            //#if tracing
            //            Console.WriteLine($"2. {id} getChartJson");
            //            Console.WriteLine(chartJson); /// to capture the chart object's json from js
            //#endif
            await Task.Yield();
        }

        private async Task UpdateLinesChart(List<RatioFrame> ratioFrames)
        {
            try
            {
                await Chart_BuildSeriesData(ratioFrames);
            }
            catch (Exception ex)
            {
                JsConsole.JsConsole.Confirm(jsruntime, ex.ToString());
            }
        }



        private async Task Chart_BuildSeriesData(List<RatioFrame> ratioFrames)
        {
            if (chart == null || chart.series == null) return;

            List<string> categories = new List<string>();
            for (int i = 0; i < chart.series.Length; i++)
            {
                chart.series[i].data = new float?[ratioFrames.Count];
                for (int j = 0; j < ratioFrames.Count; j++)
                {
                    var frame = ratioFrames[j];
                    switch (i)
                    {
                        case 1:
                            chart.series[i].data[j] = (float)frame.buysRatio;
                            categories.Add(frame.dateTime.ToString());
                            break;
                        //case 1: chart.series[i].data[j] = (float)frame.markPrice; break;
                        case 0: chart.series[i].data[j] = (float)frame.sellsRatio; break;
                    }
                }
            }
            chart.xAxis[0].categories = categories.ToArray();
#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, chart, $"5. {id} Chart_BuildSeriesDataCSVs chart", false);
#endif             
            chartJson = JsonSerializer.Serialize<LinesChartData.Rootobject>(chart);

        }

        //redraw = true;


    }
}

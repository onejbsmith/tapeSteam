#define tracing

using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Client.Data.highcharts;
using static tapeStream.Client.Data.highcharts.HighCharts_Enums;
using static tapeStream.Client.Data.highcharts.Highcharts3DMarketColumnChartData;

namespace tapeStream.Client.Components
{
    public partial class Highcharts3DMarketColumnChart
    {
        string chartJsFilename = "js/highcharts/Highcharts3DMarketColumnChart.js";
        public Highchart highChart;
        public static HighchartChart market3DColumnChart;

        [Parameter]
        public string chartJson
        {
            get { return json; }
            set
            {
                if (value != json)
                {
                    json = value;
#if tracing
                    Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " + MethodBase.GetCurrentMethod().Name);
#endif                
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
#if tracing
                    Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " + MethodBase.GetCurrentMethod().Name);
#endif              
                }
            }
        }
        private string seriesJson;

        [Parameter]
        public DataUpdateType updateSeries { get; set; }

        static public event Action OnChartCreated;
        private static void ChartCreated() => OnChartCreated?.Invoke();

        protected override async Task OnInitializedAsync()
        {
#if tracing
            Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " + MethodBase.GetCurrentMethod().Name);
#endif 
            Highchart.OnChartCreated += async () => await Highchart_OnChartCreated();
            await Task.CompletedTask;
        }

        /// Give the js a reference back to this cs
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
#if tracing
            Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " + MethodBase.GetCurrentMethod().Name);
#endif           
            if (firstRender)
            {
                var dotNetReference = Microsoft.JSInterop.DotNetObjectReference.Create(this);
                await jsruntime.InvokeVoidAsync("Initialize", dotNetReference);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable("getChartJson")]
        public async Task getChartJson(string jsonResponse)
        {
#if tracing
            Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " + MethodBase.GetCurrentMethod().Name);
#endif

            market3DColumnChart = JsonSerializer.Deserialize<HighchartChart>(jsonResponse);

            /// We set some static chart Properties here and pass back to js
            //chart.title.text = ChartConfigure.seconds.ToString();
            //chart.chart.options3d.enabled = true;
            //chart.yAxis.title.text = "Size";
            //chart.plotOptions.series.pointWidth = 100;

            chartJson = JsonSerializer.Serialize<HighchartChart>(market3DColumnChart);
            /// initialize the cs chart object in the specific chart component
            Console.WriteLine($"Highchart getChartJson jsonResponse => " + jsonResponse);

            ChartCreated();

            StateHasChanged();
            await Task.Yield();
        }


        /// Capture the chart object's json from js
        private async Task Highchart_OnChartCreated()
        {
#if tracing
            Console.WriteLine(MethodBase.GetCurrentMethod().ReflectedType.Name + " => " + MethodBase.GetCurrentMethod().Name);
#endif
            ChartCreated();
            //if (!string.IsNullOrEmpty(market3DColumnChart.title.text))
            //{
            //    //chartJson = highChart.chartJson;
            //    //System.Console.WriteLine("Highcharts3DMarketColumnChart.Highchart_OnChartCreated  highChart.chartJson => "
            //    //    + highChart.chartJson);
            //    market3DColumnChart = JsonSerializer.Deserialize<HighchartChart>(highChart.chartJson);
            //    await base.InvokeAsync(ChartCreated);
            //    await base.InvokeAsync(StateHasChanged);
            //}

            StateHasChanged();
            await Task.Yield();
        }
    }
}

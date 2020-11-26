#define tracing
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
using tapeStream.Shared.Data;
using  tapeStream.Client.Data;

namespace tapeStream.Client.Components.HighCharts
{

    public partial class TimeRows
    {
        string id = "TimeRows";


        [Parameter]
        public string symbol { get; set; }


        [Parameter]
        public Dictionary<string, BookDataItem[]> bookData
        {
            get { return _bookData; }
            set
            {
                _bookData = value;
                ChartSetData(_bookData);
#if tracing

                Console.WriteLine("3. Columns ChartSetData");
#endif
            }
        }
        Dictionary<string, BookDataItem[]> _bookData = new Dictionary<string, BookDataItem[]>();

        string chartJsFilename = "js/TimeRows.js?x=22";

        static string chartJson = "";

        public int seconds { get; set; }

        string name = "not set";

        static TimeRowsChart chart  = new TimeRowsChart();

        static Dictionary<string, string> dictSeriesColor;

        static TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        string chartSeriesJson = "";

        static double maxSize;

        protected override async Task OnInitializedAsync()
        {
            var dotNetReference = DotNetObjectReference.Create(this);
            await jsruntime.InvokeVoidAsync("Initialize", dotNetReference, id);

            /// TO draw the chart
            //bookData = new Dictionary<string, BookDataItem[]>(); 

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
            chart = JsonSerializer.Deserialize<TimeRowsChart>(jsonResponse);

            /// We set some static chart Properties here and pass back to js
            chart.title.text = "My Chart";

            //chart.plotOptions.series.pointWidth = 100;

            chartJson = JsonSerializer.Serialize<TimeRowsChart>(chart);
#if tracing            
            Console.WriteLine("2. Columns getChartJson");
#endif
            Console.WriteLine(chartJson); /// to capture the chart object's json from js
            await Task.Yield();
        }

        private async Task ChartSetData(Dictionary<string, BookDataItem[]> bookDataItems)
        {

            try
            {
                chartJson = JsonSerializer.Serialize<TimeRowsChart>(chart);

            }
            catch { }
        }
    }
}

#undef tracing
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Client.Data;
using tapeStream.Shared;
using tapeStream.Shared.Data;
using tapeStream.Shared.Managers;

namespace tapeStream.Client.Components.HighCharts
{
    public partial class LinesChart
    {
        [Parameter]
        public bool showPrice { get; set; } = true;

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
                Console.WriteLine($"1. {id} Chart_Update");
#endif
                //if (chart == null) return;
                //var isNewChart = chart.series[0].data.Length == 0;
                if (_ratioFrames.Count == 0) return;
                //if (_ratioFrames.Count == 1 && chartJson!=null)
                //    Chart_AppendData(_ratioFrames[0]);
                //else
                    Chart_Update(_ratioFrames);
            }
        }
        private List<RatioFrame> _ratioFrames = new List<RatioFrame>();

        [Parameter]
        public string buysField
        {
            get { return _buysField; }
            set
            {
                _buysField = value;
                sellsField = _buysField.Replace("buys", "sells").Replace("asks", "bids");

            }
        }
        private string _buysField;

        public List<float> lstBuysRatios { get; set; }

        public List<float> lstSellsRatios { get; set; }

        public List<string> lstSvcTimes { get; set; } = new List<string>();

        public string svcDate { get; set; }

        public List<float> lstMarkPrices { get; set; }

        string chartJson = "";
        string chartSeriesJson = "";
        string idName = "LinesChart";
        string chartJsFilename = $"js/highcharts/LinesChart.chart.js?id={DateTime.Now.ToOADate()}";
        bool redraw = true;

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

            Chart_Initialize(jsonResponse);
           // StateHasChanged();

            //#if tracing
            //            Console.WriteLine($"2. {id} getChartJson");
            //            Console.WriteLine(chartJson); /// to capture the chart object's json from js
            //#endif
            await Task.Yield();
        }

        private void Chart_Initialize(string jsonResponse)
        {
            chart = JsonSerializer.Deserialize<LinesChartData.Rootobject>(jsonResponse);

            chart.subtitle.text = ratioFrames.First().dateTime.ToLongDateString();// TDAChart.LongDateString;
            chart.yAxis[0].gridLineWidth = 1;
            chart.yAxis[1].gridLineWidth = 1;




#if tracing
            JsConsole.JsConsole.GroupCollapsed(jsruntime, $"0.2 {id} getChartJson");
            JsConsole.JsConsole.GroupTable(jsruntime, chart, $"{id} getChartJson chart");
            JsConsole.JsConsole.GroupEnd(jsruntime);
#endif

            /// We set some static chart Properties here and pass back to js

            redraw = true;
            chartJson = JsonSerializer.Serialize<LinesChartData.Rootobject>(chart);
        }

        private async Task Chart_Update(List<RatioFrame> ratioFrames)
        {
            try
            {
                if (chart == null || chart.series == null) return;

                Chart_SetCategories(ratioFrames);

                await Chart_BuildSeriesData(ratioFrames);

                redraw = true;
                chartJson = JsonSerializer.Serialize<LinesChartData.Rootobject>(chart);
            }
            catch (Exception ex)
            {
                JsConsole.JsConsole.Confirm(jsruntime, ex.ToString());
            }
        }

        private async Task Chart_BuildSeriesData(List<RatioFrame> ratioFrames)
        {
#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, chart, $"5. {id} Chart_BuildSeriesDataCSVs chart", false);
#endif

            /// Replace current series data with new 
            /// This is brute force
            /// Ideally call a js method to add the new points to each series 
            /// and remove the stale points
            /// 
            /// Should only need to refesh whole series when the time axis is reset
            /// 


            /// Get the buys, sells and marks into float []
            /// 
            var _sellsField = SellsField(_buysField);



            var lstBuys = ratioFrames.Select(item => (float)Convert.ToDouble(item[_buysField])).ToList();
            var lstSells = ratioFrames.Select(item => (float)Convert.ToDouble(item[_sellsField])).ToList();
            var lstMarks = ratioFrames.Select(item => (float)Convert.ToDouble(item.markPrice)).ToList();

            chart.series[0].data = lstBuys.ToArray();
            chart.series[1].data = lstMarks.ToArray();
            chart.series[2].data = lstSells.ToArray();
            chart.series[0].color = CONSTANTS.asksColor;
            chart.series[2].color = CONSTANTS.bidsColor;
            chart.yAxis[0].gridLineWidth = 1;
            chart.yAxis[1].gridLineWidth = 1;


            await Task.CompletedTask;
        }

        private async Task Chart_AppendData(RatioFrame ratioFrame)
        {
            var lstData = new List<float> { (float)Convert.ToDouble(ratioFrame[buysField]), (float)Convert.ToDouble(ratioFrame.markPrice), (float)Convert.ToDouble(ratioFrame[sellsField]) };
            chartSeriesJson = JsonSerializer.Serialize<float[]>(lstData.ToArray());
        }

        private void Chart_SetCategories(List<RatioFrame> ratioFrames)
        {
            List<string> categories = new List<string>();
            foreach (var frame in ratioFrames)
                categories.Add(frame.dateTime.ToString("h:mm:ss"));

            chart.xAxis[0].categories = categories.ToArray();
        }

        //redraw = true;

        #region Chart Dialog and Title Stuff


        string sellsField = "sellsTradeSizes";

        //string buysTitle = $"Buys - {sumBuys}";
        //string sellsTitle = $"Sells - {sumSells}";

        bool dialogIsOpen = false;
        bool priceDialogIsOpen = false;

        MatChip[] selectedBuysChips = null;
        MatChip selectedBuyChip = null;

        MatChip[] selectedSellsChips = null;
        MatChip selectedSellChip = null;


        string chartTitle()
        {
            return TranslateChartTitle(string.Join(" ", buysField.SplitCamelCase().Split(" ").Skip(1)));
        }

        /// <summary>
        /// This shuld supply Y-Axis label and values format, too
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private string TranslateChartTitle(string v)
        {
            string s = "";
            switch (v)
            {
                case "Trade Sizes": s = "Time & Sales — Prints — The Tape"; break;
                case "Book Sizes": s = "Level II — The NASDAQ Book"; break;
                case "Price Count": s = "Level II Price Ranges"; break;
                case "Ratio": s = "Print Size / Book Size Ratio"; break;
                case "Below": s = "Prints Below Spread"; break;
                case "Above": s = "Prints Above Spread"; break;
            }
            return s;
        }

        void OpenDialog()
        {
            dialogIsOpen = true;
        }

        void OkClick()
        {
            dialogIsOpen = false;
        }

        string ProcessChips()
        {
            foreach (var chip in selectedBuysChips ?? new MatChip[0])
            {
                if ((chip.IsSelected))
                {
                    buysField = (string)chip.Value;
                    sellsField = SellsField(buysField);
                }
            }
            return "";
        }

        private string SellsField(string buysField)
        {
            return buysField.Replace("buys", "sells").Replace("asks", "bids"); ;
        }

        #endregion


    }
}


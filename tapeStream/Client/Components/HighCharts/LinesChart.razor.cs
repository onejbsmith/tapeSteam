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
using static tapeStream.Client.Data.LinesChartData;
using MathNet.Numerics;


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
        public List<RatioFrame[]> allRatioFrames { get; set; }

        [Parameter]
        public List<RatioFrame[]> ratioFrames
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
        private List<RatioFrame[]> _ratioFrames = new List<RatioFrame[]>();

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

        private string newBuysField;
        private string newSellsField;


        public List<float?> lstBuysRatios { get; set; }

        public List<float?> lstSellsRatios { get; set; }

        public List<string> lstSvcTimes { get; set; } = new List<string>();

        public string svcDate { get; set; }

        public List<float?> lstMarkPrices { get; set; }

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

            chart.subtitle.text = ratioFrames.First()[0].dateTime.ToLongDateString();// TDAChart.LongDateString;

            chart.yAxis[0].title.text = "";
            chart.yAxis[0].title.style.color = "black";



#if tracing
            JsConsole.JsConsole.GroupCollapsed(jsruntime, $"0.2 {id} getChartJson");
            JsConsole.JsConsole.GroupTable(jsruntime, chart, $"{id} getChartJson chart");
            JsConsole.JsConsole.GroupEnd(jsruntime);
#endif

            /// We set some static chart Properties here and pass back to js

            redraw = true;
            chartJson = JsonSerializer.Serialize<LinesChartData.Rootobject>(chart);
        }

        private async Task Chart_Update(List<RatioFrame[]> ratioFrames)
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

        private void Chart_SetCategories(List<RatioFrame[]> ratioFrames)
        {


            List<string> categories = new List<string>();
            foreach (var frame in ratioFrames)
            {
                categories.Add(frame[0].dateTime.ToString("h:mm:ss"));
            }
            chart.xAxis[0].categories = categories.ToArray();

            Chart_PlotBands(ratioFrames, categories);

        }

        private static void Chart_PlotBands(List<RatioFrame[]> ratioFrames, List<string> categories)
        {
            try
            {
                var index = 0;
                /// Pick up even minutes for plot bands
                List<DateTime> lstEvenMinutesStart = new List<DateTime>();
                List<DateTime> lstEvenMinutesEnd = new List<DateTime>();
                var prevDateTime = DateTime.Now;
                foreach (var frames in ratioFrames)
                {
                    var frame = frames[0];
                    var isNewEvenMinute = frame.dateTime.Minute % 2 == 0 && frame.dateTime.Minute != prevDateTime.Minute;

                    if (isNewEvenMinute)
                    {
                        lstEvenMinutesStart.Add(frame.dateTime);
                    }
                    var isNewOddMinute = frame.dateTime.Minute % 2 == 1 && frame.dateTime.Minute != prevDateTime.Minute;
                    if (isNewOddMinute)
                        lstEvenMinutesEnd.Add(prevDateTime);

                    prevDateTime = frame.dateTime;
                    index += 1;
                }
                if (lstEvenMinutesStart.Count > lstEvenMinutesEnd.Count)
                    lstEvenMinutesEnd.Add(prevDateTime);


                var evenMinutesPlotBands = new List<LinesChartData.Plotband>();
                for (int i = 0; i < lstEvenMinutesEnd.Count; i++)
                {
                    if (lstEvenMinutesStart == null || i > lstEvenMinutesStart.Count) return;
                    var evenMinute = lstEvenMinutesStart[i];
                    var evenMinuteEnd = lstEvenMinutesEnd[i];
                    var plotBand = new LinesChartData.Plotband()
                    {
                        from = categories.IndexOf(evenMinute.ToString("h:mm:ss")),
                        to = categories.IndexOf(evenMinuteEnd.ToString("h:mm:ss")),
                        color = "lavender",
                        label = new LabelPlotline()
                        {
                            text = evenMinute.ToString("h:mm"),
                            style = new CSSObject()
                            {
                                fontSize = "15px"
                            }
                        }
                    };
                    evenMinutesPlotBands.Add(plotBand);
                }

                chart.xAxis[0].plotBands = evenMinutesPlotBands.ToArray();
            }
            catch { }
        }
        private async Task Chart_BuildSeriesData(List<RatioFrame[]> ratioFrames)
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


            /// Get the buys, sells and marks into float? []
            /// 
            var _sellsField = SellsField(_buysField);

            var lstBuys = ratioFrames.Select(item => (float?)Convert.ToDouble(item[0][_buysField])).ToList();
            var lstSells = ratioFrames.Select(item => (float?)Convert.ToDouble(item[0][_sellsField])).ToList();

            var lstMarks = ratioFrames.Select(item => (float?)Convert.ToDouble(item[0].markPrice)).ToList();

            var minMark = allRatioFrames.Min(item => (float?)Convert.ToDouble(item[0].markPrice));
            var lstMinMarks = ratioFrames.Select(item => minMark).ToList();
            var maxMark = allRatioFrames.Max(item => (float?)Convert.ToDouble(item[0].markPrice));
            var lstMaxMarks = ratioFrames.Select(item => maxMark).ToList();

            var lstBuysRs = ratioFrames.Select(item => (float?)Convert.ToDouble(item[1][_buysField])).ToList(); ;
            var lstSellsRs = ratioFrames.Select(item => (float?)Convert.ToDouble(item[1][sellsField])).ToList(); ;


            if (TDABook.showRegressionCurves==true)
            {
                var lstBuysD = ratioFrames.Select(item => (double)Convert.ToDouble(item[0][_buysField])).ToList();
                var lstSellsD = ratioFrames.Select(item => (double)Convert.ToDouble(item[0][_sellsField])).ToList();

                var xData = ratioFrames.Select(item => (double)ratioFrames.IndexOf(item)).ToArray();
                /// Fitting a fifth degree polynomial to the data can have up to 4 curves
                var cb = Fit.Polynomial(xData, lstBuysD.ToArray(), 5).Select(t => (float?)t).ToList();
                var cs = Fit.Polynomial(xData, lstSellsD.ToArray(), 5).Select(t => (float?)t).ToList();

                chart.series[0].data = xData.Select(x=> (float?)cb[0] + (float?)(5 * cb[1]*x) + (float?)(10*cb[2]*(x*x)) + (float?)(10*cb[3] * (x*x*x)) + (float?)(5 * cb[4] * (x * x * x* x)) + (float?)( cb[5] * (x * x * x * x * x))).ToArray();
                chart.series[2].data = xData.Select(x=> (float?)cs[0] + (float?)(5 * cs[1]*x) + (float?)(10* cs[2]*(x*x)) + (float?)(10*cs[3] * (x*x*x)) + (float?)(5 * cs[4] * (x * x * x * x)) + (float?)(cs[5] * (x * x * x * x * x))).ToArray();
            }
            else
            {
                chart.series[0].data = lstBuys.ToArray();
                chart.series[2].data = lstSells.ToArray();
            }

            chart.series[1].data = lstMarks.ToArray();
            //chart.series[3].data = lstBuysRs.ToArray();
            //chart.series[4].data = lstSellsRs.ToArray();
            chart.series[0].color = CONSTANTS.buysColor;
            chart.series[2].color = CONSTANTS.sellsColor;
            //chart.series[3].color = CONSTANTS.buysColor;
            //chart.series[4].color = CONSTANTS.sellsColor;

            if (buysField.StartsWith("asks") || buysField == "buysPriceCount" || buysField.EndsWith("Ratio"))
            {
                chart.series[0].color = CONSTANTS.asksColor;
                chart.series[2].color = CONSTANTS.bidsColor;

                chart.series[0].name = "Asks";
                chart.series[2].name = "Bids";
                //chart.series[3].color = CONSTANTS.asksColor;
                //chart.series[4].color = CONSTANTS.bidsColor;
            }

            chart.yAxis[0].gridLineWidth = 1;
            chart.yAxis[1].gridLineWidth = 1;
            chart.yAxis[2].gridLineWidth = 1;
            chart.xAxis[0].gridLineWidth = 1;
            chart.yAxis[0].title.text = ((float)(lstMarks.LastOrDefault())).ToString("n2");


            Chart_PlotLines(minMark, maxMark);

            var maxSells = ratioFrames.Max(item => (float?)Convert.ToDouble(item[0][buysField]));
            if (maxSells > 10000)
                chart.yAxis[1].labels.format = "{value:,.0f}";
            else
                chart.yAxis[1].labels.format = "{value}";

            var date = ratioFrames.First()[0].dateTime.ToLongDateString();
            try
            {
                var predSellsR = Convert.ToDouble(ratioFrames.SkipLast(Math.Min(10, ratioFrames.Count)).TakeLast(TDABook.ratiosBack).Average(item => Convert.ToDouble(item[1][sellsField]))).ToString("n0");
                var predBuysR = Convert.ToDouble(ratioFrames.SkipLast(Math.Min(10, ratioFrames.Count)).TakeLast(TDABook.ratiosBack).Average(item => Convert.ToDouble(item[1][buysField]))).ToString("n0");
                var sellsR = Convert.ToDouble(ratioFrames.Average(item => Convert.ToDouble(item[1][sellsField]))).ToString("n0");
                var buysR = Convert.ToDouble(ratioFrames.Average(item => Convert.ToDouble(item[1][buysField]))).ToString("n0");

                var sumValues = Convert.ToDouble(ratioFrames.Average(item => Convert.ToDouble(item[2][sellsField]))).ToString("n0");
                var diffR = Convert.ToDouble(ratioFrames.Average(item => Convert.ToDouble(item[2][buysField]))).ToString("n0");

                chart.title.text = $"Diff r All {ratioFrames.Count} secs: {diffR}  Sum All: {sumValues} | All ({TDABook.ratiosBack} secs) r [ {buysR} {sellsR} ]  | Prediction:(ahead {10} secs) r  [ {predBuysR} {predSellsR} ]"; ;// TDAChart.LongDateString;
            }
            catch { }

            await Task.CompletedTask;
        }

        private static void Chart_PlotLines(float? minMark, float? maxMark)
        {
            chart.yAxis[0].plotLines = new LinesChartData.Plotline[]
            {
                new LinesChartData.Plotline()
                {
                    color="red",
                    dashStyle = "shortdot",
                    value = (decimal)minMark,
                    label= new LinesChartData.LabelPlotline()
                        {
                            text = ((float)minMark).ToString("n2")
                        },
                    width=2
                },
                new LinesChartData.Plotline()
                {
                    color="green",
                    dashStyle = "shortdot",
                    value = (decimal)maxMark,
                    label= new LinesChartData.LabelPlotline()
                        {
                            text = ((float)maxMark).ToString("n2")
                        },
                    width=2
                }
            };
        }

        private async Task Chart_AppendData(RatioFrame ratioFrame)
        {
            var lstData = new List<float?> { (float?)Convert.ToDouble(ratioFrame[buysField]), (float?)Convert.ToDouble(ratioFrame.markPrice), (float?)Convert.ToDouble(ratioFrame[sellsField]) };
            chartSeriesJson = JsonSerializer.Serialize<float?[]>(lstData.ToArray());
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

        void Dialog_Open()
        {
            dialogIsOpen = true;
        }

        void Dialog_OkClick()
        {
            dialogIsOpen = false;
            buysField = newBuysField;
            StateHasChanged();
        }

        string ProcessChips()
        {
            foreach (var chip in selectedBuysChips ?? new MatChip[0])
            {
                if ((chip.IsSelected))
                {
                    newBuysField = (string)chip.Value;
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


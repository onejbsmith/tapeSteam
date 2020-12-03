#undef UsingSignalHub
#define dev
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using tapeStream.Client.Data;
using tapeStream.Shared.Data;
using tapeStream.Shared;
using tapeStream.Shared.Services;
using MatBlazor;
using Threader = System.Threading;
using tapeStream.Client.Components;
using Microsoft.JSInterop;

namespace tapeStream.Client.Pages
{
    public partial class ChartTest
    {
        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] ChartService chartService { get; set; }

#if AllComponents
        [Inject] BookPieChartsService bookPieChartsService { get; set; }
        [Inject] PrintsPieChartService printsPieChartService { get; set; }
        [Inject] PrintsLineChartService printsLineChartService { get; set; }
#endif



        #region Variables
#if AllComponents
        MatChip selectedChip;
        static int refreshPrintLineChartInSeconds = 5;
        static int refreshBookPieChartsInSeconds = 2;
        static int refreshPrintsPieChartsInSeconds = 1;
        static int refreshBookColumnsChartsInSeconds = 2;

        Timer timerBookPieCharts = new Timer(500);
        Timer timerPrintsPieCharts = new Timer(2000);
        Timer timerPrintsLineCharts = new Timer(5000);
#endif
        Timer timerBookColumnsCharts = new Timer(500);


        #region Chart Data in order of appearance
#if AllComponents
        /// <summary>
        /// PrintLinesGauge___Copy
        /// </summary>
        public static Dictionary<string, DataItem[]> dictAllLinePoints =
                new Dictionary<string, DataItem[]>()
                {
                    { "rawGaugesCombined", new DataItem[0] } ,
                    { "staticValueMinus7", new DataItem[0] } ,
                    { "staticValue0", new DataItem[0] } ,
                    { "staticValue7", new DataItem[0] } ,
                    { "movingAverage30sec", new DataItem[0] } ,
                    { "movingAverage5min", new DataItem[0] } ,
                    { "average10min", new DataItem[0] }
                };

        /// <summary>
        /// PrintArcGaugeChart
        /// </summary>
        public static double lastCombinedRedGreenValue = -1;

        /// <summary>
        /// PrintPieChart
        /// </summary>
        public Dictionary<string, DataItem[]> printsData
        {
            get { return _printsData; }
            set
            {
                _printsData = value;
                //getPrintsData();
            }
        }
        private Dictionary<string, DataItem[]> _printsData;

        /// <summary>
        /// Used to hold bookData by seconds 
        /// BookPieChart
        /// </summary>
        public Dictionary<string, BookDataItem[]> bookDataDict
        {
            get { return _bookDataDict; }
            set
            {
                _bookDataDict = value;
            }
        }
        private Dictionary<string, BookDataItem[]> _bookDataDict;

        /// <summary>
        /// BookGuageChart
        /// </summary>
        public BookDataItem[] bookData
        {
            get
            {
                return _bookData;
            }
            set
            {
                _bookData = value;
            }
        }
        private BookDataItem[] _bookData;

#endif
        /// <summary>
        /// BookColumnsChart
        /// </summary>
        public Dictionary<string, BookDataItem[]> bookColData
        {
            get { return _bookColData; }
            set
            {
                _bookColData = value;
            }
        }
        Dictionary<string, BookDataItem[]> _bookColData;

        #endregion


        KeyValuePair<DateTime, double>[] latestGaugeValues;
        int moduloPrints = 1;
        int moduloBook = 1;
        int lstTimeSales = 0;



        #endregion


        #region Page Methods
        protected override async Task OnInitializedAsync()
        {

            /// Init parameters so don't get "null" error
            await InitializeData();

            /// For the Hub Monitor
#if UsingSignalHub
            await InitHub();
#endif

            InitializeTimers();

            //CheckThreadpool();
        }

        //private static void CheckThreadpool()
        //{
        //    int workerThreads = 10;
        //    int portThreads;

        //    Threader.ThreadPool.SetMaxThreads(workerThreads, 100);

        //    Threader.ThreadPool.GetMaxThreads(out workerThreads, out portThreads);
        //    Debug.WriteLine("\nMaximum worker threads: \t{0}" +
        //        "\nMaximum completion port threads: {1}",
        //        workerThreads, portThreads);
        //}

        private void InitializeTimers()
        {
            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender, e);
            timerBookColumnsCharts.Start();

#if AllComponents
           //timer.Elapsed += async (sender, e) => await Timer_ElapsedAsync();
            timerBookPieCharts.Elapsed += async (sender, e) => await TimerBookPieCharts_Elapsed(sender, e);
            timerPrintsPieCharts.Elapsed += async (sender, e) => await TimerPrintsPieCharts_Elapsed(sender, e);
            timerPrintsLineCharts.Elapsed += async (sender, e) => await TimerPrintsLineCharts_Elapsed(sender, e);

            //timerBookPieCharts.Start();
            timerPrintsPieCharts.Start();

            //timerPrintsLineCharts.Start();
#endif
        }


        private async Task InitializeData()
        {
#if AllComponents
            dictTopicCounts = new Dictionary<string, int>();
            foreach (var x in CONSTANTS.valuesName)
                dictTopicCounts.Add(x, 0);

            /// For T&S Pies by seconds
            TDAPrints.dictPies = new Dictionary<string, DataItem[]>();
            foreach (var i in CONSTANTS.printSeconds)
                TDAPrints.dictPies.Add(i.ToString(), TDAPrints.newData);

            printsData = TDAPrints.dictPies;

            /// For the Book Pies
            bookDataDict = new Dictionary<string, BookDataItem[]>();
            foreach (var i in CONSTANTS.printSeconds)
                bookDataDict.Add(i.ToString(), new BookDataItem[]
                    {  new BookDataItem { Price = 0, Size = 0, time = now, dateTime = DateTime.Now }
                    });

            /// For the Book Agg Pie
            bookData = new BookDataItem[]
                     {  new BookDataItem { Price = 0, Size = 0, time = now, dateTime = DateTime.Now }
                     };
#endif
            /// For the Book Columns
            bookColData = CONSTANTS.newBookColumnsData;

            //await AppendLineChartData();
        }

        private async Task TimerBookColumnsCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            await GetBookColumnsData(ChartConfigure.seconds);
            //await jsruntime.InvokeVoidAsync("Dump", TDAChart.lastCandles.Dumps(), "TDAChart.lastCandles");

        }

        private async Task GetBookColumnsData(int seconds)
        {
            timerBookColumnsCharts.Stop();
            await Task.Yield();
            bookColData = await bookColumnsService.getBookColumnsData(seconds);
            TDAChart.bollingerBands = await chartService.getBollingerBands();
            TDAChart.lastCandle = await chartService.GetTDAChartLastCandle(0);
            TDAChart.svcDateTimeRaw = await chartService.GetSvcDate();
            TDAChart.svcDateTimeRaw = TDAChart.svcDateTimeRaw.Replace("\"", "");
            TDAChart.svcDateTime = Convert.ToDateTime(TDAChart.svcDateTimeRaw);
            StateHasChanged();

            await Task.Delay(100);
            //Debug.WriteLine("2. BookColumnsCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());

            timerBookColumnsCharts.Start();
            await Task.CompletedTask;
        }

        #endregion
#if UsingSignalHub
        #region Hub Methods


        public DateTime GetServiceTime(JToken svcJsonObject, string timeField = "timestamp")
        {
            var svcEpochTime = Convert.ToInt64(svcJsonObject[timeField]);
            var svcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0)
                .AddMilliseconds(svcEpochTime).ToLocalTime();
            clock = svcDateTime.ToString(clockFormat);
            return svcDateTime;
        }
        #endregion

#endif

    }
}

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

namespace tapeStream.Client.Pages
{
    public partial class Index
    {
        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }

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

        static string clockFormat = "h:mm:ss MMM d, yyyy";


        KeyValuePair<DateTime, double>[] latestGaugeValues;
        int moduloPrints = 1;
        int moduloBook = 1;


        bool logHub;

        int lstTimeSales = 0;

        private string topicInput;
        private string messageInput;

        string clock;
        string logTopics;

        StringBuilder logTopicsb = new StringBuilder();  /// Called from Javascript
        Dictionary<string, int> dictTopicCounts = new Dictionary<string, int>();
        private HubConnection hubConnection;
        private List<string> messages = new List<string>();



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
        //    Console.WriteLine("\nMaximum worker threads: \t{0}" +
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
            await GetBookColumnsData(BookColumnsChart.seconds);
        }

        private async Task GetBookColumnsData(int seconds)
        {
            timerBookColumnsCharts.Stop();
            await Task.Yield();
            bookColData = await bookColumnsService.getBookColumnsData(seconds);
            await Task.Delay(100);
            //Console.WriteLine("2. BookColumnsCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
            StateHasChanged();
            timerBookColumnsCharts.Start();
            await Task.CompletedTask;
        }

#if AllComponents
        private async Task TimerPrintsPieCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerPrintsPieCharts.Stop();
            await Task.Yield();
            lastCombinedRedGreenValue = await printsPieChartService.GetPrintsGaugeScore();
            await Task.Delay(100);
            printsData = await printsPieChartService.GetPrintsPies();
            await Task.Delay(100);
            Console.WriteLine("1. PrintsPieCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
            StateHasChanged();
            timerPrintsPieCharts.Start();
            await Task.CompletedTask;
        }

        private async Task TimerPrintsLineCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerPrintsLineCharts.Stop();
            await Task.Yield();
            await AppendLineChartData();
            await Task.Delay(100);
            Console.WriteLine("3. PrintsLineCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
            StateHasChanged();
            timerPrintsLineCharts.Start();
            await Task.CompletedTask;
        }

        private async Task AppendLineChartData()
        {
            try
            {
                //timerBookPieCharts.Interval=         
                await Task.Yield();
                var dictNewLinePoints = await printsLineChartService.getPrintsLineChartData(600);
                await Task.Delay(100);

                foreach (var name in CONSTANTS.lineNames)
                {
                    await Task.Yield();
                    if (dictNewLinePoints.ContainsKey(name))
                    {
                        var points = dictAllLinePoints[name].ToList();
                        points.AddRange(dictNewLinePoints[name]);
                        dictAllLinePoints[name] = points.ToArray();
                    }
                }
            }
            catch { }
            await Task.CompletedTask;
        }

        private async Task TimerBookPieCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerBookPieCharts.Stop();
            await Task.Yield();
            bookDataDict = await bookPieChartsService.getBookPiesData();
            await Task.Delay(100);
            bookData = await bookPieChartsService.getBookCompositePieData(1);
            await Task.Delay(100);
            Console.WriteLine("4. BookPieCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
            StateHasChanged();
            timerBookPieCharts.Start();
            await Task.CompletedTask;
        }


#endif
        #region old hub subs

        /// Setup a hub Url
        /// 

        /// Setup a callback for Receiving messages from hub
        /// "NASDAQ_BOOK", "TIMESALE_EQUITY", "CHART_EQUITY", "OPTION", "QUOTE","ACTIVES"
        /// 

        //hubConnection.On("BookColsData", (Action<string, string>)(async (topic, message) =>
        //{
        //    Receive(topic, message);
        //    /// The data for this feed is kept on the server and updated from the server
        //    /// so we update the components with data directly from the server
        //    /// 
        //    bookColData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, BookDataItem[]>>(message);
        //    StateHasChanged();
        //}));

        //hubConnection.On("BookPiesData", (Action<string, string>)(async (topic, message) =>
        //{
        //    Receive(topic, message);
        //    /// The data for this feed is kept on the server and updated from the server
        //    /// so we update the components with data directly from the server
        //    /// 

        //    StateHasChanged();
        //}));

        //hubConnection.On("BookBigPieData", (Action<string, string>)(async (topic, message) =>
        //{
        //    Receive(topic, message);
        //    /// The data for this feed is kept on the server and updated from the server
        //    /// so we update the components with data directly from the server
        //    /// 

        //    StateHasChanged();
        //}));

        //hubConnection.On("PrintsPies", (Action<string, string>)(async (topic, message) =>
        //{
        //    Receive(topic, message);
        //    ///// The data for this feed is kept on the server and updated from the server
        //    ///// so we update the components with data directly from the server
        //    ///// 
        //    //printsData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, DataItem[]>>(message);

        //    StateHasChanged();
        //}));

        //hubConnection.On("GaugeScore", (Action<string, string>)(async (topic, message) =>
        //{
        //    Receive(topic, message);
        //    ///// The data for this feed is kept on the client and added to from the server
        //    ///// so we update the components from data in TDAStreamerData
        //    ///// 
        //    ///// Get one X,Y pair from the server
        //    //KeyValuePair<DateTime, double> pair
        //    //    = System.Text.Json.JsonSerializer.Deserialize<KeyValuePair<DateTime, double>>(message);
        //    ///// Remove all stale X,Y values and add new pair
        //    //TDAStreamerData.gaugeValues.RemoveAll((key, val) => key < DateTime.Now.AddSeconds(-600));
        //    //TDAStreamerData.gaugeValues.Add(pair.Key, pair.Value);
        //    ///// Pass the last pair to the Arc Gauge 
        //    //lastCombinedRedGreenValue = (int)pair.Value;
        //    ///// Pass the list of X,Y values to the Line Chart
        //    //gaugeValues = TDAStreamerData.gaugeValues;

        //    ///// Do we need to do this now ?
        //    ////TDAStreamerData.TimeSalesStatusChanged();

        //    StateHasChanged();
        //}));

        #endregion

#if IncludeAllTDA_Feeds
            hubConnection.On("NASDAQ_BOOK", (Action<string, string>)(async (topic, message) =>
            {
                Receive(topic, message);
                await TDAStreamerData.captureTdaServiceData(message, moduloPrints);
            }));

            hubConnection.On("TIMESALE_EQUITY", (Action<string, string>)(async (topic, message) =>
            {
                Receive(topic, message);
            // await TDAStreamerData.captureTdaServiceData(message, moduloPrints);
        }));

            hubConnection.On("CHART_EQUITY", (Action<string, string>)(async (topic, message) =>
            {
                Receive(topic, message);
                await TDAStreamerData.captureTdaServiceData(message, moduloPrints);
            }));
            hubConnection.On("OPTION", (Action<string, string>)(async (topic, message) =>
            {
                Receive(topic, message);
                await TDAStreamerData.captureTdaServiceData(message, moduloPrints);

            }));
            hubConnection.On("QUOTE", (Action<string, string>)(async (topic, message) =>
            {
                Receive(topic, message);
                await TDAStreamerData.captureTdaServiceData(message, moduloPrints);

            }));
            hubConnection.On("ACTIVES_NYSE", (Action<string, string>)(async (topic, message) =>
            {
                Receive(topic, message);
                await TDAStreamerData.captureTdaServiceData(message, moduloPrints);

            }));

            hubConnection.On("ACTIVES_NASDAQ", (Action<string, string>)(async (topic, message) =>
            {
                Receive(topic, message);
                await TDAStreamerData.captureTdaServiceData(message, moduloPrints);

            }));
            hubConnection.On("ACTIVES_OPTIONS", (Action<string, string>)(async (topic, message) =>
            {
                Receive(topic, message);
                await TDAStreamerData.captureTdaServiceData(message, moduloPrints);
            }));
            /// Start a Connection to the hub
            /// 
       
#endif

        // <summary>
        /// Process message received from Hub
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        #endregion

#if UsingSignalHub
        private async Task InitHub()
        {
            /// Init the SignalR Hub
            hubConnection = new HubConnectionBuilder()
                 .WithUrl("http://tapestream.com/tdahub")
                 .Build();

            /// Set up Hub Subscriptions -- Don't really need any anymore
            /// Perhaps get messages of counts? or ,essage to refresh to avoid using a timer 
            /// await SetupHub();

            await hubConnection.StartAsync();

            /// Show Hub Status in lamp color
            var color = IsConnected ? "green" : "red";
            TDAStreamerData.hubStatus = $"./images/{color}.gif";

        }


        private async Task SetupHub()
        {
            hubConnection.On("TimeAndSales", (Action<string, string>)(async (topic, message) =>
            {

                Receive(topic, message);
                //var timeAndSales = System.Text.Json.JsonSerializer.Deserialize<TimeSales_Content>(message);
                //DateTime retention = DateTime.Now.AddSeconds(-600);
                //TDAStreamerData.timeSales[symbol].RemoveAll(t => t.TimeDate < retention);
                //TDAStreamerData.timeSales[symbol].Add(timeAndSales);
                //lstTimeSales = TDAStreamerData.timeSales[symbol].Count;

                //await Task.Yield();
                //if (lstTimeSales % 10 == 0)
                //{
                //    lastCombinedRedGreenValue = (int)TDAPrints.GetPrintsGaugeScore(symbol);
                //    //printsData = TDAPrints.dictPies;
                //}
                StateHasChanged();

                await Task.CompletedTask;
            }));


            hubConnection.On("BookColsData", (Action<string, string>)(async (topic, message) =>
           {
               Receive(topic, message);
               ///// The data for this feed is kept on the server and updated from the server
               ///// so we update the components with data directly from the server
               ///// 
               //bookColData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, BookDataItem[]>>(message);
               //StateHasChanged();
               await Task.CompletedTask;
           }));

        }
        #region Hub Methods


        public DateTime GetServiceTime(JToken svcJsonObject, string timeField = "timestamp")
        {
            var svcEpochTime = Convert.ToInt64(svcJsonObject[timeField]);
            var svcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0)
                .AddMilliseconds(svcEpochTime).ToLocalTime();
            clock = svcDateTime.ToString(clockFormat);
            return svcDateTime;
        }

        /// <summary>
        /// Runs once, when this page is first loaded
        /// </summary>
        /// <returns></returns>
        /// 



        void Receive(string topic, string content)
        {
            // Show the topic text (last 1000 lines)
            logTopicsb.Insert(0, "\n" + topic + ":" + content.Replace("\r", "").Replace("\n", ""));
            logTopics = string.Join('\n', logTopicsb.ToString().Split('\n').Take(10));

            // Update topic's Stats count
            dictTopicCounts[topic] += 1;

            //var svcJsonObject = JObject.Parse(content);
            //var svcName = svcJsonObject["service"].ToString();
            //var contents = svcJsonObject["content"];
            ////var timeStamp = Convert.ToInt64(svcJsonObject["timestamp"]);
            //GetServiceTime(svcJsonObject);

            clock = DateTime.Now.ToString(clockFormat);

            StateHasChanged();
        }
        /// <summary>
        /// Method for this client to Send a message to the hub
        /// </summary>
        /// <returns></returns>
        /// 
        Task Send() =>
            hubConnection.SendAsync("SendMessage", topicInput, messageInput);

        /// <summary>
        /// Method to test if hub connection is alive
        /// </summary>
        /// 

        public bool IsConnected =>
            hubConnection != null
            && hubConnection.State == HubConnectionState.Connected;

        /// <summary>
        /// Method that will shut down hub if this page is closed
        /// </summary>
        /// 
        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        #endregion

#endif

    }
}


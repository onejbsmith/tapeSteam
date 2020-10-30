﻿using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using tapeStream.Client.Data;
using tapeStream.Data;
using tapeStream.Shared;
using System.Timers;
using Microsoft.AspNetCore.Components;
using tapeStream.Shared.Services;

namespace tapeStream.Client.Pages
{
    public partial class Index
    {
        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] BookPieChartsService bookPieChartsService { get; set; }
        [Inject] PrintsLineChartService printsLineChartService { get; set; }
        [Inject] PrintsPieChartService printsPieChartService { get; set; }


        Timer timerBookPieCharts = new Timer(1000);
        Timer timerPrintsPieCharts = new Timer(1000);
        Timer timerBookColumnsCharts = new Timer(1000);
        Timer timerPrintsLineCharts = new Timer(1000);

        #region Variables
        public Dictionary<string, BookDataItem[]> bookColData
        {
            get { return _bookColData; }
            set
            {
                _bookColData = value;
            }
        }

        Dictionary<string, BookDataItem[]> _bookColData;

        private BookDataItem[] _bookData;
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
        /// <summary>
        /// Used to hold bookData by seconds 
        /// </summary>
        public Dictionary<int, BookDataItem[]> bookDataDict
        {
            get { return _bookDataDict; }
            set
            {
                _bookDataDict = value;
            }
        }
        private Dictionary<int, BookDataItem[]> _bookDataDict;


        public Dictionary<int, DataItem[]> printsData
        {
            get { return _printsData; }
            set
            {
                _printsData = value;
                //getPrintsData();
            }
        }
        private Dictionary<int, DataItem[]> _printsData;

        static string clockFormat = "h:mm:ss MMM d, yyyy";

        int lastCombinedRedGreenValue = -1;
        KeyValuePair<DateTime, double>[] latestGaugeValues;
        int moduloPrints = 1;
        int moduloBook = 1;
        string clock;
        bool logHub;
        string logTopics;
        StringBuilder logTopicsb = new StringBuilder();  /// Called from Javascript
        Dictionary<string, int> dictTopicCounts = new Dictionary<string, int>();
        private HubConnection hubConnection;
        private List<string> messages = new List<string>();
        private string topicInput;
        private string messageInput;

        public DataItem[] rawGaugesCombined { get; set; }
        public static Dictionary<DateTime, double> gaugeValues { get; set; } 
            = new Dictionary<DateTime, double>();

        int lstTimeSales = 0;
        #endregion


        #region Page Methods
        protected override async Task OnInitializedAsync()
        {
            /// Init parameters so don't get "null" error
           /// For the Hub Monitor
            dictTopicCounts = new Dictionary<string, int>();
            foreach (var x in CONSTANTS.valuesName)
                dictTopicCounts.Add(x, 0);

            /// For T&S Pies by seconds
            TDAPrints.dictPies = new Dictionary<int, DataItem[]>();
            foreach (var i in CONSTANTS.printSeconds)
                TDAPrints.dictPies.Add(i, TDAPrints.newData);

            printsData = TDAPrints.dictPies;

            /// For the Book Pies
            bookDataDict = new Dictionary<int, BookDataItem[]>();
            foreach (var i in CONSTANTS.printSeconds)
                bookDataDict.Add(i, new BookDataItem[]
                    {  new BookDataItem { Price =0, Size=0, time=DateTime.Now }
                    });

             /// For the Book Agg Pie
           bookData = new BookDataItem[]
                    {  new BookDataItem { Price =0, Size=0, time=DateTime.Now }
                    };

            /// For the Book Columns
            bookColData = new Dictionary<string, BookDataItem[]>()
            { { "bids", new BookDataItem[] {new BookDataItem{ Price =0, Size=0, time=DateTime.Now } }},
              { "asks", new BookDataItem[] {new BookDataItem{ Price =0, Size=0, time=DateTime.Now } }}
            };

            /// Init the SignalR Hub
            hubConnection = new HubConnectionBuilder()
                 .WithUrl("https://localhost:44367/tdahub")
                 .Build();

            /// Set up Hub Subscriptions -- Don't really need any anymore
            /// Perhaps get messages of counts? or ,essage to refresh to avoid using a timer 
            /// await SetupHub();

            await hubConnection.StartAsync();

            /// Show Hub Status in lamp color
            var color = IsConnected ? "green" : "red";
            TDAStreamerData.hubStatus = $"./images/{color}.gif";

            //timer.Elapsed += async (sender, e) => await Timer_ElapsedAsync();
            timerBookPieCharts.Elapsed += async (sender,e) => await TimerBookPieCharts_Elapsed(sender,e);
            timerPrintsPieCharts.Elapsed += async (sender, e) => await TimerPrintsPieCharts_Elapsed(sender,e);
            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender,e);
            timerPrintsLineCharts.Elapsed += async (sender, e) => await TimerPrintsLineCharts_Elapsed(sender,e);

            timerBookPieCharts.Start();
            timerPrintsPieCharts.Start();
            timerBookColumnsCharts.Start();
            timerPrintsLineCharts.Start();
        }

        private async Task TimerPrintsLineCharts_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        private async Task TimerBookColumnsCharts_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        private async Task TimerPrintsPieCharts_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        private async Task TimerBookPieCharts_Elapsed(object sender, ElapsedEventArgs e)
        {

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

    }
}


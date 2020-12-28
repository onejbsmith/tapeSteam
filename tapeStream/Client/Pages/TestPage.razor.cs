#define tracing
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Shared;
using tapeStream.Shared.Data;
using Microsoft.AspNetCore.Components;
using tapeStream.Shared.Services;
using System.Timers;
using tapeStream.Client.Components;
using Microsoft.AspNetCore.Http.Connections;
using tapeStream.Client.Components.HighCharts;
using Microsoft.Extensions.Configuration;

namespace tapeStream.Client.Pages
{
    public partial class TestPage
    {
        #region Variables
        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject] Microsoft.JSInterop.IJSRuntime jsruntime { get; set; }

        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }


        string symbol = "QQQ";

        string hubUrl = "";

        public List<string> buyFields = new List<string>()
        {
            "buysTradeSizes",
            "asksBookSizes",
            "buysPriceCount",

            "buysBelow",
            "buysInSpread",
            "buysAbove",

            "buysSummedAboveBelowLong",
            "buysSummedAboveBelowMed",
            "buysSummedAboveBelowShort",

            "buysSummedInSpreadLong",
            "buysSummedInSpreadMed",
            "buysSummedInSpreadShort"

            //"buysRatio"
        };


        [Parameter]
        public string mode { get; set; }

        private int _seconds;

        public int seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                TDABook.seconds = seconds;
                /// Send to Server
                /// 
                Send("SetBookDataSeconds", _seconds.ToString());
                dictTopicCounts["BookPiesData"] = TDABook.seconds;

            }
        }


        public List<RatioFrame[]> allRatioFrames = new List<RatioFrame[]>();

        public List<RatioFrame[]> ratioFrames
        {
            get { return _ratioFrames; }
            set
            {
                _ratioFrames = value;
            }
        }
        private List<RatioFrame[]> _ratioFrames = new List<RatioFrame[]>();

        public Dictionary<string, BookDataItem[]> bookColData
        {
            get { return _bookColData; }
            set
            {
                _bookColData = value;
            }
        }
        Dictionary<string, BookDataItem[]> _bookColData;


        #region Hub Vars
        Dictionary<string, int> dictTopicCounts = new Dictionary<string, int>() { };


        static string clockFormat = "h:mm:ss dddd MMMM d, yyyy";

        string clock;
        string logTopics;

        System.Text.StringBuilder logTopicsb = new System.Text.StringBuilder();  /// Called from Javascript

        private HubConnection hubConnection;

        public bool IsConnected => hubConnection != null && hubConnection.State == HubConnectionState.Connected;
        #endregion

        #region Dialog Vars
        Timer timerBookColumnsCharts = new Timer(500);
        bool priceDialogIsOpen = false;
        int ratiosBack = TDABook.ratiosBack;
        int ratiosDepth = TDABook.ratiosDepth;
        DateTime? endTime = TDABook.endTime;
        DateTime? startTime = TDABook.startTime;
        bool? isCurrentEndTime = TDABook.isCurrentEndTime;
        bool isChartTimeFrameNew = true;
        bool? showRegressionCurves = true;
        #endregion

        #endregion

        #region Dialog methods
        void OpenPriceDialog()
        {
            priceDialogIsOpen = true;
        }

        void Dialog_OkClick()
        {
            TDABook.ratiosDepth = ratiosDepth;
            TDABook.ratiosBack = ratiosBack;
            TDABook.endTime = endTime;
            TDABook.startTime = startTime;
            TDABook.isCurrentEndTime = isCurrentEndTime;
            TDABook.showRegressionCurves = showRegressionCurves;
            priceDialogIsOpen = false;
        }
        void OnChange(object value, string name, string format)
        {
            switch (name)
            {
                case "Start Time":
                    startTime = value as DateTime?;
                    break;

                case "End Time":
                    endTime = value as DateTime?;
                    break;

                case "End Time Current":
                    isCurrentEndTime = value as bool?;
                    break;

                case "Regression Curves":
                    showRegressionCurves = value as bool?;
                    break;
            }
        }
        #endregion



        protected override async Task OnInitializedAsync()
        {
            foreach (var name in CONSTANTS.valuesName)
                dictTopicCounts.Add(name, 0);


            //bookColData = await bookColumnsService.getBookColumnsData(ChartConfigure.seconds);

            /// TODO: Get symbol and date derived
            /// 
            var todaysDate = Math.Floor(DateTime.Now.ToOADate());


            await HubConnection_Initialize();

            InitializeTimers();

            dictTopicCounts["BookPiesData"] = TDABook.seconds;


            //if (mode == "simulate")
            //{ }
            //else
            var frames =await bookColumnsService.getAllRatioFrames(symbol, todaysDate, jsruntime);
            allRatioFrames = frames.Where(frame => frame[0].markPrice > 0).ToList();


            //await jsruntime.InvokeAsync<string>("BlazorSetTitle", new object[] { "Hello Dali!" });
        }

        private void InitializeTimers()
        {
#if tracing
            JsConsole.JsConsole.Error(jsruntime, $"InitializeTimers");
#endif

            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender, e);
            timerBookColumnsCharts.Start();
        }

        /// <summary>
        /// This makes the UI responsive. Events picked up every half second
        /// </summary>
        private async Task TimerBookColumnsCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            //JsConsole.JsConsole.Error(jsruntime, $"TimerBookColumnsCharts_Elapsed");
            //Data.TDAStreamerData.hubStatus = $"./images/blue.png";
            //if (TDAChart.isActive == true)
            {
                //JsConsole.JsConsole.Time(jsruntime, "TimerBookColumnsCharts_Elapsed");

                //timerBookColumnsCharts.Stop();



                //var frames = TDABook.ratiosDepth;
                //ratioFrames = new List<RatioFrame>();
                //ratioFrames.AddRange(allRatioFrames.TakeLast(frames));

                //timerBookColumnsCharts.Start();


                //var ratioFrame = await bookColumnsService.getIncrementalRatioFrames(SurfaceChartConfigurator.longSeconds, TDABook.ratiosDepth, jsruntime);
                //var addFrame = ratioFrames.Count == 0 || (ratioFrames.Count > 0 && ratioFrames.Last().dateTime != ratioFrame.dateTime);
                //if (addFrame)
                //{
                //    ratioFrames.Add(ratioFrame);

                //    dictTopicCounts[dictTopicCounts.Keys.Last()] += 1;
                //    //ratioFrames = new List<RatioFrame>();
                //    //ratioFrames.AddRange(allRatioFrames);
                //}
                //timerBookColumnsCharts.Start();
                //JsConsole.JsConsole.TimeEnd(jsruntime, "TimerBookColumnsCharts_Elapsed");
                //         TDAChart.isActive = false;
            }
            await Task.Yield();
        }

        #region Hub Events
        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }

        public async System.Threading.Tasks.Task HubConnection_Initialize()
        {
#if tracing
            JsConsole.JsConsole.Warn(jsruntime, $"HubConnection_Initialize: ");
#endif
            /// Init the SignalR Hub
            /// 
            try
            {

                //                var hubUrl = "";
                //#if dev
                //                hubUrl ="http://localhost:55540/tdahub";
                //                hubConnection = new HubConnectionBuilder().WithUrl(, options =>
                //                {
                //                    //options.Transports = HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents;
                //                }).Build();
                //#else
                //                hubUrl = "http://tapestreamserver.com/tdahub";
                //                hubConnection = new HubConnectionBuilder().WithUrl("http://tapestreamserver.com/tdahub", options =>
                //                {
                //                    //options.Transports =  HttpTransportType.WebSockets;
                //                }).Build();
                //#endif
                //                hubConnection = new HubConnectionBuilder().WithUrl(hubUrl, options =>
                //                {
                //                    //options.Transports = HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents;
                //                }).Build();

                var serverUrl = Configuration["ServerUrl"];
                hubUrl = $"{serverUrl}tdahub";
                hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).Build();

#if tracing
                JsConsole.JsConsole.Warn(jsruntime, $"hubConnection: {hubConnection.State}");
#endif
                /// Set up Hub Subscriptions -- Don't really need any anymore
                /// Perhaps get messages of counts? or ,essage to refresh to avoid using a timer 
                hubConnection.On("getIncrementalRatioFrames", (Action<string, string>)((topic, message) =>
                {
                    Chart_ProcessHubData(topic, message);

                }));

                hubConnection.Reconnected += HubConnection_Reconnected;
                hubConnection.Closed += HubConnection_Closed;
                hubConnection.Reconnecting += HubConnection_Reconnecting;

                //HubConnection_ShowStatus();


                await HubConnection_Start();

            }
            catch (System.Exception ex)
            {

            }
        }

        private void Chart_ProcessHubData(string topic, string message)
        {
            var newRatioFrames = System.Text.Json.JsonSerializer.Deserialize<RatioFrame[]>(message);

            /// To fix drop outs
            if (newRatioFrames[0].markPrice == 0) return;
            //newRatioFrames[0].markPrice = allRatioFrames.Last()[0].markPrice;

            clock = newRatioFrames[0].dateTime.ToString(clockFormat);
            var alwaysUpdate = allRatioFrames.Count < 60;
            allRatioFrames.Add(newRatioFrames);

            //if (isChartTimeFrameNew == true || alwaysUpdate)
            //{
            /// Resetting ratioFrames updates the whole chart
            ratioFrames = allRatioFrames.TakeLast(TDABook.ratiosDepth).ToList();
            //StateHasChanged();
            isChartTimeFrameNew = false;

            //}
            //else
            //{
            //    /// Resetting ratioFrames to 1 item appends new data points to the chart
            //    ratioFrames = new List<RatioFrame>();
            //    ratioFrames.Add( newRatioFrame);
            //    StateHasChanged();
            //}

            /// 
            //countIn++;
            //if (countIn % 10 == 0)
            //{
            //    ratioFrames = new List<RatioFrame>();
            //    ratioFrames.AddRange(allRatioFrames);
            //}

            Receive(topic, message);
            StateHasChanged();
        }

        private async Task HubConnection_Start()
        {
            await hubConnection.StartAsync();

            var msg = $"HubConnection {hubUrl} Started";
            HubConnection_ShowStatus(msg);
        }

        private void HubConnection_ShowStatus(string msg)
        {
            /// Show Hub Status in lamp color
            var color = IsConnected ? "green" : "red";
            Data.TDAStreamerData.hubStatusMessage = msg;
            Data.TDAStreamerData.hubStatus = $"./images/{color}.gif";
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
#if tracing
            JsConsole.JsConsole.Warn(jsruntime, $"HubConnection_Reconnecting: {hubConnection.State}");
#endif      
            Data.TDAStreamerData.hubStatusMessage = "HubConnection Reconnecting";
            Data.TDAStreamerData.hubStatus = $"./images/yellow.gif";
            return Task.CompletedTask;
        }

        private async Task HubConnection_Closed(Exception arg)
        {
            var color = IsConnected ? "green" : "red";
            Data.TDAStreamerData.hubStatusMessage = "HubConnection Closed";
            Data.TDAStreamerData.hubStatus = $"./images/{color}.gif";
#if tracing
            JsConsole.JsConsole.Warn(jsruntime, $"HubConnection_Closed: {hubConnection.State}");
#endif       

            /// Restart
            ///
            while (hubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection_Start();
                Data.TDAStreamerData.hubStatusMessage = "HubConnection Restarted";
                Data.TDAStreamerData.hubStatus = Data.TDAStreamerData.hubStatus;
            }
        }

        private Task HubConnection_Reconnected(string arg)
        {

            var color = IsConnected ? "green" : "red";
            Data.TDAStreamerData.hubStatusMessage = "HubConnection Reconnected";
            Data.TDAStreamerData.hubStatus = $"./images/{color}.gif";
            return Task.CompletedTask;
#if tracing
            JsConsole.JsConsole.Warn(jsruntime, $"HubConnection_Reconnected: {hubConnection.State}");
#endif       
        }

        void Receive(string topic, string content)
        {
            //JsConsole.JsConsole.Warn(jsruntime, $"Receive: {topic}:{content}");
            //clock = System.DateTime.Now.ToString(clockFormat);

            // Show the topic text (last 1000 lines)
            //logTopicsb.Insert(0, "\n" + topic + ":" + content.Replace("\r", "").Replace("\n", ""));
            //logTopics = string.Join('\n', logTopicsb.ToString().Split('\n'));

            ////jsruntime.count("TimeAndSales");
            //Task.Yield();
            // Update topic's Stats count
            dictTopicCounts[topic] += 1;


            //StateHasChanged();
            //var svcJsonObject = JObject.Parse(content);
            //var svcName = svcJsonObject["service"].ToString();
            //var contents = svcJsonObject["content"];
            ////var timeStamp = Convert.ToInt64(svcJsonObject["timestamp"]);
            //GetServiceTime(svcJsonObject);

        }

        Task Send(string userInput, string messageInput) =>
            hubConnection.SendAsync("SendTopic", userInput, messageInput);

        #endregion
    }
}

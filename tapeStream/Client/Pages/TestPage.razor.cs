#define dev
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

namespace tapeStream.Client.Pages
{
    public partial class TestPage
    {
        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }

        LinesChart LinesChart1;
        LinesChart LinesChart2;
        LinesChart LinesChart3;
        LinesChart LinesChart4;
        LinesChart LinesChart5;
        LinesChart LinesChart6;

        public List<RatioFrame> allRatioFrames = new List<RatioFrame>();

        public List<RatioFrame> ratioFrames
        {
            get { return _ratioFrames; }
            set
            {
                _ratioFrames = value;
            }
        }
        private List<RatioFrame> _ratioFrames = new List<RatioFrame>();

        #region Hub Vars
        Dictionary<string, int> dictTopicCounts = new Dictionary<string, int>() { };


        static string clockFormat = "h:mm:ss dddd MMMM d, yyyy";

        string clock;
        string logTopics;

        System.Text.StringBuilder logTopicsb = new System.Text.StringBuilder();  /// Called from Javascript

        private HubConnection hubConnection;

        public bool IsConnected => hubConnection != null && hubConnection.State == HubConnectionState.Connected;
        #endregion

        Timer timerBookColumnsCharts = new Timer(500);

        protected override async Task OnInitializedAsync()
        {
            foreach (var name in CONSTANTS.valuesName)
                dictTopicCounts.Add(name, 0);

            await HubConnection_Initialize();

            InitializeTimers();

        }

        private void InitializeTimers()
        {
#if tracing
            JsConsole.JsConsole.Error(jsruntime, $"InitializeTimers");
#endif

            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender, e);
            timerBookColumnsCharts.Start();
        }

        private async Task TimerBookColumnsCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            //JsConsole.JsConsole.Error(jsruntime, $"TimerBookColumnsCharts_Elapsed");
            //Data.TDAStreamerData.hubStatus = $"./images/blue.png";
            //if (TDAChart.isActive == true)
            {
                //JsConsole.JsConsole.Time(jsruntime, "TimerBookColumnsCharts_Elapsed");
                
                //timerBookColumnsCharts.Stop();

                var frames = TDABook.ratiosDepth;
                ratioFrames = new List<RatioFrame>();
                ratioFrames.AddRange(allRatioFrames.TakeLast(frames));

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
        }

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
#if dev
                hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:55540/tdahub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                }).Build();
#else
                hubConnection = new HubConnectionBuilder().WithUrl("http://tapestreamserver.com/tdahub", options =>
                {
                    options.Transports =  HttpTransportType.WebSockets;
                }).Build();
#endif
                /// Set up Hub Subscriptions -- Don't really need any anymore
                /// Perhaps get messages of counts? or ,essage to refresh to avoid using a timer 
                hubConnection.On("getIncrementalRatioFrames", (Action<string, string>)((topic, message) =>
                {

                    var newRatioFrame = System.Text.Json.JsonSerializer.Deserialize<RatioFrame>(message);
                    allRatioFrames.Add(newRatioFrame);
                    clock = newRatioFrame.dateTime.ToString(clockFormat);
                    //countIn++;
                    //if (countIn % 10 == 0)
                    //{
                    //    ratioFrames = new List<RatioFrame>();
                    //    ratioFrames.AddRange(allRatioFrames);
                    //}

                    Receive(topic, message);
                    StateHasChanged();

                }));

                hubConnection.Reconnected += HubConnection_Reconnected;
                hubConnection.Closed += HubConnection_Closed;
                hubConnection.Reconnecting += HubConnection_Reconnecting;

                await HubConnection_Start();

            }
            catch (System.Exception ex)
            {

            }
        }

        private async Task HubConnection_Start()
        {
            await hubConnection.StartAsync();

            /// Show Hub Status in lamp color
            var color = IsConnected ? "green" : "red";
            Data.TDAStreamerData.hubStatusMessage = "HubConnection Started";
            Data.TDAStreamerData.hubStatus = $"./images/{color}.gif";
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
            Data.TDAStreamerData.hubStatusMessage = "HubConnection Reconnecting";
            Data.TDAStreamerData.hubStatus = $"./images/yellow.gif";
            return Task.CompletedTask;
        }

        private async Task HubConnection_Closed(Exception arg)
        {
            var color = IsConnected ? "green" : "red";
            Data.TDAStreamerData.hubStatusMessage = "HubConnection Closed";
            Data.TDAStreamerData.hubStatus = $"./images/{color}.gif";

            /// Restart
            await HubConnection_Start();
            Data.TDAStreamerData.hubStatusMessage = "HubConnection Restarted";
            Data.TDAStreamerData.hubStatus = Data.TDAStreamerData.hubStatus;
        }

        private Task HubConnection_Reconnected(string arg)
        {
            var color = IsConnected ? "green" : "red";
            Data.TDAStreamerData.hubStatusMessage = "HubConnection Reconnected";
            Data.TDAStreamerData.hubStatus = $"./images/{color}.gif";
            return Task.CompletedTask;
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


            StateHasChanged();
            //var svcJsonObject = JObject.Parse(content);
            //var svcName = svcJsonObject["service"].ToString();
            //var contents = svcJsonObject["content"];
            ////var timeStamp = Convert.ToInt64(svcJsonObject["timestamp"]);
            //GetServiceTime(svcJsonObject);

        }

    }
}

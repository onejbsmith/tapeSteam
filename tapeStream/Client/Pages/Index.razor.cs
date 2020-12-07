#undef tracing
#define UsingSignalHub
#define dev
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using tapeStream.Client.Components;
using tapeStream.Shared;
using tapeStream.Shared.Data;
using tapeStream.Shared.Services;

namespace tapeStream.Client.Pages
{
    public partial class Index
    {

        //[Inject] SignalRBase signalRBase { get; set; }
        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] ChartService chartService { get; set; }

        Timer timerBookColumnsCharts = new Timer(1000);

        public List<RatioFrame> allRatioFrames = new List<RatioFrame>();

        public List<RatioFrame> _ratioFrames = new List<RatioFrame>();

        public List<RatioFrame> ratioFrames
        {
            get { return _ratioFrames; }
            set
            {
                _ratioFrames = value;
            }
        }

        public Dictionary<string, BookDataItem[]> bookColData
        {
            get { return _bookColData; }
            set
            {
                _bookColData = value;
            }
        }
        Dictionary<string, BookDataItem[]> _bookColData;

#if UsingSignalHub
        Dictionary<string, int> dictTopicCounts = new Dictionary<string, int>();

        static string clockFormat = "h:mm:ss dddd MMMM d, yyyy";


        bool logHub;

        private string topicInput;
        private string messageInput;

        string clock;
        string logTopics;

        System.Text.StringBuilder logTopicsb = new System.Text.StringBuilder();  /// Called from Javascript

        private HubConnection hubConnection;
        private System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();


        /// <summary>
        /// Method to test if hub connection is alive
        /// </summary>
        /// 

        public bool IsConnected =>
            hubConnection != null
            && hubConnection.State == HubConnectionState.Connected;

#endif
        protected override async Task OnInitializedAsync()
        {
            JsConsole.JsConsole.Error(jsruntime, $"OnInitializedAsync: ");

            /// Init parameters so don't get "null" error
            await InitializeData();

#if UsingSignalHub

            foreach (var name in CONSTANTS.valuesName)
                dictTopicCounts.Add(name, 0);

            //await InitHub();
            await HubConnection_Initialize();

            //await signalRBase.Subscribe("TimeAndSales", incrementTimeAndSales);
            //await signalRBase.Unsubscribe("TimeAndSales", incrementTimeAndSales);

            InitializeTimers();


#else
#endif
        }

        private void incrementTimeSales()
        { }

#if UsingSignalHub
        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        static int  countIn;

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
                    countIn += 1;
                    if (countIn % 1 == 0)
                    {
                        ratioFrames = new List<RatioFrame>();
                        ratioFrames.AddRange(allRatioFrames);
                    }
                    Task.Yield();
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

            Task.Yield();

            StateHasChanged();
            //var svcJsonObject = JObject.Parse(content);
            //var svcName = svcJsonObject["service"].ToString();
            //var contents = svcJsonObject["content"];
            ////var timeStamp = Convert.ToInt64(svcJsonObject["timestamp"]);
            //GetServiceTime(svcJsonObject);

        }
#endif
        private void InitializeTimers()
        {
            JsConsole.JsConsole.Error(jsruntime, $"InitializeTimers");

            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender, e);
            timerBookColumnsCharts.Start();


        }
        private async Task InitializeData()
        {
            JsConsole.JsConsole.Error(jsruntime, $"InitializeData");

            bookColData = CONSTANTS.newBookColumnsData;

            //await AppendLineChartData();
        }


        private async Task TimerBookColumnsCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            JsConsole.JsConsole.Error(jsruntime, $"TimerBookColumnsCharts_Elapsed");
            //Data.TDAStreamerData.hubStatus = $"./images/blue.png";
            //if (TDAChart.isActive == true)
            {
                JsConsole.JsConsole.Time(jsruntime, "TimerBookColumnsCharts_Elapsed");
                timerBookColumnsCharts.Stop();
                await GetBookColumnsData(ChartConfigure.seconds);
                timerBookColumnsCharts.Start();
                JsConsole.JsConsole.TimeEnd(jsruntime, "TimerBookColumnsCharts_Elapsed");
                //         TDAChart.isActive = false;
            }
        }

        private async Task GetBookColumnsData(int seconds)
        {
            try
            {
                await Task.Yield();
                bookColData = await bookColumnsService.getBookColumnsData(seconds);
                await Task.Yield();

                //var frames = TDABook.ratiosDepth;
                //ratioFrames = new List<RatioFrame>();
                //ratioFrames.AddRange(allRatioFrames);


                //var ratioFrame = await bookColumnsService.getIncrementalRatioFrames(SurfaceChartConfigurator.longSeconds, TDABook.ratiosDepth, jsruntime);
                //allRatioFrames.Add(ratioFrame);
                //ratioFrames = new List<RatioFrame>();
                //ratioFrames.AddRange(allRatioFrames);

                //await Task.Yield();
                //await bookColumnsService.getLtRatios(SurfaceChartConfigurator.longSeconds, jsruntime);
                //allRatioFrames = await bookColumnsService.getRatioFrames(SurfaceChartConfigurator.longSeconds, TDABook.ratiosDepth, jsruntime);
                //ratioFrames = allRatioFrames;
                //await Task.Yield();

                await Task.Yield();
                TDAChart.svcDateTimeRaw = await chartService.GetSvcDate();
                await Task.Yield();

                await Task.Yield();
                TDAChart.svcDateTimeRaw = TDAChart.svcDateTimeRaw.Replace("\"", "");
                await Task.Yield();

                TDAChart.svcDateTime = Convert.ToDateTime(TDAChart.svcDateTimeRaw);
                TDAChart.LongDateString = TDAChart.svcDateTime.ToLongDateString() + " " + TDAChart.svcDateTime.ToLongTimeString();

                StateHasChanged();
            }
            catch (Exception ex)
            {

                //jsruntime.error(ex.ToString());
                //JsConsole.JsConsole.Confirm(jsruntime, ex.ToString());
            }
            await Task.CompletedTask;
        }


        private async Task xGetBookColumnsData(int seconds)
        {
            JsConsole.JsConsole.Error(jsruntime, $"GetBookColumnsData");

#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, seconds, $"0. Index GetBookColumnsData seconds");
#endif

            JsConsole.JsConsole.Time(jsruntime, "GetBookColumnsData");

            ///JsConsole.JsConsole.Time(jsruntime, "timerBookColumnsCharts.Stop()");
           // timerBookColumnsCharts.Stop();
            ///JsConsole.JsConsole.TimeEnd(jsruntime, "timerBookColumnsCharts.Stop()");

            JsConsole.JsConsole.GroupCollapsed(jsruntime, "GetBookColumnsData");

            try
            {
                await Task.Yield();
                ///JsConsole.JsConsole.Time(jsruntime, "getBookColumnsData");
                bookColData = await bookColumnsService.getBookColumnsData(seconds);
                await Task.Yield();
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "getBookColumnsData");
                ///JsConsole.JsConsole.Time(jsruntime, "getBookColumnsData");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "getBookColumnsData");

                ///JsConsole.JsConsole.Time(jsruntime, $"getAverages {SurfaceChartConfigurator.longSeconds}");
                //var avgSizes = await bookColumnsService.getAverages(SurfaceChartConfigurator.longSeconds, jsruntime);
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getAverages {SurfaceChartConfigurator.longSeconds}");
                ///JsConsole.JsConsole.Time(jsruntime, $"getAverages {SurfaceChartConfigurator.longSeconds}");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getAverages {SurfaceChartConfigurator.longSeconds}");

                ///JsConsole.JsConsole.Time(jsruntime, $"getAverages {SurfaceChartConfigurator.shortSeconds}");
                //var avgStSizes = await bookColumnsService.getAverages(SurfaceChartConfigurator.shortSeconds, jsruntime);
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getAverages {SurfaceChartConfigurator.shortSeconds}");
                ///JsConsole.JsConsole.Time(jsruntime, $"getAverages {SurfaceChartConfigurator.shortSeconds}");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getAverages {SurfaceChartConfigurator.shortSeconds}");

                ///JsConsole.JsConsole.Time(jsruntime, $"getAverages {0}");
                //var avgLtSizes = await bookColumnsService.getAverages(0, jsruntime);
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getAverages {0}");

                /// This will update http://tapestreamserver.com/files/ratioFrames{seconds}.txt
                ///JsConsole.JsConsole.Time(jsruntime, $"getRatioFrames {SurfaceChartConfigurator.longSeconds}");
                await Task.Yield();
                //var ratioFrames = await bookColumnsService.getRatioFrames(SurfaceChartConfigurator.longSeconds, TDABook.ratiosDepth, jsruntime);
                await Task.Yield();
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getRatioFrames {SurfaceChartConfigurator.longSeconds}");
                ///JsConsole.JsConsole.Time(jsruntime, $"getRatioFrames {SurfaceChartConfigurator.longSeconds}");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getRatioFrames {SurfaceChartConfigurator.longSeconds}");


                var ratioFrame = await bookColumnsService.getIncrementalRatioFrames(SurfaceChartConfigurator.longSeconds, TDABook.ratiosDepth, jsruntime);
                _ratioFrames.Add(ratioFrame);
                ratioFrames = _ratioFrames;

                StateHasChanged();

                ///JsConsole.JsConsole.Time(jsruntime, $"getRatios {SurfaceChartConfigurator.longSeconds}");
                //await Task.Yield();
                //var avgRatios = await bookColumnsService.getRatios(SurfaceChartConfigurator.longSeconds, jsruntime);
                //await Task.Yield();
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getRatios {SurfaceChartConfigurator.longSeconds}");
                ///JsConsole.JsConsole.Time(jsruntime, $"getRatios {SurfaceChartConfigurator.longSeconds}");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getRatios {SurfaceChartConfigurator.longSeconds}");
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, avgRatios, "1. Index avgRatios");
#endif
                ///JsConsole.JsConsole.Time(jsruntime, $"getRatios {SurfaceChartConfigurator.shortSeconds}");
                //await Task.Yield();
                //var avgStRatios = await bookColumnsService.getRatios(SurfaceChartConfigurator.shortSeconds, jsruntime);
                //await Task.Yield();
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getRatios {SurfaceChartConfigurator.shortSeconds}");
                ///JsConsole.JsConsole.Time(jsruntime, $"getRatios {SurfaceChartConfigurator.shortSeconds}");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getRatios {SurfaceChartConfigurator.shortSeconds}");
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, avgStRatios, "2. Index avgStRatios");
#endif
                ///JsConsole.JsConsole.Time(jsruntime, $"getRatios {0}");
                //await Task.Yield();
                //var avgLtRatios = await bookColumnsService.getRatios(0, jsruntime);
                //await Task.Yield();
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getRatios {0}");
                ///JsConsole.JsConsole.Time(jsruntime, $"getRatios {0}");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, $"getRatios {0}");
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, avgLtRatios, "3. Index avgLtRatios");
#endif
                ///JsConsole.JsConsole.Time(jsruntime, "getBollingerBands");
                //TDAChart.bollingerBands = await chartService.getBollingerBands();
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "getBollingerBands");
                ///JsConsole.JsConsole.Time(jsruntime, "getBollingerBands");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "getBollingerBands");
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, TDAChart.bollingerBands, "3a. Index TDAChart.bollingerBands");
#endif
                ///JsConsole.JsConsole.Time(jsruntime, "GetTDAChartLastCandle 0");
                //await Task.Yield();
                //TDAChart.lastCandle = await chartService.GetTDAChartLastCandle(0);
                //await Task.Yield();
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "GetTDAChartLastCandle 0");
                ///JsConsole.JsConsole.Time(jsruntime, "GetTDAChartLastCandle 0");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "GetTDAChartLastCandle 0");
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, TDAChart.lastCandle, "3b. Index TDAChart.bollingerBands");
#endif
                ///JsConsole.JsConsole.Time(jsruntime, "GetSvcDate");
                await Task.Yield();
                TDAChart.svcDateTimeRaw = await chartService.GetSvcDate();
                await Task.Yield();
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "GetSvcDate");
                ///JsConsole.JsConsole.Time(jsruntime, "GetSvcDate");
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "GetSvcDate");
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, TDAChart.svcDateTimeRaw, "3c. Index TDAChart.svcDateTimeRaw");
#endif
                ///JsConsole.JsConsole.Time(jsruntime, "Calcs");
                await Task.Yield();
                TDAChart.svcDateTimeRaw = TDAChart.svcDateTimeRaw.Replace("\"", "");
                await Task.Yield();

                TDAChart.svcDateTime = Convert.ToDateTime(TDAChart.svcDateTimeRaw);
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, TDAChart.svcDateTime, "3e. Index TDAChart.svcDateTime");
#endif
                TDAChart.LongDateString = TDAChart.svcDateTime.ToLongDateString() + " " + TDAChart.svcDateTime.ToLongTimeString();
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, TDAChart.LongDateString, "3f. Index TDAChart.LongDateString");
#endif
#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.lstSvcTimes.Count, $"3g. Index  lstSvcTimes.Count");
#endif
                //foreach (var name in avgSizes.averageSize.Keys)
                //    if (avgSizes.averageSize[name] > 0)
                //        TDAChart.avgSizes.averageSize[name] = avgSizes.averageSize[name];

                //foreach (var name in avgStSizes.averageSize.Keys)
                //    if (avgStSizes.averageSize[name] > 0)
                //        TDAChart.avgStSizes.averageSize[name] = avgStSizes.averageSize[name];

                //TDAChart.avgSizes = avgSizes;
                //TDAChart.avgStSizes = avgStSizes;
                //TDAChart.avgLtSizes = avgLtSizes;

                //TDAChart.avgRatios = avgRatios;
                //TDAChart.avgStRatios = avgStRatios;
                //TDAChart.avgLtRatios = avgLtRatios;

#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, avgRatios.averageSize.ContainsKey("buys"), "4a. Index avgRatios.averageSize.ContainsKey('buys')");
#endif
                //var avgBuys = 0d;

                //if (avgRatios.averageSize != null)
                //    if (avgRatios.averageSize.ContainsKey("buys"))
                //    {
                //        avgBuys = avgRatios.averageSize["buys"];
#if tracing
                JsConsole.JsConsole.GroupTable(jsruntime, avgBuys, $"4b. Index avgBuys");
#endif

                //TDAChart.avgBuysRatio = avgBuys;
#if tracing
                JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.avgBuysRatio, $"4c. Index TDAChart.avgBuysRatio");
#endif

                //TDAChart.lstBuysRatios.Add((float)avgBuys);
#if tracing
                JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.lstBuysRatios, $"5. Index lstBuysRatios");
#endif
                //    }

                //var avgSells = 0d;
                //if (avgRatios.averageSize != null)
                //    if (avgRatios.averageSize.ContainsKey("sells"))
                //    {
                //        avgSells = avgRatios.averageSize["sells"];
                //        TDAChart.avgSellsRatio = avgSells;
                //        //TDAChart.lstSellsRatios.Add((float)avgSells);
#if tracing
                JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.lstSellsRatios, $"6. Index lstSellsRatios");
#endif
                //    }

                //if (avgLtRatios.averageSize != null)
                //    if (avgLtRatios.averageSize.ContainsKey("buys"))
                //    {
                //        TDAChart.avgLtBuysRatio = avgLtRatios.averageSize["buys"];
                //    }

                //if (avgLtRatios.averageSize != null)
                //    if (avgLtRatios.averageSize.ContainsKey("sells"))
                //    {
                //        TDAChart.avgLtSellsRatio = avgLtRatios.averageSize["sells"];
                //    }
#if tracing
            jsruntime.GroupTable(TDAChart.lastCandle, "6. Index lastCandle");
#endif

                //TDAChart.lstMarkPrices.Add((float)mark);
#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.lstMarkPrices, $"9. Index lstMarkPrices");
#endif
                //TDAChart.countBuysRatioUp += avgBuys > avgSells ? 1 : 0;
                //TDAChart.countSellsRatioUp += avgSells > avgBuys ? 1 : 0;
                ///JsConsole.JsConsole.TimeEnd(jsruntime, "Calcs");

                JsConsole.JsConsole.GroupEnd(jsruntime);
                //TDAChart.lastCandles = await chartService.getLastCandles(2);
                //await Task.Delay(100);

                //await jsruntime.InvokeVoidAsync("Dump", Dumps(), "TDAChart.lastCandle");
                //await jsruntime.InvokeAsync<string>("Confirm", "Task GetBookColumnsData");



                //jsruntime.Confirm("Task GetBookColumnsData");

                //Debug.WriteLine("2. BookColumnsCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
                StateHasChanged();
            }
            catch (Exception ex)
            {

                //jsruntime.error(ex.ToString());
                //JsConsole.JsConsole.Confirm(jsruntime, ex.ToString());
            }


            ///JsConsole.JsConsole.Time(jsruntime, "timerBookColumnsCharts.Start()");
            //timerBookColumnsCharts.Start();
            ///JsConsole.JsConsole.TimeEnd(jsruntime, "timerBookColumnsCharts.Start()");

            JsConsole.JsConsole.TimeEnd(jsruntime, "GetBookColumnsData");
            await Task.CompletedTask;
        }
    }
}
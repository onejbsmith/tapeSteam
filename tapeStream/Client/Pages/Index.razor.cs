#undef tracing
#define UsingSignalHub
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
    public partial class Index
    {


        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] ChartService chartService { get; set; }

        Timer timerBookColumnsCharts = new Timer(500);

        private List<RatioFrame> _ratioFrames;


        public Dictionary<string, BookDataItem[]> bookColData
        {
            get { return _bookColData; }
            set
            {
                _bookColData = value;
            }
        }
        Dictionary<string, BookDataItem[]> _bookColData;

        Dictionary<string, int> dictTopicCounts = new Dictionary<string, int>();

        static string clockFormat = "h:mm:ss MMM d, yyyy";


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


        protected override async Task OnInitializedAsync()
        {

            /// Init parameters so don't get "null" error
            await InitializeData();

#if UsingSignalHub

            foreach (var name in CONSTANTS.valuesName)
                dictTopicCounts.Add(name, 0);

            await InitHub();
#endif


            InitializeTimers();

        }
        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        public async System.Threading.Tasks.Task InitHub()
        {
            /// Init the SignalR Hub
            /// 
            try
            {

#if dev
                hubConnection = new HubConnectionBuilder()
             .WithUrl("http://localhost:55540/tdahub")
             .Build();
#else
                hubConnection = new HubConnectionBuilder()
             .WithUrl("http://tapestreamserver.com/tdahub")
             .Build();
#endif
                /// Set up Hub Subscriptions -- Don't really need any anymore
                /// Perhaps get messages of counts? or ,essage to refresh to avoid using a timer 
                await SetupHub();

                await hubConnection.StartAsync();

                /// Show Hub Status in lamp color
                var color = IsConnected ? "green" : "red";
                Data.TDAStreamerData.hubStatus = $"./images/{color}.gif";

            }
            catch (System.Exception ex)
            {
                JsConsole.JsConsole.Confirm(jsruntime, ex.ToString());
            }
        }


        private async Task SetupHub()
        {
            hubConnection.On("TimeAndSales", (System.Action<string, string>)(async (topic, message) =>
            {

                Receive(topic, message);
                var dateTime = DateTime.FromOADate(Convert.ToDouble(message));
                /// Fire event here
                TDAChart.isActive = true;
                Data.TDAStreamerData.hubStatus = $"./images/green.gif";
                JsConsole.JsConsole.Log(jsruntime, dateTime);

                //await TimerBookColumnsCharts_Elapsed(null, null);

            }));
        }

        void Receive(string topic, string content)
        {
            clock = System.DateTime.Now.ToString(clockFormat);
            // Show the topic text (last 1000 lines)
            //logTopicsb.Insert(0, "\n" + topic + ":" + content.Replace("\r", "").Replace("\n", ""));
            //logTopics = string.Join('\n', logTopicsb.ToString().Split('\n'));

            // Update topic's Stats count
            //dictTopicCounts[topic] += 1;

            //var svcJsonObject = JObject.Parse(content);
            //var svcName = svcJsonObject["service"].ToString();
            //var contents = svcJsonObject["content"];
            ////var timeStamp = Convert.ToInt64(svcJsonObject["timestamp"]);
            //GetServiceTime(svcJsonObject);

            //StateHasChanged();
        }

        private void InitializeTimers()
        {

            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender, e);
            timerBookColumnsCharts.Start();


        }
        private async Task InitializeData()
        {

            bookColData = CONSTANTS.newBookColumnsData;

            //await AppendLineChartData();
        }


        private async Task TimerBookColumnsCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            Data.TDAStreamerData.hubStatus = $"./images/blue.png";
            if (TDAChart.isActive == true)
            {
                await GetBookColumnsData(ChartConfigure.seconds);
                TDAChart.isActive = false;
            }
        }

        private async Task GetBookColumnsData(int seconds)
        {

#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, seconds, $"0. Index GetBookColumnsData seconds");
#endif


            timerBookColumnsCharts.Stop();


            try
            {
                await Task.Yield();
                bookColData = await bookColumnsService.getBookColumnsData(seconds);

                var avgSizes = await bookColumnsService.getAverages(SurfaceChartConfigurator.longSeconds, jsruntime);
                var avgStSizes = await bookColumnsService.getAverages(SurfaceChartConfigurator.shortSeconds, jsruntime);
                var avgLtSizes = await bookColumnsService.getAverages(0, jsruntime);

                /// This will update http://tapestreamserver.com/files/ratioFrames{seconds}.txt
                var ratioFrames = await bookColumnsService.getRatioFrames(SurfaceChartConfigurator.longSeconds, TDABook.ratiosDepth, jsruntime);

                _ratioFrames = ratioFrames;
                var avgRatios = await bookColumnsService.getRatios(SurfaceChartConfigurator.longSeconds, jsruntime);
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, avgRatios, "1. Index avgRatios");
#endif
                var avgStRatios = await bookColumnsService.getRatios(SurfaceChartConfigurator.shortSeconds, jsruntime);
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, avgStRatios, "2. Index avgStRatios");
#endif
                var avgLtRatios = await bookColumnsService.getRatios(0, jsruntime);
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, avgLtRatios, "3. Index avgLtRatios");
#endif
                TDAChart.bollingerBands = await chartService.getBollingerBands();
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, TDAChart.bollingerBands, "3a. Index TDAChart.bollingerBands");
#endif
                TDAChart.lastCandle = await chartService.GetTDAChartLastCandle(0);
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, TDAChart.lastCandle, "3b. Index TDAChart.bollingerBands");
#endif
                TDAChart.svcDateTimeRaw = await chartService.GetSvcDate();
                TDAChart.svcDateTimeRaw = TDAChart.svcDateTimeRaw.Replace("\"", "");
#if tracing
            await JsConsole.JsConsole.GroupTableAsync(jsruntime, TDAChart.svcDateTimeRaw, "3c. Index TDAChart.svcDateTimeRaw");
#endif

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

                TDAChart.avgSizes = avgSizes;
                TDAChart.avgStSizes = avgStSizes;
                TDAChart.avgLtSizes = avgLtSizes;

                TDAChart.avgRatios = avgRatios;
                TDAChart.avgStRatios = avgStRatios;
                TDAChart.avgLtRatios = avgLtRatios;

#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, avgRatios.averageSize.ContainsKey("buys"), "4a. Index avgRatios.averageSize.ContainsKey('buys')");
#endif
                var avgBuys = 0d;

                if (avgRatios.averageSize != null)
                    if (avgRatios.averageSize.ContainsKey("buys"))
                    {
                        avgBuys = avgRatios.averageSize["buys"];
#if tracing
                JsConsole.JsConsole.GroupTable(jsruntime, avgBuys, $"4b. Index avgBuys");
#endif

                        TDAChart.avgBuysRatio = avgBuys;
#if tracing
                JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.avgBuysRatio, $"4c. Index TDAChart.avgBuysRatio");
#endif

                        //TDAChart.lstBuysRatios.Add((float)avgBuys);
#if tracing
                JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.lstBuysRatios, $"5. Index lstBuysRatios");
#endif
                    }

                var avgSells = 0d;
                if (avgRatios.averageSize != null)
                    if (avgRatios.averageSize.ContainsKey("sells"))
                    {
                        avgSells = avgRatios.averageSize["sells"];
                        TDAChart.avgSellsRatio = avgSells;
                        //TDAChart.lstSellsRatios.Add((float)avgSells);
#if tracing
                JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.lstSellsRatios, $"6. Index lstSellsRatios");
#endif
                    }

                if (avgLtRatios.averageSize != null)
                    if (avgLtRatios.averageSize.ContainsKey("buys"))
                    {
                        TDAChart.avgLtBuysRatio = avgLtRatios.averageSize["buys"];
                    }

                if (avgLtRatios.averageSize != null)
                    if (avgLtRatios.averageSize.ContainsKey("sells"))
                    {
                        TDAChart.avgLtSellsRatio = avgLtRatios.averageSize["sells"];
                    }
#if tracing
            jsruntime.GroupTable(TDAChart.lastCandle, "6. Index lastCandle");
#endif

                //TDAChart.lstMarkPrices.Add((float)mark);
#if tracing
            JsConsole.JsConsole.GroupTable(jsruntime, TDAChart.lstMarkPrices, $"9. Index lstMarkPrices");
#endif
                TDAChart.countBuysRatioUp += avgBuys > avgSells ? 1 : 0;
                TDAChart.countSellsRatioUp += avgSells > avgBuys ? 1 : 0;

                //TDAChart.lastCandles = await chartService.getLastCandles(2);
                await Task.Delay(100);

                //await jsruntime.InvokeVoidAsync("Dump", Dumps(), "TDAChart.lastCandle");
                //await jsruntime.InvokeAsync<string>("Confirm", "Task GetBookColumnsData");



                //jsruntime.Confirm("Task GetBookColumnsData");

                //Debug.WriteLine("2. BookColumnsCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
                StateHasChanged();
            }
            catch (Exception ex)
            {

                JsConsole.JsConsole.Confirm(jsruntime, ex.ToString());
            }

            timerBookColumnsCharts.Start();

            await Task.CompletedTask;
        }
    }
}
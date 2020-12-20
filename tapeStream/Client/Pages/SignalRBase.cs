#define UsingSignalHub
#undef dev
using System;
using System.Text;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace tapeStream.Client.Pages
{
    //[Route("/chartest")]
    public class SignalRBase
    {

        static string clockFormat = "h:mm:ss MMM d, yyyy";


        bool logHub;

        private string topicInput;
        private string messageInput;

        string clock;
        string logTopics;

        System.Text.StringBuilder logTopicsb = new System.Text.StringBuilder();  /// Called from Javascript
        System.Collections.Generic.Dictionary<string, int> dictTopicCounts = new System.Collections.Generic.Dictionary<string, int>();
        private HubConnection hubConnection;
        private System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();


        /// <summary>
        /// Method to test if hub connection is alive
        /// </summary>
        /// 

        public bool IsConnected =>
            hubConnection != null
            && hubConnection.State == HubConnectionState.Connected;


#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private Microsoft.JSInterop.IJSRuntime jsruntime { get; set; }

        /// <summary>
        /// Method that will shut down hub if this page is closed
        /// </summary>
        /// 
        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
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
            Debug.WriteLine("1. PrintsPieCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
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
            Debug.WriteLine("3. PrintsLineCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
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
            Debug.WriteLine("4. BookPieCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
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
        public async System.Threading.Tasks.Task Subscribe(string Topic, string idCreator, Func<Task> callback )
        {
            // <summary>
            /// Send message to Hub to start a Topic 
            /// Set up the receiver for the Topic to execute the callback
            /// </summary>
            /// <param name="user"></param>
            /// <param name="message"></param>
        }
        public async System.Threading.Tasks.Task Unsubscribe(string Topic, Func<Task> callback )
        {
            // <summary>
            /// Process message received from Hub
            /// Send message to Hub to stop a Topic
            /// </summary>
            /// <param name="user"></param>
            /// <param name="message"></param>
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


        /// <summary>
        /// Runs once, when this page is first loaded
        /// </summary>
        /// <returns></returns>
        /// 



        void Receive(string topic, string content)
        {
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

            clock = System.DateTime.Now.ToString(clockFormat);

            //StateHasChanged();
        }
        /// <summary>
        /// Method for this client to Send a message to the hub
        /// </summary>
        /// <returns></returns>
        /// 
        Task Send() =>
            hubConnection.SendAsync("SendMessage", topicInput, messageInput);


        private async Task SetupHub()
        {
            hubConnection.On("TimeAndSales", (System.Action<string, string>)(async (topic, message) =>
            {

                Receive(topic, message);

                /// The message is the TDAStreamerData.timeAndSales.time.ToString()
                /// 
                JsConsole.JsConsole.Log(jsruntime, message);



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
                //StateHasChanged();

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
    }
}
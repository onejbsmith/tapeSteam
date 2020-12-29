#define UsingSignalHub
#undef dev
using JSconsoleExtensionsLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using tapeStream.Server.Data;
using tapeStream.Server.Data.classes;
using tapeStream.Shared;
using tapeStream.Shared.Data;
using FilesManager = tapeStream.Server.Data.FilesManager;
using TDAApiService = tapeStream.Server.Data.TDAApiService;
using TDAConstants = tapeStream.Server.Data.TDAConstants;



namespace tapeStream.Server.Components
{
    public partial class TDAStreamer
    {
        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject]
        IJSRuntime console { get; set; }

        [Inject]
        BrowserService browserService { get; set; }
        [Inject]
        BlazorTimer Timer { get; set; }

        [Inject]
        IJSRuntime TDAStreamerJs { get; set; }


        #region Variables
        [Parameter]

        public string symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;

            }
        }
        private string _symbol = "QQQ";


        [Parameter]
        public bool simulate { get; set; }

        [Parameter]
        public SimulatorSettings simulatorSettings { get; set; }


        //public ObservableCollection<SparkData> DataSource { get; set; }
        DateTime svcDateTime = DateTime.Now;
        private string admCode = "-1";
        private string admStatus = "Not started";
        static string clockFormat = "h:mm:ss MMM d, yyyy";
        static string timerFormat = "h'h 'm'm 's's'";
        static string dateFormat = "h:mm:ss tt dddd MMM d, yyyy";
        string clock = DateTime.Now.ToString(clockFormat);
        string serviceRequestText = "";
        string serviceSelection = "Service Request";
        string logText = "";
        string logTopics = "";

        static string status = "None";
        string statusClass = "primary";

        StringBuilder logTextsb = new StringBuilder();  /// Called from Javascript
        StringBuilder logTopicsb = new StringBuilder();  /// Called from Javascript
        //string feedFile, chartFile, quoteFile = "";

        double strike = 0;

        IEnumerable<int> values = new int[] { 1, 2, 3 };
        readonly string[] valuesName = CONSTANTS.valuesName;
        int[] valuesCounts = new int[] { 999, 0, 0, 0, 0, 0, 0, 0, 0 };

        Dictionary<string, int> dictTopicCounts = new Dictionary<string, int>();

        DateTime optionExpDate = DateTime.Now.AddDays(1);

        private HubConnection hubConnection;

        private string dateTimeNow = DateTime.Now.ToString(dateFormat);

        bool? logStreamer = false;
        bool logHub;

        string sinceLastData = "0";
        string elapsed = "0";

        DateTime startedTime = DateTime.Now;
        Timer timer = new Timer(1000);

        public int Height { get; set; }
        public int Width { get; set; }

        double value = 0;

        #endregion


        public async Task ConsoleLog(string log)
        {
            await TDAStreamerJs.InvokeAsync<string>("console.log", log);
        }


        #region Simulator Button Event Handlers

        void Click(MouseEventArgs args, string buttonName)
        {
            switch (buttonName)
            {
                case "Start":
                    Simulator_Start(symbol);
                    break;
                case "Pause":
                    Simulator_Pause();
                    break;
                case "Resume":
                    Simulator_Resume();
                    break;
                case "Stop":
                    Simulator_Stop();
                    break;
            }
            StateHasChanged();
        }
        private async Task Simulator_Start(string symbol)
        {
            simulatorStarted = true;
            StateHasChanged();

            ////TEMP
            //FilesManager.MoveQFilesToDatedFolders();
            //return;

            /// Get all the feed file names for the TDAStreamerData.simulatorSettings

            TDAStreamerData.runDate = TDAStreamerData.simulatorSettings.runDate;

            TDAStreamerData.simulatorSettings.currentSimulatedTime = TDAStreamerData.simulatorSettings.runDateDate.Add(TDAStreamerData.simulatorSettings.startTime.TimeOfDay);
            if (TDAStreamerData.simulatorSettings.isSimulated != null && TDAStreamerData.simulatorSettings.isSimulated == false)
            /// means the simulator was stopped so need to reset counts
            {
                DictTopicCounts_Initialize();
            }
            TDAStreamerData.simulatorSettings.isSimulated = true;

            Dictionary<DateTime, string> feedFilesList = FilesManager.GetFeedFileNames(symbol, TDAStreamerData.simulatorSettings);

            /// Process each file
            foreach (var feedFile in feedFilesList)
            {
                /// Get the next feed file and make it a json Array 
                //await Task.Yield();

                var fileJson = FilesManager.GetFeedFile(feedFile.Value);
                var feedJson = $"{{ \"data\":[{fileJson}] }}";

                //await Task.Yield();
                /// Process file
                await TDAStreamerOnMessage(feedJson);

                //await Task.Yield();
                /// Wait for next file
                int delayMilliSecs = TDAStreamerData.simulatorSettings.clockDelay;
                await Task.Delay(delayMilliSecs);
                //await Task.Yield();

                /// Pause the simulator
                while (simulatorPaused)
                {
                    await Task.Yield();
                    await Task.Delay(1000);
                }
                /// Stop the simulator
                if (simulatorStarted == false)
                    break;
            }
        }

        private void Simulator_Stop()
        {
            simulatorStarted = false;
            TDAStreamerData.simulatorSettings.isSimulated = false;

        }

        private void Simulator_Resume()
        {
            simulatorPaused = false;
        }

        private void Simulator_Pause()
        {
            simulatorPaused = true;
        }

        #endregion

        #region Page Event Handlers   
        protected override async Task OnInitializedAsync()
        {
            console.warn("OnInitializedAsync");
            JSconsoleExtensions.enabled = true;
            DictTopicCounts_Initialize();

            JSconsoleExtensions.enabled = false;

            /// Connect to the web socket, passing it a ref to this page, so it can call methods from javascript
            var dotNetReference = DotNetObjectReference.Create(this);
            var dud = await TDAStreamerJs.InvokeAsync<string>("Initialize", dotNetReference);
            var dud2 = await TDAStreamerJs.InvokeAsync<string>("Connect");
            //feedFile = FilesManager.GetFileNameForToday("FEED");
            //quoteFile = FilesManager.GetFileNameForToday(@$"{symbol} QUOTES");
            //chartFile = FilesManager.GetFileNameForToday(@$"{symbol} CANDLES");
            await TDAApiService.GetAuthenticationAsync();


            await Task.CompletedTask;
            /// Get the current QQQ price so can determine options to track


            List<DayOfWeek> optionDaysOfWeek = new List<DayOfWeek>(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday });
            while (!optionDaysOfWeek.Contains(optionExpDate.DayOfWeek))
            {
                optionExpDate = optionExpDate.AddDays(1);
            }

            TDAStreamerData.quoteSymbol = symbol;
            TDAStreamerData.expiryDate = optionExpDate;

            timer.Elapsed += async (sender, e) => await Timer_ElapsedAsync();
            timer.Start();

#if UsingSignalHub
            await Init();
#endif

            /// This is fired by Decode_TimeSales
            TDAStreamerData.OnTimeSalesStatusChanged += sendTimeSalesData;
            TDAStreamerData.OnBookStatusChanged += TDAStreamerData_OnBookStatusChanged;

            TDAStreamerData.jSRuntime = TDAStreamerJs;


            string svcDate = DateTime.Now.ToString("MMMM dd, yyyy");

            TDAStreamerData.runDate = svcDate;

            TDAStreamerData.simulatorSettings = new SimulatorSettings(); ;

            TDAStreamerJs.warn($"tapeStream.Server.Components TDAStreamer OnInitializedAsync {TDAStreamerData.runDate}");

            SetBookDataSeconds(TDABook.seconds.ToString());

            await Task.CompletedTask;
        }

        private void DictTopicCounts_Initialize()
        {
            dictTopicCounts = new Dictionary<string, int>();
            foreach (var x in CONSTANTS.valuesName)
                dictTopicCounts.Add(x, 0);
        }

        async Task GetDimensions()
        {
            var dimension = await browserService.GetDimensions();
            Height = dimension.Height;
            Width = dimension.Width;
        }


        private async Task StateChangedAsync()
        {
            await InvokeAsync(() => StateHasChanged());
        }


        protected void serviceRequestChanged(RadzenSplitButtonItem item)
        {
            if (item == null) return;

            serviceSelection = item.Value;
            TDAStreamerData.chain = "102120C337,102120P337";
            serviceRequestText = TDAStreamerData.getServiceRequestOld(serviceSelection, symbol);
            //LogText(serviceRequestText);
        }

        void Change(IEnumerable<int> value, string name)
        {
            //var str = string.Join(", ", value);
            //events.Add(DateTime.Now, $"{name} value changed to {str}");
        }

        #endregion

        #region Button Event Handlers
        protected async Task Login()
        {
            string request = TDAStreamerData.getLoginRequest();

            var dud = await TDAStreamerJs.InvokeAsync<string>("tdaSendRequest", request);

            StateHasChanged();
        }

        protected async Task Sends()
        {
            //var servicesSelected = string.Join(',', values.Select(i => valuesName[i]));
            serviceRequestText = await TDAStreamerData.getServiceRequest(values, symbol);
            await TDAStreamerJs.InvokeAsync<string>("tdaSendRequest", serviceRequestText);
        }

        protected void Ping()
        {
            StateHasChanged();
        }

        protected async Task Logout()
        {
            string request = TDAStreamerData.getLoginRequest(isLogout: true);

            var dud = await TDAStreamerJs.InvokeAsync<string>("tdaSendRequest", request);

            StateHasChanged();
        }

        protected async Task Requested()
        {
            await Logout();
            await Login();
            await Login();
            await Sends();
        }
        void logStreamerChange(bool? arg, string comment)
        {
            if (arg == false)
            {
                logText = "";
                StateHasChanged();
            }
        }
        void logHubChange(bool? arg, string comment)
        {
            if (arg == false)
            {
                logTopics = "";
                StateHasChanged();
            }
        }
        #endregion

        static StringBuilder sb = new StringBuilder();

        #region External Event Handlers
        private void TDAStreamerData_OnBookStatusChanged()
        {
            /// Get Book Columns Data
            /// 
            try
            {
                console.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
                //var bookColsData = TDAPrintsManager.getBookColumnsData();
                //Send("BookColsData", JsonSerializer.Serialize<Dictionary<string, BookDataItem[]>>(bookColsData));
                dictTopicCounts["BookColsData"] += 1;
                StateHasChanged();
            }
            catch { }



            ///// Get Book Pies Data
            ///// 
            //try
            //{
            //    var dictBookPiesData = new Dictionary<int, BookDataItem[]>();
            //    foreach (var secs in CONSTANTS.printSeconds)
            //    {
            //        var bookPiesData = TDAPrintsManager.getBookPieData(secs);
            //        dictBookPiesData.Add(secs, bookPiesData);

            //    }
            //    Send("BookPiesData", JsonSerializer.Serialize<Dictionary<int, BookDataItem[]>>(dictBookPiesData));

            //}
            //catch
            //{

            //}
            ///// Get Book Summary Pie Data
            ///// 
            //try
            //{
            //    var bookData = TDAPrintsManager.getBookCompositePieData();
            //    Send("BookBigPieData", JsonSerializer.Serialize<BookDataItem[]>(bookData));

            //}
            //catch
            //{

            //}
        }

        static DateTime lastSvcTime;
        static int i = 0;
        static DateTime prevRatioFrameDateTime = DateTime.Now;
        private void sendTimeSalesData()
        {
            if (TDAStreamerData.simulatorSettings != null && TDAStreamerData.simulatorSettings.buildDatabaseDuringSimulate) return;
            console.warn(System.Reflection.MethodBase.GetCurrentMethod().Name);
            console.warn($"TDAChart.svcDateTime: {TDAChart.svcDateTime}  lastSvcTime:  {lastSvcTime}");
            if (TDAChart.svcDateTime != lastSvcTime)
            {
                //JsConsole.JsConsole.Warn(TDAStreamerJs, TDAChart.svcDateTime.Subtract(lastSvcTime).Milliseconds);
                i++;
                //if (lastSvcTime == null)
                //    lastSvcTime = TDAChart.svcDateTime;
                //else if (TDAChart.svcDateTime.Subtract(lastSvcTime).Milliseconds >= 500)
                //{


                //JsConsole.JsConsole.Warn(TDAStreamerJs, TDAChart.svcDateTime.Subtract(lastSvcTime).Milliseconds);
                //var msg = TDAChart.svcDateTime.ToOADate().ToString();


                sendData();
                lastSvcTime = TDAChart.svcDateTime;
                //Send("TimeAndSales", JsonSerializer.Serialize<Data.TimeSales_Content>(TDAStreamerData.timeAndSales));
                // Send("TimeAndSales", msg);
                StateHasChanged();
                //}
                async Task sendData()
                {
                    console.warn("sendData");


                    /// 3 needs to be set from client
                    var ratioFrames = await TDABookManager.getIncrementalRatioFrames(TDABook.seconds);
                    /// So we don't send the same frame more than once
                    if (ratioFrames[0].dateTime != prevRatioFrameDateTime)
                    {
                        // System.Diagnostics.Debug.Print(ratioFrames[0].dateTime.ToLongTimeString());
                        prevRatioFrameDateTime = ratioFrames[0].dateTime;
                        try
                        {
                            console.warn($"ratioFrames[0].dateTime: {ratioFrames[0].dateTime}  prevRatioFrameDateTime: {prevRatioFrameDateTime}");
                            var msg = JsonSerializer.Serialize<RatioFrame[]>(ratioFrames);
                            await Send("getIncrementalRatioFrames", msg);
                            //console.warn("sendData:" + msg, true);

                            /// TODO: Remove this to rebuild old AllRatioFrames files! !!!!!!!!!!!!!!!!!!!!!!!!!
                            /// 
                            var buildAllRatioFrames =
                                TDAStreamerData.simulatorSettings.isSimulated == null
                                || !(bool)TDAStreamerData.simulatorSettings.isSimulated
                                || TDAStreamerData.simulatorSettings.rebuildAllRatioFrames
                                ;

                            if (buildAllRatioFrames)
                            {
                                await FilesManager.AppendToMessageQueue(symbol, "AllRatioFrames", svcDateTime, msg);
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            //Send("TimeAndSales", JsonSerializer.Serialize<TimeSales_Content>(TDAStreamerData.timeAndSales));
            //sendPrintsData();
            //Send("TimeAndSales", JsonSerializer.Serialize<TimeSales_Content>(TDAStreamerData.timeAndSales));


            //dictTopicCounts["TimeAndSales"] += 1;


        }

        Dictionary<int, DataItem[]> dictPies = new Dictionary<int, DataItem[]>();
        //void sendPrintsData()
        //{
        //    //if (moduloPrints ==0 || TDAStreamerData.timeSales[symbol].Count() % moduloPrints != 0) return;

        //    // Only summarize once every new 2 time and sales
        //    if (TDAStreamerData.timeSales[symbol].Count % 2 != 0) return;

        //    value = TDAPrintsManager.GetPrintsGaugeScore(symbol, ref dictPies);
        //    KeyValuePair<DateTime, double> pair = new KeyValuePair<DateTime, double>(DateTime.Now, value);

        //    Send("GaugeScore", JsonSerializer.Serialize<KeyValuePair<DateTime, double>>(pair));
        //    Send("PrintsPies", JsonSerializer.Serialize<Dictionary<int, DataItem[]>>(dictPies));


        //    //TDAStreamerData.gaugeValues.Add(DateTime.Now, value);
        //    //TDAStreamerData.gaugeValues.RemoveAll((key, val) => key < DateTime.Now.AddSeconds(-printSeconds.Max()));

        //    StateHasChanged();
        //}



        private async Task Timer_ElapsedAsync()
        {
            dateTimeNow = DateTime.Now.ToString(dateFormat);
            sinceLastData = DateTime.Now.Subtract(svcDateTime.AddMilliseconds(-1600)).ToString(timerFormat);
            elapsed = DateTime.Now.Subtract(startedTime).ToString(timerFormat);
            await StateChangedAsync();
        }

        #endregion

        #region Utility
        void LogText(string text)
        {
            if (logStreamer == false) return;

            logTextsb.Insert(0, "\n" + text);
            logText = string.Join('\n', logTextsb.ToString().Split('\n').Take(100));

            StateHasChanged();
        }

        void LogTopic(string topic, string content)
        {
            // Show the topic text (last 1000 lines)
            logTopicsb.Insert(0, "\n" + topic + ":" + content.Replace("\r", "").Replace("\n", ""));
            logTopics = string.Join('\n', logTopicsb.ToString().Split('\n').Take(10));

            // Update topic's Stats count
            dictTopicCounts[topic] += 1;
            StateHasChanged();
        }
        #endregion

        #region WebSocket Responses
        // Called from javascript
        [JSInvokable("TDAStreamerStatus")]
        public void TDAStreamerStatus(string it)
        {
            switch (it)
            {
                case "0": status = "CONNECTING"; break;
                case "1": status = "OPEN"; statusClass = "badge-success"; break;
                case "2": status = "CLOSING"; statusClass = "badge-warning"; break;
                case "3": status = "CLOSED"; statusClass = "badge-danger"; break;
            }
            LogText($"STATUS:{status}");
            StateHasChanged();
        }

        List<string> lstJson = new List<string>();
        private string loginStatus;
        private string adminMsg;
        private bool simulatorStarted;
        private bool simulatorPaused;

        [JSInvokable("TDAStreamerOnMessage")]
        public async Task TDAStreamerOnMessage(string jsonResponse)
        {

            if (TDAStreamerData.simulatorSettings != null && !TDAStreamerData.simulatorSettings.buildDatabaseDuringSimulate)
            {
                await Task.Yield();

                LogText("RECEIVED: " + jsonResponse);
            }

            var fieldedResponse = jsonResponse;
            if (jsonResponse.Contains("\"data\":")) await TDA_Process_Data(jsonResponse);
            else if (jsonResponse.Contains("\"notify\":")) TDA_Process_Heartbeat(jsonResponse);
            else TDA_Process_Admin(jsonResponse);
            StateHasChanged();
        }

        private async Task TDA_Process_Data(string jsonResponse)
        {
            var dataJsonSvcArray = JObject.Parse(jsonResponse)["data"];
            foreach (var svcJsonObject in dataJsonSvcArray)
            {
                var svcName = svcJsonObject["service"].ToString();
                //if (simulatorSettings!=null && simulatorSettings.isSimulated)
                svcDateTime = GetServiceTime(svcJsonObject);

                var svcJson = svcJsonObject.ToString();
                var svcFieldedJson = svcJson;

                /// Decode
                /// Get field names corresponding to field numbers 
                List<string> svcFields = TDAConstants.TDAResponseFields[svcName];
                /// Replace field numbers with field names  
                for (int i = 1; i < svcFields.Count; i++)
                {
                    string sIndex = $"\"{i}\":";
                    svcFieldedJson = svcFieldedJson.Replace(sIndex, $" \"{svcFields[i]}\":");
                }

                if (TDAStreamerData.simulatorSettings != null && !TDAStreamerData.simulatorSettings.buildDatabaseDuringSimulate)
                    LogText("DECODED: " + svcFieldedJson);

                var svcJsonObjectDecoded = JObject.Parse(svcFieldedJson);
                //var contents = svcJsonObjectDecoded["content"].ToString();

                /// Send to connected hub
                /// 
                await TDAStreamerData.captureTdaServiceData(svcFieldedJson);

                clock = TDAChart.svcDateTime.ToString(clockFormat);

                /// Send to message queue
                /// We can replay these messages later from simulator
                /// Need to NOT reswnd 
                dictTopicCounts[svcName] += 1;
                if (!simulate)
                    await FilesManager.SendToMessageQueue(symbol, svcName, svcDateTime, svcFieldedJson);
                //await Send(svcName, svcFieldedJson);
                StateHasChanged();
            }
        }

        private void TDA_Process_Heartbeat(string jsonResponse)
        {
            var dataJsonSvcArray = JObject.Parse(jsonResponse)["notify"];
            var svcJsonObject = dataJsonSvcArray[0];
            if (svcJsonObject["heartbeat"] != null)
            {
                svcDateTime = GetServiceTime(svcJsonObject, "heartbeat");
                LogText("DECODED: " + jsonResponse + " " + svcDateTime.TimeOfDay);
            }
        }

        private void TDA_Process_Admin(string jsonResponse)
        {
            var dataJsonSvcArray = JObject.Parse(jsonResponse)["response"];
            var svcJsonObject = dataJsonSvcArray[0];
            if (svcJsonObject["service"] != null)
            {
                var svcName = ((Newtonsoft.Json.Linq.JValue)svcJsonObject["service"]).Value;
                if (svcName.ToString() == "ADMIN")
                {
                    svcDateTime = GetServiceTime(svcJsonObject);
                    var content = (Newtonsoft.Json.Linq.JObject)svcJsonObject["content"];
                    var code = ((Newtonsoft.Json.Linq.JValue)content["code"]).Value.ToString();
                    var cmd = ((Newtonsoft.Json.Linq.JValue)svcJsonObject["command"]).Value.ToString();
                    admCode = ((Newtonsoft.Json.Linq.JValue)content["msg"]).Value.ToString();
                    admStatus = $"{cmd} at {svcDateTime} : Code {code}";
                    LogText("DECODED: " + jsonResponse + " " + svcDateTime.TimeOfDay);
                }
            }
        }


        private DateTime GetServiceTime(JToken svcJsonObject, string timeField = "timestamp")
        {
            var svcEpochTime = Convert.ToInt64(svcJsonObject[timeField]);
            var svcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0)
                .AddMilliseconds(svcEpochTime).ToLocalTime();
            TDAChart.svcDateTime = svcDateTime;
            clock = svcDateTime.ToString(clockFormat);
            return svcDateTime;
        }
        #endregion

#if UsingSignalHub
        #region SignalR Client

        private async Task Init()
        {
            //#if dev
            //            hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:55540/tdahub").Build();
            //#else
            //            hubConnection = new HubConnectionBuilder().WithUrl("http://tapestreamserver.com/tdahub").Build();
            //#endif

            var serverUrl = Configuration["ServerUrl"];
            var hubUrl = $"{serverUrl}tdahub";

            hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).Build();

            hubConnection.Closed += HubConnection_Closed;
            hubConnection.Reconnected += HubConnection_Reconnected;
            hubConnection.Reconnecting += HubConnection_Reconnecting;

            hubConnection.On("ReceiveMessage", (Action<string, string>)((user, message) =>
            {
                Receive(user, message);
            }));

            //foreach (var name in valuesName.Skip(1))
            //    hubConnection.On(name, (Action<string, string>)((topic, message) => { Receive(topic, message); }));

            hubConnection.On("NASDAQ_BOOK", (Action<string, string>)((topic, message) =>
            {
                Receive(topic, message);
            }));

            hubConnection.On("TIMESALE_EQUITY", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("CHART_EQUITY", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("OPTION", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("QUOTE", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("ACTIVES_NYSE", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("ACTIVES_NASDAQ", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("ACTIVES_OPTIONS", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("getIncrementalRatioFrames", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("GaugeScore", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("BookColsData", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("BookPiesData", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("BookBigPieData", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("SetBookDataSeconds", (Action<string, string>)((topic, message) => { SetBookDataSeconds(message); }));


            /// Start a Connection to the hub
            /// 
            await hubConnection.StartAsync();
            SetHubStatus($"HubConnection {hubUrl} Started");

            StateHasChanged();

        }

        private void SetHubStatus(string msg)
        {
            var color = IsConnected ? "green" : "red";
            quoteSymbol = symbol;
            TDAStreamerData.hubStatusMessage = msg;
            TDAStreamerData.hubStatus = $"./images/{color}.gif";
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
            SetHubStatus("HubConnection Reconnecting");
            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnected(string arg)
        {
            SetHubStatus("HubConnection Reconnected");

            return Task.CompletedTask;
        }

        private Task HubConnection_Closed(Exception arg)
        {
            SetHubStatus("HubConnection Closed");
            hubConnection.StartAsync();
            SetHubStatus("HubConnection  Restarted");
            return Task.CompletedTask;
        }

        private void SetBookDataSeconds(string message)
        {
            TDABook.seconds = Convert.ToInt32(message);
            dictTopicCounts["BookPiesData"] = TDABook.seconds;
            // TDAStreamerJs.confirm("Changed seconds to " + message);
        }

        private void Receive(string user, string message)
        {
            LogTopic(user, message);
            StateHasChanged();
        }

        /// <summary>
        /// Method for this client to Send a message to the hub
        /// </summary>
        /// <returns></returns>
        /// 
        Task Send(string userInput, string messageInput) =>
            hubConnection.SendAsync("SendTopic", userInput, messageInput);

        /// <summary>
        /// Method to test if hub connection is alive
        /// </summary>
        /// 
        public bool IsConnected => hubConnection != null && hubConnection.State == HubConnectionState.Connected;

        public static string quoteSymbol { get; internal set; }

        /// <summary>
        /// Method that will shut down hub if this page is closed
        /// </summary>
        /// 
        public void Dispose()
        {
            if (hubConnection != null && hubConnection.State == HubConnectionState.Connected)
                _ = hubConnection.DisposeAsync();

            if (TDAStreamerData.simulatorSettings != null && TDAStreamerData.simulatorSettings.isSimulated != null && TDAStreamerData.simulatorSettings.isSimulated == true)
                Simulator_Stop();
        }

        #endregion
#endif
    }
}

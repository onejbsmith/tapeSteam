#undef tracing
#undef dev
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
using JSconsoleExtensionsLib;
using Alpaca.Markets;
using Alpaca.Markets.Extensions;
using tapeStream.Client.Shared;
using tapeStream.Client.Data;
using MatBlazor;



namespace tapeStream.Client.Pages
{
    public partial class TestPage
    {

        EventConsole console;

        private AlpacaClient alpacaClient = new AlpacaClient();

        private string lstAlpacaTrades = "Started";
        private List<string> _logAlpacaTrades = new List<string>();

        public static List<IAccountActivity> lstActivity = new List<IAccountActivity>();

        public List<string> logAlpacaTrades
        {
            get { return _logAlpacaTrades; }
            set
            {
                _logAlpacaTrades = value;
                lstAlpacaTrades = string.Join('\n', _logAlpacaTrades);
                StateHasChanged();

            }
        }
        //[Inject]
        //protected IMatToaster Toaster { get; set; }

        private const String KEY_ID = "PKF2HF1AQFTA212201YK";

        private const String SECRET_KEY = "76iwKMUD0A5ajPGRdJvOxZFNrH17aXhpVSpFmunj";

        [Inject]
        Blazored.LocalStorage.ISyncLocalStorageService localStorage { get; set; }


        #region Variables
        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject] Microsoft.JSInterop.IJSRuntime jsruntime { get; set; }

        [Inject] BlazorTimer timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }

        Radzen.Blazor.RadzenGrid<Trade> radzenGrid;

        List<Trade> currentTrades = new List<Trade>();
        public static bool isPaused;
        public static bool isSimulated;
        public static bool isChartFromHub;
        public static bool isTradingOn;
        public static bool isTradingOnClient;
        public static bool redraw = true;

        string symbol = "QQQ";

        string hubUrl = "";

        string posnColor => AlpacaClient.currentPosition == null || AlpacaClient.currentPosition.Quantity > 0 ? "green" : "red";

        public List<string> buyFields = new List<string>()
        {
            "buysTradeSizes",
             "buysSummedAboveBelowShort",
           "buysPriceCount",

            "asksBookSizes",
            "buysSummedAboveBelowMed",
            "buysSummedAboveBelowLong",

            "buysBelow",
            "buysInSpread",
            "buysAbove",

            "buysSummedInSpreadShort",
            "buysSummedInSpreadMed",
            "buysSummedInSpreadLong"

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

        public List<long> lstClosing { get; private set; } = new List<long>() { 0 };
        public List<long> lstOpening { get; private set; } = new List<long>() { 0 };
        public List<long> lstClosed { get; private set; } = new List<long>() { 0 };
        public List<long> lstOpened { get; private set; } = new List<long>() { 0 };
        public static bool tradesFromHub { get; internal set; }
        #endregion

        #region Dialog Vars
        Timer timerBookColumnsCharts = new Timer(1000);
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


        void OnClick(string buttonName)
        {
            // jsruntime.log($"{buttonName} clicked");
        }

        DateTime toDate;
        internal string toTime;

        static IAlpacaTradingClient client = null;

        protected override async Task OnInitializedAsync()
        {
            foreach (var name in CONSTANTS.valuesName)
                dictTopicCounts.Add(name, 0);



            //bookColData = await bookColumnsService.getBookColumnsData(ChartConfigure.seconds);
            AlpacaClient.jsruntime = jsruntime;

            AlpacaClient.OnAlpacaChanged += NavMenu_OnAlpacaChanged;
            AlpacaClient.OnStatusesChanged += AlpacaClient_OnStatusesChanged;

            /// TODO: Get symbol and date derived
            /// 
            var todaysDate = Math.Floor(DateTime.Now.ToOADate());

            LinesChart.toDateTime = DateTime.Now.ToString(LinesChart.dateFormat); //  "2020-12-31-0940-00";
#if dev
            string serverUrl = "https://localhost:44363/";
#else
            string serverUrl = "http://tda2tapeStream.io/";
#endif
            var userEnteredDateTime = await jsruntime.prompt($"Server Url: {serverUrl}\nTo Date Time:", LinesChart.toDateTime);
            LinesChart.toDateTime = userEnteredDateTime;
            toDate = DateTime.ParseExact(userEnteredDateTime, LinesChart.dateFormat, null).AddSeconds(30);
            LinesChart.fromDateTime = toDate.AddMinutes(-10).ToString(LinesChart.dateFormat);

            isSimulated = toDate.Date < DateTime.Now.Date
                || (toDate.Date == DateTime.Now.Date && DateTime.Now.TimeOfDay.Subtract(toDate.TimeOfDay).TotalHours > 1)
                ; // Date before today is a simumlation or today more than an hour ago

            TradesService.jsruntime = jsruntime;

            currentTrades = await TradesService.getTrades(toDate.ToString("yyyy-MM-dd"), 0);

            // just get trades up to start time
            currentTrades = currentTrades.Where(trade => trade.DateTime <= toDate).ToList();

            if (currentTrades.Count > 1)
            {
                var lastTrade = currentTrades.First();

                var nextTrade = new Trade()
                {
                    Trades = currentTrades.Count + 1,
                    Symbol = lastTrade.Symbol,
                    TimeStart = lastTrade.TimeEnd,
                    Type = lastTrade.Type == "C" ? "P" : "C",
                    PrevMark = lastTrade.MarkPrice
                };
                currentTrades.Insert(0, nextTrade);

                AlpacaClient.lastAlpacaTradeId = (int)nextTrade.Trades;

            }

            // get the last trade number to check in timer loop
            //jsruntime.alert(currentTrades.Count.ToString());

            EventConsole.localStorage = localStorage;
            console.Init();

            await HubConnection_Initialize();
            jsruntime.title("Hello");

            InitializeTimers();

            dictTopicCounts["BookPiesData"] = TDABook.seconds;


            //if (mode == "simulate")
            //{ }
            //else
            var frames = await bookColumnsService.getAllRatioFrames(symbol, todaysDate, jsruntime);
            allRatioFrames = frames.Where(frame => frame[0].markPrice > 0).ToList();


            //await jsruntime.InvokeAsync<string>("BlazorSetTitle", new object[] { ]o Dali!" });
        }

        private void AlpacaClient_OnStatusesChanged()
        {

            StateHasChanged();
        }

        private async void NavMenu_OnAlpacaChanged()
        {
            if (AlpacaClient.useAlpaca == true)
            {

                isLoadingAlpaca = true;
                redraw = false;
                await alpacaClient.StartPaperClient();

                StateHasChanged();
            }
            else
            {
                jsruntime.alert("Alpaca Paper Trading turned OFF.");
            }
            console.Log($"AlpacaClient OnAlpacaChanged {AlpacaClient.useAlpaca}");
        }

        ////string buyingPower, portfolioValue;
        ////IPosition currentPosition;
        ////static IReadOnlyList<IOrder> orders = new List<IOrder>();

        ////static int positionQuantity = 0;
        ////string position = $"{positionQuantity}--{orders.Count}";
        ////Decimal positionValue = 0;

        //private async Task Alpaca_StartPaperClient()
        //{
        //    //isLoadingAlpaca = true;
        //    redraw = false;
        //    client = Environments.Paper
        //        .GetAlpacaTradingClient(new SecretKey(KEY_ID, SECRET_KEY));

        //    var clock = await client.GetClockAsync();

        //    if (clock != null)
        //    {
        //        await Alpaca_GetAccountData();
        //        await Alpaca_GetCurrentPosition();

        //        await Alpaca_GetOrders();

        //        // redraw = true;

        //        //jsruntime.alert(string.Format(@"Alpaca Paper Trading turned ON. " +
        //        //    "\n\nTimestamp: {0}" +
        //        //    "\nNextOpen: {1}" +
        //        //    "\nNextClose: {2}" +
        //        //    "\n\nBuying Power: {3}" +
        //        //    "\nPortfolio Value: {4}" +
        //        //    "\n\nPosition Quantity: {5}" +
        //        //    "\nPosition MarketValue: {6}"
        //        //    ,
        //        //    clock.TimestampUtc.ToLocalTime(),
        //        //    clock.NextOpenUtc.ToLocalTime(),
        //        //    clock.NextCloseUtc.ToLocalTime(),
        //        //    buyingPower,
        //        //    portfolioValue,
        //        //    positionQuantity,
        //        //    positionValue.ToString("c2")
        //        //    ));


        //    }
        //}

        //private async Task Alpaca_GetAccountData()
        //{
        //    var account = await client.GetAccountAsync();
        //    AlpacaClient.buyingPower = account.BuyingPower.ToString("c2");
        //    AlpacaClient.portfolioValue = account.Equity.ToString("c2");
        //}

        //private async Task Alpaca_GetCurrentPosition()
        //{
        //    //positionQuantity = 0;
        //    //positionValue = 0;
        //    try
        //    {
        //        AlpacaClient.currentPosition = await client.GetPositionAsync(symbol);
        //        AlpacaClient.positionQuantity = AlpacaClient.currentPosition.Quantity;
        //        AlpacaClient.positionValue = AlpacaClient.currentPosition.MarketValue;
        //    }
        //    catch (Exception)
        //    {
        //        // No position exists. This exception can be safely ignored.
        //    }
        //    StateHasChanged();
        //}
        //private async Task Alpaca_AwaitMarketOpen()
        //{
        //    while (!(await client.GetClockAsync()).IsOpen)
        //    {
        //        await Task.Delay(60000);
        //    }
        //}

        //private async Task Alpaca_CancelOrders()
        //{
        //    if (client == null) return;

        //    await Alpaca_GetOrders();

        //    var result = await jsruntime.confirm($"Are you sure you want to cancel {orders.Count} orders?");
        //    if (result == false) return;
        //    foreach (var order in orders)
        //    {
        //        await client.DeleteOrderAsync(order.OrderId);
        //    }

        //    await Alpaca_GetOrders();
        //}

        //private async Task Alpaca_ClosePosition()
        //{
        //    if (client == null) return;

        //    //orders = await client.ListOrdersAsync(new ListOrdersRequest());
        //    var result = await jsruntime.confirm($"Are you sure you want to close a {positionQuantity} share position?");
        //    if (result == false) return;

        //    var orderType = positionQuantity > 0 ? "C" : "P";
        //    var trade = new Trade()
        //    {
        //        Trades = 0,
        //        Type = orderType
        //    };


        //    try
        //    {
        //        await Alpaca_CloseCurrentPosition(trade);
        //    }
        //    catch
        //    {
        //    }
        //    await Alpaca_GetCurrentPosition();
        //    await Alpaca_GetAccountData();
        //    await Alpaca_GetOrders();


        //}

        //private async Task Alpaca_GetOrders()
        //{
        //    orders = await client.ListOrdersAsync(new ListOrdersRequest());
        //    StateHasChanged();
        //}

        //private Guid lastTradeId = Guid.NewGuid();
        //// Submit a market order if quantity is not zero.
        //private async Task<Guid> Alpaca_SubmitMarketOrder(int quantity, OrderSide side)
        //{
        //    //if (quantity == 0)
        //    if (quantity == 0 || isSimulated == true)
        //    {
        //        return Guid.Empty;
        //    }
        //    //Console.WriteLine($"Submitting {side} order for {quantity} shares.");
        //    try
        //    {
        //        var order = await client.PostOrderAsync(side.Market(symbol, quantity));
        //        lastTradeId = order.OrderId;
        //    }
        //    catch (Exception ex)
        //    {
        //        jsruntime.alert(ex.Message);
        //    }
        //    return lastTradeId;
        //}

        //private async Task Alpaca_ClosePositionAtMarket()
        //{
        //    try
        //    {
        //        var positionQuantity = (await client.GetPositionAsync(symbol)).Quantity;
        //        await client.PostOrderAsync(
        //            OrderSide.Sell.Market(symbol, positionQuantity));
        //    }
        //    catch (Exception)
        //    {
        //        // No position to exit.
        //    }
        //}
        //private async Task Alpaca_CloseCurrentPosition(Trade lastTrade)
        //{
        //    var lastTradeType = lastTrade.Type;                      /// The last completed trade

        //    if (TDAStreamerData.useAlpaca == true)
        //    {
        //        /// Close last position                              
        //        if (lastTradeType == "C")                              ///
        //        {                                                       /// Close long position
        //            if (currentPosition.Quantity > 0)
        //            {/// Sell current position  
        //                await Alpaca_SubmitMarketOrder(currentPosition.Quantity, OrderSide.Sell);
        //                //jsruntime.alert($"Close position Sell {currentPosition.Quantity}");
        //                logAlpacaTrades.Insert(0, $"{lastTrade.DateTime.ToShortTimeString()} Closed Long position #{lastTrade.Trades} SELL {currentPosition.Quantity}");
        //                //Toaster.Add($"Closed Long position #{lastTrade.Trades} SELL {currentPosition.Quantity}", MatToastType.Warning, "Alpaca Submit Market Order", "remove_shopping_cart");

        //            }
        //        }
        //        else
        //        {                                                       /// Close short position
        //            if (currentPosition.Quantity < 0)                   /// Buy back current position  
        //            {
        //                await Alpaca_SubmitMarketOrder(-currentPosition.Quantity, OrderSide.Buy);
        //                //jsruntime.alert($"Close position Buy {-currentPosition.Quantity}");
        //                //Toaster.Add($"Closed Short position #{lastTrade.Trades} BUY {currentPosition.Quantity}", MatToastType.Danger, "Alpaca Submit Market Order", "remove_shopping_cart");
        //                logAlpacaTrades.Insert(0, $"{lastTrade.DateTime.ToShortTimeString()} Closed Short position #{lastTrade.Trades} BUY {currentPosition.Quantity}");

        //            }
        //        }
        //    }
        //}
        private void InitializeTimers()
        {
#if tracing
            jsruntime.error($"InitializeTimers");
#endif

            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender, e);
            timerBookColumnsCharts.Start();
        }

        Trade nextTrade = new Trade();
        public static bool isLoadingAlpaca;

        static int counter = 0;
        static int counter2 = 0;
        static int counter3 = 0;
        static int nextTradeId;

        /// <summary>
        /// This makes the UI responsive. Events picked up every half second
        /// </summary>
        private async Task TimerBookColumnsCharts_Elapsed(object sender, ElapsedEventArgs e)
        {

            // instead of relading every second, should check 
            try
            {
                await Alpaca_GetStatus();

                // jsruntime.warn("");
                var latestTrades = await TradesService.getTrades(toTime);
                //jsruntime.error ( $"{toTime} = { latestTrades.Count}:{currentTrades.Count -1 }" );

                if (latestTrades.Count > currentTrades.Count - 1)           /// We have a new trade
                {
                    var lastTrade = latestTrades.First();                   /// One trade but comes in an array
                                                                            /// 
                    counter += 1;
                    ///
                    var nextTrade = new Trade()                             /// 
                    {
                        Trades = latestTrades.Count + 1,
                        Symbol = lastTrade.Symbol,
                        TimeStart = lastTrade.TimeEnd,
                        Type = lastTrade.Type == "C" ? "P" : "C",           /// Switch type
                        PrevMark = lastTrade.MarkPrice,
                        DateTime = DateTime.Now
                    };
                    latestTrades.Insert(0, nextTrade);

                    nextTradeId = (int)nextTrade.Trades;  /// Show next trade
                    StateHasChanged();

                    if (AlpacaClient.useAlpaca == true && AlpacaClient.isTradingOnClient == true && nextTradeId > AlpacaClient.lastAlpacaTradeId)
                    {
                        counter2 += 1;

                        await Alpaca_Trade(lastTrade, nextTrade);

                        //await Alpaca_GetStatus();

                        AlpacaClient.lastAlpacaTradeId = (int)nextTrade.Trades;

                        await Alpaca_GetStatus();

                    }


                    currentTrades.Clear();
                    currentTrades.AddRange(latestTrades);

                    await radzenGrid.Reload();
                    //await InvokeAsync(() => StateHasChanged());
                    //jsruntime.alert(currentTrades.Count.ToString());
                }
            }
            catch
            {
            }

            //jsruntime.warn("");
            //jsruntime.error( $"TimerBookColumnsCharts_Elapsed");


            //Data.AlpacaClient.hubStatus = $"./images/blue.png";
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

        private async Task Alpaca_GetStatus()
        {
            if (AlpacaClient.useAlpaca == true)
            {
                try
                {
                    await alpacaClient.GetAccountData();
                }
                catch { }

                // jsruntime.warn("GetAccountData");
                try
                {
                    await alpacaClient.GetCurrentPosition();
                }
                catch { }


                try
                {
                    // jsruntime.warn("GetCurrentPosition");
                    await alpacaClient.GetOrders(clientId);
                    // jsruntime.warn("GetOrders");
                }
                catch { }

                try
                {
                    await alpacaClient.GetAccountActivity(DateTime.Now);
                }
                catch { }

                StateHasChanged();

                /// Trade signal from server
                if (AlpacaClient.isTradingOn == true && AlpacaClient.isTradingOnClient == false && AlpacaClient.tradesFromHub==false)
                {
                    //jsruntime.warn("Trade Start!");
                    var trades = await TradesService.getLastTrades();
                    //jsruntime.warn($"Trade Type '{trades[0].Type }'");
                    if (trades[0].Type != "")
                    {
                        counter2 += 1;
                        try
                        {
                            await Alpaca_Trade(trades[0], trades[1]);
                        }
                        catch
                        { }
                    }
                }


            }
        }

        public static string clientId;
        private async Task Alpaca_Trade(Trade lastTrade, Trade nextTrade)
        {
            try
            {
                var lastTradeNum = (long)lastTrade.Trades;
                jsruntime.warn($"Before Closing lastTradeNum={lastTradeNum}");
                jsruntime.table(lastTrade);


                if (!lstClosing.Contains(lastTradeNum)) // Haven't already attemped to close
                {
                ClosePositionsFirst:
                    var posn = 999;
                    try
                    {
                        posn = AlpacaClient.currentPosition.Quantity;
                    }
                    catch { }

                    if (posn != 999)
                    {
                        lstClosing.Add(lastTradeNum);
                        jsruntime.warn($"Closing lastTradeNum={lastTradeNum}");
                        var side = lastTrade.Type == "C" ? "Long" : "Short";

                        try
                        {
                            //if (AlpacaClient.orders.Count > 0)
                            console.Log($"AlpacaClient Closing Current {side} Position  #{lastTrade.Trades} {lastTrade.Type} {posn}");
                            await alpacaClient.CloseCurrentPosition(lastTrade);

                            lstClosed.Add((long)lastTrade.Trades);
                            console.Log($"AlpacaClient Closed Current {side} Position  #{lastTrade.Trades} {lastTrade.Type} {posn}");
                        }
                        catch { }
                    }
                    ///// So can only open new position once old position closed
                    ///// 
                    console.Log($"AlpacaClient Canceling {AlpacaClient.orders.Count} Outstanding Orders");
                    await Alpaca_CancelOutstandingOrders();
                    /// and all open orders are canceled

                    jsruntime.warn($"About to Open nextTrade={nextTrade.Trades}");
                    await alpacaClient.GetCurrentPosition();

                    /// Make sure FLAT 
                    if (AlpacaClient.positionQuantity > 0)
                        goto ClosePositionsFirst;

                    await OpenNewPosition(nextTrade);

                }
            }
            catch
            {
            }

            /// Wait until


            async Task OpenNewPosition(Trade nextTrade)
            {
                /// Open new position
                var newPositionShares = 100;
                var nextTradeNum = (long)nextTrade.Trades;

                jsruntime.warn($"nextTradeNum={nextTradeNum}");
                jsruntime.table(nextTrade);

                jsruntime.warn($"Before Opening nextTrade={nextTradeNum}");

                clientId = nextTrade.DateTime.ToString($"yyyy-MM-dd-{nextTradeNum}");
                decimal stopDollars = 0.05M;

                if (nextTrade.Type == "C")
                {/// Open long position

                    if (!lstOpening.Contains(nextTradeNum))
                    {

                        jsruntime.warn($"Opening nextTrade={nextTradeNum} {nextTrade.Type}");

                        console.Log($"AlpacaClient Opening Long position #{nextTradeNum} BUY {newPositionShares}");
                        lstOpening.Add(nextTradeNum);
                        await alpacaClient.SubmitMarketOrder(newPositionShares, OrderSide.Buy, clientId, stopDollars);

                        console.Log($"AlpacaClient Opened Long position #{nextTrade.Trades} BUY {newPositionShares}");

                        //if (lstOpening.Last() == nextTradeNum) // Don't list as opened if removed from lstOpening
                        //{
                        //    lstOpened.Add(nextTradeNum);
                        //    //Toaster.Add($"Opened Long position #{nextTrade.Trades} BUY {newPositionShares}",MatToastType.Success,"Alpaca Submit Market Order", "add_shopping_cart");
                        //    //logAlpacaTrades.Insert(0, $"{nextTrade.DateTime.ToShortTimeString()} Opened Long position #{nextTrade.Trades} BUY {newPositionShares}");
                        //}
                    }
                }
                else
                {/// Open short position

                    if (!lstOpening.Contains(nextTradeNum))
                    {
                        jsruntime.warn($"Opening nextTrade={nextTradeNum} {nextTrade.Type}");

                        console.Log($"AlpacaClient Opening Short position #{nextTradeNum} SELL {newPositionShares}");
                        lstOpening.Add(nextTradeNum);
                        await alpacaClient.SubmitMarketOrder(newPositionShares, OrderSide.Sell, clientId, stopDollars);
                        console.Log($"AlpacaClient Opened Short position #{nextTradeNum} SELL {newPositionShares}");
                        //logAlpacaTrades.Insert(0, $"{nextTrade.DateTime.ToShortTimeString()} Opened Short position #{nextTrade.Trades} SELL {newPositionShares}");
                        //Toaster.Add($"Opened Short position #{nextTrade.Trades} SELL {newPositionShares}", MatToastType.Primary, "Alpaca Submit Market Order", "add_shopping_cart");

                    }
                }

                if (lstOpening.Last() == nextTradeNum) // Don't list as opened if removed from lstOpening
                {
                    lstOpened.Add(nextTradeNum);
                }
            }

        }

        private async Task Alpaca_CancelOutstandingOrders()
        {
            /// And there are no other open orders
            if (lstOpening.Last() > lstOpened.Last())
            {


                jsruntime.warn($"About to Cancel Orders");
                // We have an open order 
                // Cancel all open orders
                //await alpacaClient.GetOrders();
                //if (AlpacaClient.orders.Count > 0)
                try
                {
                    console.Log($"AlpacaClient Canceling All Orders");
                    await alpacaClient.CancelAllOrders();
                    console.Log($"AlpacaClient Canceled All Orders");
                }
                catch
                { }

                while (lstOpening.Last() > lstOpened.Last())
                    lstOpening.Remove(lstOpening.Last());



            }
        }


        #region Hub Events
        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();

            if (client != null)
                client.Dispose();
        }

        public async System.Threading.Tasks.Task HubConnection_Initialize()
        {
#if tracing
            jsruntime.warn($"HubConnection_Initialize: ");
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

                var serverUrl = Configuration["tda2tapeStreamUrl"];
                //jsruntime.alert($"Server Url {serverUrl}");
                hubUrl = $"{serverUrl}tdahub";
                hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).Build();

#if tracing
                jsruntime.warn($"hubConnection: {hubConnection.State}");
#endif
                /// Set up Hub Subscriptions -- Don't really need any anymore
                /// Perhaps get messages of counts? or ,essage to refresh to avoid using a timer 
                hubConnection.On("getIncrementalRatioFrames", (Action<string, string>)((topic, message) =>
                {
                    Chart_ProcessHubData(topic, message);

                }));


                hubConnection.On("Trade", (Action<string, string>)((topic, message) =>
                {
                    Trade_ProcessHubData(topic, message);

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

        private void Trade_ProcessHubData(string topic, string message)
        {

            if (AlpacaClient.isTradingOn == true && AlpacaClient.isTradingOnClient == false && AlpacaClient.tradesFromHub == true)
            {
                //jsruntime.warn("Trade Start && AlpacaClient.isTradingOnClient == false
                //var trades = await TradesService.getLastTrades();
                var lastTrade = System.Text.Json.JsonSerializer.Deserialize<Trade>(message);
                    counter3 += 1;

                var nextTrade = new Trade() { Type = "" };
                /// 
                if (lastTrade.Type != "")
                    nextTrade = new Trade()                             /// 
                    {
                        Trades = lastTrade.Trades + 1,
                        Symbol = lastTrade.Symbol,
                        TimeStart = lastTrade.TimeEnd,
                        Type = lastTrade.Type == "C" ? "P" : "C",           /// Switch type
                        PrevMark = lastTrade.MarkPrice,
                        DateTime = DateTime.Now
                    };

                //jsruntime.warn($"Trade Type '{trades[0].Type }'");
                if (lastTrade.Type != "")
                {
                    try
                    {
                        ExecuteTrade(lastTrade, nextTrade);
                    }
                    catch
                    { }
                }
            }

            async Task ExecuteTrade(Trade lastTrade, Trade nextTrade)
            {
                await Alpaca_Trade(lastTrade, nextTrade);
            }
        }



        private void Chart_ProcessHubData(string topic, string message)
        {
            var newFrameWhole = System.Text.Json.JsonSerializer.Deserialize<Models.FrameWhole>(message);


            if (!isChartFromHub) return;

            /// Send to all the Linecharts
            /// 
            LinesChart.newFrame = newFrameWhole;

            /// need to add to array and then CALL AddPoint

            //}
            //else
            //{
            //    /// Resetting ratioFrames to 1 item appends new data points to the chart
            //ratioFrames = new List<RatioFrame>();
            //ratioFrames.Add(newRatioFrame);
            //StateHasChanged();
            ////}

            /// 
            //countIn++;
            //if (countIn % 10 == 0)
            //{
            //    ratioFrames = new List<RatioFrame>();
            //    ratioFrames.AddRange(allRatioFrames);
            //}

            //Receive(topic, message);
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
            jsruntime.warn($"HubConnection_Reconnecting: {hubConnection.State}");
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
            jsruntime.warn($"HubConnection_Closed: {hubConnection.State}");
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
            jsruntime.warn($"HubConnection_Reconnected: {hubConnection.State}");
#endif       
        }

        void Receive(string topic, string content)
        {
            //jsruntime.warn($"Receive: {topic}:{content}");
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

using Alpaca.Markets;
using JSconsoleExtensionsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tapeStream.Shared.Data
{
    public class AlpacaClient
    {

        static public event Action OnAlpacaChanged;
        private static void AlpacaChanged() => OnAlpacaChanged?.Invoke();

        static public event Action OnStatusesChanged;
        private static void StatusChanged() => OnStatusesChanged?.Invoke();


        static public event Action OnAlpacaActivityChanged;
        private static void AlpacaActivityChanged() => OnAlpacaActivityChanged?.Invoke();

        private static bool _useAlpaca;
        public static bool useAlpaca
        {
            get { return _useAlpaca; }
            set
            {
                _useAlpaca = value;
                AlpacaChanged();
            }
        }


        private static bool _isTradingOn;

        public static bool isTradingOn
        {
            get { return _isTradingOn; }
            set
            {
                _isTradingOn = value;
                StatusChanged();
            }
        }


        private static bool _isTradingOnClient;

        public static bool isTradingOnClient
        {
            get { return _isTradingOnClient; }
            set
            {
                _isTradingOnClient = value;
                StatusChanged();
            }
        }


        private static int _lastAlpacaTradeId;

        public static int lastAlpacaTradeId
        {
            get { return _lastAlpacaTradeId; }
            set
            {
                _lastAlpacaTradeId = value;
                StatusChanged();
            }
        }



        public static string symbol = "QQQ";

        public static string lstAlpacaTrades { get; set; } = "Started";
        private List<string> _logAlpacaTrades = new List<string>();

        public static Microsoft.JSInterop.IJSRuntime jsruntime { get; set; }

        public List<string> logAlpacaTrades
        {
            get { return _logAlpacaTrades; }
            set
            {
                _logAlpacaTrades = value;
                lstAlpacaTrades = string.Join('\n', _logAlpacaTrades);
                StatusChanged();

            }
        }


        private const String KEY_ID = "PKF2HF1AQFTA212201YK";

        private const String SECRET_KEY = "76iwKMUD0A5ajPGRdJvOxZFNrH17aXhpVSpFmunj";

        static IAlpacaTradingClient client = null;


        public static string buyingPower;
        public static string portfolioValue;

        public static IPosition currentPosition { get; set; }
        public static List<IPosition> currentPositions { get; set; } = new List<IPosition>();

        public static IReadOnlyList<IOrder> orders = new List<IOrder>();
        public static IReadOnlyList<IOrder> closedOrders = new List<IOrder>();
        public static IReadOnlyList<IOrder> LastOrders = new List<IOrder>();

        public static int positionQuantity = 0;
        public static string position = $"{positionQuantity}--{orders.Count}";
        public static Decimal positionValue = 0;

        public static IReadOnlyList<IAccountActivity> lstActivity = new List<IAccountActivity>();

        public async Task StartPaperClient()
        {
            try
            {
                //isLoadingAlpaca = true;
                //redraw = false;
                client = Environments.Paper
                    .GetAlpacaTradingClient(new SecretKey(KEY_ID, SECRET_KEY));

                var clock = await client.GetClockAsync();

                if (clock != null)
                {
                    //lstActivity = await GetAccountActivity(DateTime.Now);
                    await GetAccountData();
                    await GetCurrentPosition();

                    await GetOrders();

                    // Prepare the websocket connection and subscribe to trade_updates.
#if alpacaStreamingClient
                    var alpacaStreamingClient = Environments.Paper
                        .GetAlpacaStreamingClient(new SecretKey(KEY_ID, SECRET_KEY));
                    await alpacaStreamingClient.ConnectAndAuthenticateAsync();
                    alpacaStreamingClient.OnTradeUpdate += AlpacaStreamingClient_OnTradeUpdate; ;
#endif
                    // redraw = true;

                    //jsruntime.alert(string.Format(@"Alpaca Paper Trading turned ON. " +
                    //    "\n\nTimestamp: {0}" +
                    //    "\nNextOpen: {1}" +
                    //    "\nNextClose: {2}" +
                    //    "\n\nBuying Power: {3}" +
                    //    "\nPortfolio Value: {4}" +
                    //    "\n\nPosition Quantity: {5}" +
                    //    "\nPosition MarketValue: {6}"
                    //    ,
                    //    clock.TimestampUtc.ToLocalTime(),
                    //    clock.NextOpenUtc.ToLocalTime(),
                    //    clock.NextCloseUtc.ToLocalTime(),
                    //    buyingPower,
                    //    portfolioValue,
                    //    positionQuantity,
                    //    positionValue.ToString("c2")
                    //    ));


                }

            }
            catch (Exception ex)
            {

                jsruntime.alert(ex.Message);
            }
        }
#if alpacaStreamingClient
        private void AlpacaStreamingClient_OnTradeUpdate(ITradeUpdate update)
        {
            //  throw new NotImplementedException();
            lstTradeUpdates.Add(update);
            StatusChanged();
        }
#endif

        public static IAccount account { get; set; }
        public static bool tradesFromHub { get; set; }
        public static  List<ITradeUpdate> lstTradeUpdates { get; private set; }

        public async Task GetAccountData()
        {
            account = await client.GetAccountAsync();
            buyingPower = account.BuyingPower.ToString("c2");
            portfolioValue = account.Equity.ToString("c2");
            StatusChanged();

        }

        public async Task<IReadOnlyList<IAccountActivity>> GetAccountActivity(DateTime date)
        {

            try
            {

                lstActivity = await client.ListAccountActivitiesAsync(new AccountActivitiesRequest(AccountActivityType.Fill));

                //return lst;
                // lstActivity = accountActivity.ToList();
                //.OrderBy(act => act.ActivityDate)
                //.TakeLast(20)
                //.ToList()
                //;
            }
            catch
            {
            }
            StatusChanged();

            return lstActivity;
        }

        private class Position : IPosition
        {
            public Guid AccountId => new Guid();

            public Guid AssetId => new Guid();


            public string Symbol => "";

            public Exchange Exchange => new Exchange();

            public AssetClass AssetClass => new AssetClass();

            public decimal AverageEntryPrice => 0;

            public int Quantity => 0;

            public PositionSide Side => new PositionSide();

            public decimal MarketValue => 0;

            public decimal CostBasis => 0;

            public decimal UnrealizedProfitLoss => 0;

            public decimal UnrealizedProfitLossPercent => 0;

            public decimal IntradayUnrealizedProfitLoss => 0;

            public decimal IntradayUnrealizedProfitLossPercent => 0;

            public decimal AssetCurrentPrice => 0;

            public decimal AssetLastPrice => 0;

            public decimal AssetChangePercent => 0;
        }

        public async Task GetCurrentPosition()
        {
            try
            {
                positionQuantity = 0;
                positionValue = 0;
                currentPosition = await client.GetPositionAsync(symbol);
                positionQuantity = currentPosition.Quantity;
                positionValue = currentPosition.MarketValue;

                //var positions = await client.ListPositionsAsync();
                //if (positions.Count > 0)
                //{
                //    currentPosition = positions.FirstOrDefault();
                //}
                //else
                //{
                //    currentPosition = new Position();
                //}
                StatusChanged();
            }
            catch
            {
                // currentPosition = new Position();
                // No position exists. This exception can be safely ignored.
            }


        }


        public async Task AwaitMarketOpen()
        {
            while (!(await client.GetClockAsync()).IsOpen)
            {
                await Task.Delay(60000);
            }
        }

        public async Task CancelOrders()
        {
            if (client == null) return;

            await GetOrders();
            var orders = await client.ListOrdersAsync(new ListOrdersRequest());

            var result = await jsruntime.confirm($"Are you sure you want to cancel {orders.Count} orders?");
            if (result == false) return;
            foreach (var order in orders)
            {
                await client.DeleteOrderAsync(order.OrderId);
            }

            await GetOrders();
        }

        public async Task CancelAllOrders()
        {
            // First, cancel any existing orders so they don't impact our buying power.
            try
            {
                var orders = await client.ListOrdersAsync(new ListOrdersRequest());
                jsruntime.warn($"About to Cancel {orders.Count} Orders");

                foreach (var order in orders)
                {
                    jsruntime.warn($"Canceling order {order.OrderId}");
                    await client.DeleteOrderAsync(order.OrderId);
                    jsruntime.warn($"Canceled order {order.OrderId}");
                }
                jsruntime.warn($"Canceled {orders.Count} Orders");
            }
            catch { }
        }

        public async Task ClosePosition()
        {
            if (client == null) return;

            //orders = await client.ListOrdersAsync(new ListOrdersRequest());
            var result = await jsruntime.confirm($"Are you sure you want to close a {positionQuantity} share position?");
            if (result == false) return;

            var orderType = positionQuantity > 0 ? "C" : "P";
            var trade = new Trade()
            {
                Trades = 0,
                Type = orderType
            };


            try
            {
                await CloseCurrentPosition(trade);
            }
            catch
            {
            }
            await GetCurrentPosition();
            await GetAccountData();
            await GetOrders();


        }

        public async Task CloseCurrentPosition(Trade lastTrade)
        {
            var lastTradeType = lastTrade.Type;                      /// The last completed trade

            //if (TDAStreamerData.useAlpaca == true)
            //{
            /// Close last position                              
            if (lastTradeType == "C")                              ///
            {                                                       /// Close long position
                if (currentPosition.Quantity > 0)
                {/// Sell current position  


                    await SubmitMarketOrder(currentPosition.Quantity, OrderSide.Sell,"Close Long");
                    //jsruntime.alert($"Close position Sell {currentPosition.Quantity}");
                    //logAlpacaTrades.Insert(0, $"{lastTrade.DateTime.ToShortTimeString()} Closed Long position #{lastTrade.Trades} SELL {currentPosition.Quantity}");
                    //Toaster.Add($"Closed Long position #{lastTrade.Trades} SELL {currentPosition.Quantity}", MatToastType.Warning, "Alpaca Submit Market Order", "remove_shopping_cart");

                }
            }
            else
            {                                                       /// Close short position
                if (currentPosition.Quantity < 0)                   /// Buy back current position  
                {
                    await SubmitMarketOrder(-currentPosition.Quantity, OrderSide.Buy, "Close Short");
                    //jsruntime.alert($"Close position Buy {-currentPosition.Quantity}");
                    //Toaster.Add($"Closed Short position #{lastTrade.Trades} BUY {currentPosition.Quantity}", MatToastType.Danger, "Alpaca Submit Market Order", "remove_shopping_cart");
                    //logAlpacaTrades.Insert(0, $"{lastTrade.DateTime.ToShortTimeString()} Closed Short position #{lastTrade.Trades} BUY {currentPosition.Quantity}");

                }
            }
            //}
        }
        public async Task GetOrders(string clientId = "")
        {
            try
            {
                closedOrders = await client.ListOrdersAsync(
                new ListOrdersRequest
                {
                    LimitOrderNumber = 100,
                    OrderStatusFilter = OrderStatusFilter.Closed
                });

                closedOrders = closedOrders.OrderByDescending(order => order.SubmittedAtUtc).ToList();

                orders = await client.ListOrdersAsync(
                new ListOrdersRequest
                {
                    LimitOrderNumber = 100,
                    OrderStatusFilter = OrderStatusFilter.Open
                });

                if (orders.Count == 0 && clientId.Length > 0)
                {
                    var order = await client.GetOrderAsync(clientId);
                    LastOrders = new List<IOrder>() { order };
                }
                else
                {
                    LastOrders = orders;
                }
            }
            catch
            {
            }
            StatusChanged();
        }

        public Guid lastTradeId = Guid.NewGuid();
        // Submit a market order if quantity is not zero.
        public async Task<Guid> SubmitMarketOrder(int quantity, OrderSide side, string clientId = "", decimal stopDollars = 0)
        {
            //if (quantity == 0)
            if (quantity == 0)
            {
                return Guid.Empty;
            }
            //Console.WriteLine($"Submitting {side} order for {quantity} shares.");
            try
            {
                IOrder order;
                if (clientId.Length > 0)
                    order = await client.PostOrderAsync(side.Market(symbol, quantity).WithClientOrderId(clientId));
                else
                    order = await client.PostOrderAsync(side.Market(symbol, quantity));
                lastTradeId = order.OrderId;

                if (stopDollars > 0)
                {
                    // Submit a trailing stop order to sell / buy quantitu of symbol at market - stoploss
                    // trailing stop of
                    if (side == OrderSide.Buy)
                        order = await client.PostOrderAsync(
                            TrailingStopOrder.Sell(symbol, quantity, TrailOffset.InDollars(stopDollars))); // stop price will be hwm - stopDollars$ (326.44 - 0.05 = 326.39)
                    else
                        order = await client.PostOrderAsync(
                            TrailingStopOrder.Buy(symbol, quantity, TrailOffset.InDollars(stopDollars))); // stop price will be hwm + stopDollars$

                }
            }
            catch (Exception ex)
            {
                //  jsruntime.alert(ex.Message);
            }

            if (clientId.Length > 0)
                await GetOrders(clientId);

            return lastTradeId;
        }

        private async Task ClosePositionAtMarket()
        {
            try
            {
                var positionQuantity = (await client.GetPositionAsync(symbol)).Quantity;
                await client.PostOrderAsync(
                    OrderSide.Sell.Market(symbol, positionQuantity));
            }
            catch (Exception)
            {
                // No position to exit.
            }
        }

    }
}

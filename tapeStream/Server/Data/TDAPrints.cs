using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Shared;

namespace tdaStreamHub.Data
{
    public class TDAPrints
    {
        DataItem[] rawGaugesCombined = new DataItem[] {
            new DataItem
            {
                Date = DateTime.Parse("2019-12-01"),
                Revenue = 6
            } };

        private static Dictionary<DateTime, double> gaugeValues = new Dictionary<DateTime, double>();

        private static Dictionary<int, DataItem[]> dictPies = new Dictionary<int, DataItem[]>();
        static public async Task<double> GetPrintsGaugeScore()
        {
            var value = 0;
            foreach (var seconds in CONSTANTS.printSeconds)
            {
                var slices = dictPies[seconds];

                var reds = slices[0].Revenue + slices[2].Revenue;
                var greens = slices[3].Revenue + slices[4].Revenue;

                value += reds > greens ? -1
                        : greens > reds ? 1
                        : 0;
            }
            await Task.CompletedTask;
            gaugeValues.Add(DateTime.Now, value);
            return value;
        }

        public static async Task<Dictionary<int, DataItem[]>> GetPrintsPies(string symbol)
        {
            dictPies = new Dictionary<int, DataItem[]>();
            foreach (var seconds in CONSTANTS.printSeconds)
            {
                var slices = TDAPrints.GetPieSlices(symbol, seconds);
                dictPies.Add(seconds, slices);
            }
            await Task.CompletedTask;
            return dictPies;
        }

        public List<DataItem[]> getLastPrintsData(int nSeconds)
        {
            if (gaugeValues.Count() == 0) return new List<DataItem[]>();

            var rawGaugesCombined = gaugeValues
                .Where(dict => DateTime.Now.Subtract(dict.Key).Seconds > nSeconds)
                .Select(dict =>
                new DataItem()
                {
                    Date = dict.Key,
                    Revenue = dict.Value
                }
                ).ToArray();

            var staticValue0 = staticValue(0).Where(t => DateTime.Now.Subtract(t.Date).Seconds > nSeconds).ToArray();
            var staticValue7 = staticValue(7).Where(t => DateTime.Now.Subtract(t.Date).Seconds > nSeconds).ToArray();
            var staticValueMinus7 = staticValue(-7).Where(t => DateTime.Now.Subtract(t.Date).Seconds > nSeconds).ToArray();

            var movingAverage5min = movingAverage(300).Where(t => DateTime.Now.Subtract(t.Date).Seconds > nSeconds).ToArray();

            var average10min = staticAverage(600).Where(t => DateTime.Now.Subtract(t.Date).Seconds > nSeconds).ToArray();

            var movingAverage30sec = movingAverage(30).Where(t => DateTime.Now.Subtract(t.Date).Seconds > nSeconds).ToArray(); ;

            /// This holds the value of one point for each line on the chart
            /// THe chart should maintain the prior points locally and add these
            /// to update itslf
            var lastPoints = new List<DataItem[]>()
            { rawGaugesCombined, staticValueMinus7, staticValue0, staticValue7,movingAverage30sec,movingAverage5min,average10min };

            return lastPoints;

        }

        public static async Task Decode(string svcName, string content, string symbol)
        {
            if (TDABook.lstBookEntry.Count == 0) return;

            if (!TDAStreamerData.timeSales.ContainsKey(symbol))
                TDAStreamerData.timeSales.Add(symbol, new List<TimeSales_Content>());

            /// Get current time and sales from streamer content
            var timeAndSales = JsonSerializer.Deserialize<TimeSales_Content>(content);

            var prevTimeAndSales = timeAndSales;
            if (TDAStreamerData.timeSales[symbol].Count > 0)
                prevTimeAndSales = TDAStreamerData.timeSales[symbol].Last();

            /// Combine bid/ask with time & sales and write to database
            /// Need to match time of print and time of quote to get accuarate buys/sells
            //Debugger.Break();

            try
            {
                /// t.Key is Quote date and time, we want last quote before t&s time
                ///                 
                var book = TDABook.lstBookEntry.Where(be => be.time < timeAndSales.time).Last();

                timeAndSales.bid = book.bid;
                timeAndSales.ask = book.ask;
                timeAndSales.askSize = book.askSize;
                timeAndSales.bidSize = book.bidSize;
                timeAndSales.bookTime = book.time;

                timeAndSales.bidIncr = timeAndSales.bid - prevTimeAndSales.bid;
                timeAndSales.askIncr = timeAndSales.ask - prevTimeAndSales.ask;
                timeAndSales.priceIncr = timeAndSales.price - prevTimeAndSales.price;
                var bid = timeAndSales.bid;
                var ask = timeAndSales.ask;
                var price = timeAndSales.price;

                timeAndSales.level = bid == 0 || ask == 0 || price == 0 ? 0 :
                 price < bid ? 1
                 : price == bid ? 2
                 : price > bid && price < ask ?
                 (
                    price - bid < 0.01 ? 2
                    : (ask - price < 0.01 ? 4 : 3)
                 )
                 : price == ask ? 4
                 : price > ask ? 5
                 : 0;


                TDAStreamerData.timeSales[symbol].Add(timeAndSales);

                string json = JsonSerializer.Serialize<TimeSales_Content>(timeAndSales);
                await FilesManager.SendToMessageQueue("TimeSales", timeAndSales.TimeDate, json);
            }
            catch { }
            await Task.CompletedTask;

        }

        private static DataItem[] GetPieSlices(string symbol, int seconds)
        {
            long thisManySecondsAgo = (long)(DateTime.Now.ToUniversalTime().AddSeconds(-seconds) - new DateTime(1970, 1, 1)).TotalMilliseconds;

            var printsData = new DataItem[5];
            var timeSales = TDAStreamerData.timeSales[symbol];

            for (var level = 1; level <= 5; level++)
                try
                {
                    printsData[level - 1].Revenue = timeSales.Where(t => t.level == level && t.time >= thisManySecondsAgo).Sum(t => t.size);
                }
                catch { }

            return printsData;
        }
        private DataItem[] staticAverage(int secs)
        {
            var maxDate = gaugeValues.Keys.Max();
            var avg5min = gaugeValues
                .Where(d => d.Key >= maxDate.AddSeconds(-secs))
                .Average(d => d.Value);

            return gaugeValues.Select(dict =>
            new DataItem()
            {
                Date = dict.Key,
                Revenue = avg5min
            }
            ).ToArray();
        }

        private DataItem[] staticValue(double val)
        {
            return gaugeValues.Select(dict =>
            new DataItem()
            {
                Date = dict.Key,
                Revenue = val
            }
            ).ToArray();
        }

        private DataItem[] movingAverage(int secs)
        {
            return gaugeValues.Select(dict =>
                 new DataItem()
                 {
                     Date = dict.Key,
                     Revenue = gaugeValues
                     .Where(d => d.Key <= dict.Key && d.Key >= dict.Key.AddSeconds(-secs))
                     .Select(d => d.Value).Average()
                 }
            ).ToArray();
        }

        public static void SaveToCsvFile(string svcName, string symbol, TimeSales_Content timeAndSales)
        {
            List<string> timeAndSalesFields = FilesManager.GetCSVHeader(new TimeSales_Content()).Split(',').ToList();

            var lstValues = new List<string>();
            foreach (string name in timeAndSalesFields)
            {
                lstValues.Add($"{timeAndSales[name]}");
            }
            string record = string.Join(',', lstValues) + "\n";

            string fileName = $"{svcName} {symbol} {DateTime.Now.ToString("MMM dd yyyy")}.csv";
            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.AppendAllText(fileName, string.Join(",", timeAndSalesFields) + "\n");

            }
            System.IO.File.AppendAllText(fileName, record);
        }
    }
}

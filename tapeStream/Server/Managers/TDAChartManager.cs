using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Server.Components;
using tapeStream.Server.Data;
using tapeStream.Shared.Data;
//using tapeStream.Server.Data;
using static tapeStream.Shared.Data.TDAChart;
using FilesManager = tapeStream.Server.Data.FilesManager;

//using Chart_Content = tapeStream.Shared.Data.Chart_Content;

namespace tapeStream.Server.Managers
{
    public class TDAChartManager
    {
        /// <summary>
        /// We need to update the final close item each time a new Time & Sales 
        /// price comes in.
        /// OR should we use the mark? from the Book
        /// </summary>        
        /// 
        public static List<double> closes = new List<double>();

        private static int periods;

        internal static DateTime getSvcDateTime()
        {
            return TDAChart.svcDateTime;
        }



        /// <summary>
        /// This happens once per minute.
        /// The charts[] will already have a last entry added by TDAPrintsManager
        /// This Decode just needs to update that to the final close value
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task Decode(string symbol, string content)
        {
            /// Get current time and sales from streamer content
            var chartEntry = JsonSerializer.Deserialize<TDAChart.Chart_Content>(content);

            TDAChart.lastCandle = chartEntry;

            /// Just in case this hits before TDAPrintsManager update
            if (closes.Count() == 0)
                SeedCloses(symbol);

            /// Overwrite last entry with final close for minute
            closes[closes.Count() - 1] = chartEntry.close;

            /// Add entry for TDAPrintsManager to update until next minute entry arrives
            closes.Add(chartEntry.close);


            /// This should be called by the Charts controller when it needs BBs

            GetBollingerBands(closes.ToArray(), periods);

            await Task.CompletedTask;
        }


        public static void SeedCloses(string symbol)
        {
            var chartEntry = new TDAChart.Chart_Content();
            closes.Clear();
            /// Read the last 25 chart entries from CHART_EQUITY Inputs
            var files = FilesManager.GetChartEntries(symbol, 25);
            foreach (var file in files)
            {
                var svcJsonObject = JObject.Parse(file);
                var svcName = svcJsonObject["service"].ToString();
                var contents = svcJsonObject["content"];
                var timeStamp = Convert.ToInt64(svcJsonObject["timestamp"]);
                foreach (var contentObj in contents)
                {
                    var content = contentObj.ToString();
                    //var symbolId = contentObj["key"].ToString();

                    chartEntry = JsonSerializer.Deserialize<TDAChart.Chart_Content>(content);
                    if (chartEntry.close > 0)
                        closes.Add(chartEntry.close);
                }
            }
            if (TDAChart.lastCandle == null)
                TDAChart.lastCandle = chartEntry;

        }


        public static async Task<Bollinger> GetBollingerBands()
        {

            var symbol = Data.TDAStreamerData.timeSales.Keys.Last();
            SeedCloses(symbol);
            closes.Add(Data.TDAStreamerData.timeSales[symbol].Last().price);
            System.Diagnostics.Debug.Print("Last Close=" + closes[closes.Count() - 1].ToString());

#if tracing
            DateTime.Now.Dump();
            closes.Dump();
#endif
            //var x = new TDAStreamer();
            //x.ConsoleLog("Hello World");
            //x.Dispose();

            GetBollingerBands(closes.ToArray(), periods);
            await Task.CompletedTask;
            return bollingerBands;
        }

        public static void GetBollingerBands(double[] closes, int periods)
        {

            periods = Math.Min(20, closes.Count());
            if (periods <= 1) return;

            double average = 0, stdev = 0, total_average = 0, total_squares = 0;

            for (int i = 0; i < closes.TakeLast(periods).Count(); i++)
            {
                total_average += closes[i];
                total_squares += Math.Pow(closes[i], 2);

                if (i >= periods - 1)
                {
                    average = total_average / periods;
                    stdev = Math.Sqrt((total_squares - Math.Pow(total_average, 2) / periods) / periods);

                    //Console.WriteLine("TDA Server closes count=" + closes.Length);
                    total_average -= closes[i - periods + 1];
                    total_squares -= Math.Pow(closes[i - periods + 1], 2);
                }
            }
            bollingerBands.mid = average;
            bollingerBands.high = average + 2 * stdev;
            bollingerBands.low = average - 2 * stdev;
            bollingerBands.midhigh = average + stdev;
            bollingerBands.midlow = average - stdev;
            bollingerBands.Dump();
        }

        internal static TDAChart.Chart_Content getLastCandle()
        {
            var chartEntry = TDAChart.lastCandle;
            //var files = FilesManager.GetChartEntries(1);
            //foreach (var file in files)
            //{
            //    var svcJsonObject = JObject.Parse(file);
            //    var svcName = svcJsonObject["service"].ToString();
            //    var contents = svcJsonObject["content"];
            //    var timeStamp = Convert.ToInt64(svcJsonObject["timestamp"]);
            //    foreach (var contentObj in contents)
            //    {
            //        var content = contentObj.ToString();
            //        var symbol = contentObj["key"].ToString();

            //        chartEntry = JsonSerializer.Deserialize<TDAChart.Chart_Content>(content);
            //        break;
            //    }
            //}
            return chartEntry;
        }
    }
}

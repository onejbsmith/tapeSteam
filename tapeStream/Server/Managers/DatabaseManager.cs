using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Server.Data;
using tapeStream.Server.Models;


namespace tapeStream.Server.Managers
{
    public class DatabaseManager
    {
        public static Dictionary<string, int> dictRunIds = new Dictionary<string, int>();
        public static Dictionary<string, int> dictStreamIds = new Dictionary<string, int>();
        private static tapeStreamDb_devContext db = new tapeStreamDb_devContext();

        private static int Runs_GetRunId(string symbol)
        {
            return dictRunIds[symbol];
        }

        private static int Streamed_GetStreamId(string symbol)
        {
            return dictStreamIds[symbol];
        }

        public static void Runs_Add(string symbol, DateTime runDate)
        {
            var run = new Runs()
            {
                RunDate = runDate,
                Symbol = symbol
            };

            db.Runs.Add(run);
            db.SaveChanges();
            var runId = run.RunId;
            if (dictRunIds.ContainsKey(symbol))
                dictRunIds[symbol] = runId;
            else
                dictRunIds.Add(symbol, runId);
        }

        public static void Streamed_Add(string symbol, DateTime dateTime)
        {
            var runId = Runs_GetRunId(symbol);
            var streamed = new Streamed()
            {
                RunId = runId,
                StreamTime = dateTime
            };

            db.Streamed.Add(streamed);
            db.SaveChanges();
            var streamId = streamed.StreamId;
            if (dictStreamIds.ContainsKey(symbol))
                dictStreamIds[symbol] = streamId;
            else
                dictStreamIds.Add(symbol, streamId);
        }

        public static void Sells_Add(TimeSales_Content timeAndSales, int size)
        {
            var symbol = timeAndSales.key;
            var sell = new Sells()
            {
                StreamId = Streamed_GetStreamId(symbol),
                Price = (decimal)timeAndSales.price,
                Size = size,
                PriceLevel = (byte)timeAndSales.level
            };
            db.Sells.Add(sell);
            db.SaveChanges();
        }

        public static void Buys_Add(TimeSales_Content timeAndSales, int size)
        {
            var symbol = timeAndSales.key;
            var buy = new Buys()
            {
                StreamId = Streamed_GetStreamId(symbol),
                Price = (decimal)timeAndSales.price,
                Size = size,
                PriceLevel = (byte)timeAndSales.level
            };
            db.Buys.Add(buy);
            db.SaveChanges();
        }


        public static void Bids_Add(string symbol, tapeStream.Shared.Data.BookDataItem bid)
        {
            var bidded = new Bids()
            {
                StreamId = Streamed_GetStreamId(symbol),
                Price = (decimal)bid.Price,
                Size = (int)bid.Size
            };
            db.Bids.Add(bidded);
            db.SaveChanges();
        }

        public static void Asks_Add(string symbol, tapeStream.Shared.Data.BookDataItem ask)
        {
            var asked = new Asks()
            {
                StreamId = Streamed_GetStreamId(symbol),
                Price = (decimal)ask.Price,
                Size = (int)ask.Size
            };
            db.Asks.Add(asked);
            db.SaveChanges();
        }

        public static void Marks_Add(string symbol, tapeStream.Shared.Data.BookDataItem mark)
        {
            var marked = new Marks()
            {
                StreamId = Streamed_GetStreamId(symbol),
                Price = (decimal)mark.Price,
                Size = (int)mark.Size
            };
            db.Marks.Add(marked);
            db.SaveChanges();
        }
    }
}


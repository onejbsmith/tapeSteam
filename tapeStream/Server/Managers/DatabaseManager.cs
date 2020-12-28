using System;
using System.Collections.Generic;
using System.Linq;
using tapeStream.Server.Data;
using tapeStream.Server.Models;


namespace tapeStream.Server.Managers
{
    public class DatabaseManager
    {
        private static tapeStreamDb_devContext db = new tapeStreamDb_devContext();

        public static void Sells_Add(TimeSales_Content timeAndSales, int size)
        {
            var symbol = timeAndSales.key;
            var sell = new Sells()
            {
                Symbol = symbol,
                RunDateTime = timeAndSales.TimeDate,
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
                Symbol = symbol,
                RunDateTime = timeAndSales.TimeDate,
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
                Symbol = symbol,
                RunDateTime = bid.dateTime,
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
                Symbol = symbol,
                RunDateTime = ask.dateTime,
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
                Symbol = symbol,
                RunDateTime = mark.dateTime,
                Price = (decimal)mark.Price,
                Size = (int)mark.Size
            };
            db.Marks.Add(marked);
            db.SaveChanges();
        }

        public static void Bids_Add(string symbol, List<tapeStream.Shared.Data.BookDataItem> bids)
        {
            var biddeds = bids.Select(bid => new Bids()
            {
                Symbol = symbol,
                RunDateTime = bid.dateTime,
                Price = (decimal)bid.Price,
                Size = (int)bid.Size
            });
            db.Bids.AddRange(biddeds);
            db.SaveChanges();
        }
        public static void Asks_Add(string symbol, List<tapeStream.Shared.Data.BookDataItem> asks)
        {
            var askeds = asks.Select(ask => new Asks()
            {
                Symbol = symbol,
                RunDateTime = ask.dateTime,
                Price = (decimal)ask.Price,
                Size = (int)ask.Size
            });
            db.Asks.AddRange(askeds);
            db.SaveChanges();
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using tapeStream.Client.Data;
using tapeStream.Shared;

namespace tapeStream.Data
{
    public class TDAPrints
    {
        public static DataItem[] newData = new DataItem[] {
            new DataItem { Quarter = "1", Revenue = 1 },
            new DataItem { Quarter = "2", Revenue = 2 },
            new DataItem { Quarter = "3", Revenue = 3 },
            new DataItem { Quarter = "4", Revenue = 4 },
            new DataItem { Quarter = "2", Revenue = 2 }
        };

        public static Dictionary<int, DataItem[]> dictPies
            = new Dictionary<int, DataItem[]>() { { 5, newData } };

        public static DataItem[] GetPieSlices(string symbol, int seconds)
        {
            long thisManySecondsAgo = (long)(DateTime.Now.ToUniversalTime().AddSeconds(-seconds) - new DateTime(1970, 1, 1)).TotalMilliseconds;

            var printsData = new DataItem[5];
            var timeSales = TDAStreamerData.timeSales[symbol];

            try
            {
                printsData[0].Revenue = timeSales.Where(t => t.level == 1 && t.time >= thisManySecondsAgo).Sum(t => t.size);

            }
            catch
            {

            }
            try
            {
                printsData[1].Revenue = timeSales.Where(t => t.level == 2 && t.time >= thisManySecondsAgo).Sum(t => t.size);

            }
            catch
            {
            }


            try
            {
                printsData[2].Revenue = timeSales.Where(t => t.level == 3 && t.time >= thisManySecondsAgo).Sum(t => t.size);

            }
            catch
            {
            }

            try
            {

            }
            catch
            {
            }
            printsData[3].Revenue = timeSales.Where(t => t.level == 4 && t.time >= thisManySecondsAgo).Sum(t => t.size);


            try
            {
                printsData[4].Revenue = timeSales.Where(t => t.level == 5 && t.time >= thisManySecondsAgo).Sum(t => t.size);

            }
            catch
            {
            }

            return printsData;
        }

        static public double GetPrintsGaugeScore(string symbol)
        {
            var value = 0;
            dictPies = new Dictionary<int, DataItem[]>();
            foreach (var seconds in CONSTANTS.printSeconds)
            {
                var slices = TDAPrints.GetPieSlices(symbol, seconds);
                dictPies.Add(seconds, slices);

                var reds = slices[0].Revenue + slices[2].Revenue;
                var greens = slices[3].Revenue + slices[4].Revenue;

                value += reds > greens ? -1
                        : greens > reds ? 1
                        : 0;
            }
            return value;
        }
    }

}

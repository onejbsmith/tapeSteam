using System.Collections.Generic;

namespace tapeStream.Shared.Data
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


        public static Dictionary<string, DataItem[]> dictPies = new Dictionary<string, DataItem[]>();
        //static public async Task<double> GetPrintsGaugeScore()
        //{
        //    var value = 0;
        //    foreach (var seconds in CONSTANTS.printSeconds)
        //    {
        //        var slices = dictPies[seconds];

        //        var reds = slices[0].Revenue + slices[2].Revenue;
        //        var greens = slices[3].Revenue + slices[4].Revenue;

        //        value += reds > greens ? -1
        //                : greens > reds ? 1
        //                : 0;
        //    }
        //    await Task.CompletedTask;
        //    return value;
        //}

        //public static async Task<Dictionary<int, DataItem[]>> GetPrintsPies(string symbol)
        //{
        //    dictPies = new Dictionary<int, DataItem[]>();
        //    foreach (var seconds in CONSTANTS.printSeconds)
        //    {
        //        var slices = TDAPrints.GetPieSlices(symbol, seconds);
        //        dictPies.Add(seconds, slices);

        //    }
        //    await Task.CompletedTask;
        //    return dictPies;
        //}

        //public static DataItem[] GetPieSlices(string symbol, int seconds)
        //{
        //    long thisManySecondsAgo = (long)(DateTime.Now.ToUniversalTime().AddSeconds(-seconds) - new DateTime(1970, 1, 1)).TotalMilliseconds;

        //    var printsData = new DataItem[5];
        //    var timeSales = TDAStreamerData.timeSales[symbol];

        //    for (var level = 1; level <= 5; level++)
        //        try
        //        {
        //            printsData[level - 1].Revenue = timeSales.Where(t => t.level == level && t.time >= thisManySecondsAgo).Sum(t => t.size);
        //        }

        //    return printsData;
        //}

        //private static void SaveToCsvFile(string svcName, string symbol, TimeSales_Content timeAndSales)
        //{
        //    List<string> timeAndSalesFields = FilesManager.GetCSVHeader(new TimeSales_Content()).Split(',').ToList();

        //    var lstValues = new List<string>();
        //    foreach (string name in timeAndSalesFields)
        //    {
        //        lstValues.Add($"{timeAndSales[name]}");
        //    }
        //    string record = string.Join(',', lstValues) + "\n";

        //    string fileName = $"{svcName} {symbol} {DateTime.Now.ToString("MMM dd yyyy")}.csv";
        //    if (!System.IO.File.Exists(fileName))
        //    {
        //        System.IO.File.AppendAllText(fileName, string.Join(",", timeAndSalesFields) + "\n");

        //    }
        //    System.IO.File.AppendAllText(fileName, record);
        //}
    }
}

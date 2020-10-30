using System;
using System.Diagnostics;
using System.Threading.Tasks;
using tapeStream.Client.Data;

namespace tapeStream.Client.Components
{
    public partial class PrintArcGaugeChart
    {
        public int[] printSeconds { get; set; } = new int[] { 5, 10, 30, 60, 120, 240, 600 };

        Stopwatch stopWatch = new Stopwatch();
        string elapsedTime;

        protected override async Task OnInitializedAsync()
        {
            TDAStreamerData.OnTimeSalesStatusChanged += getPrintsData;
            StateHasChanged();
            await Task.CompletedTask;
            stopWatch.Start();
            GetElapsedTime();
        }

        void getPrintsData()
        {
            //if (moduloPrints ==0 || TDAStreamerData.timeSales[symbol].Count() % moduloPrints != 0) return;

            //var sales = TDAStreamerData.timeSales[symbol];
            //value = 0;
            //foreach (var seconds in printSeconds)
            //{
            //    long oneMinuteAgo = (long)(DateTime.Now.ToUniversalTime().AddSeconds(-seconds) - new DateTime(1970, 1, 1)).TotalMilliseconds;

            //    var reds = weight * sales.Where(t => t.level == 1 && t.time >= oneMinuteAgo).Sum(t => t.size)
            //               + sales.Where(t => t.level == 2 && t.time >= oneMinuteAgo).Sum(t => t.size);
            //    //printsData[2].Revenue = sales.Where(t => t.level == 3 && t.time >= oneMinuteAgo).Sum(t => t.size);
            //    var greens = sales.Where(t => t.level == 4 && t.time >= oneMinuteAgo).Sum(t => t.size)
            //    + weight * sales.Where(t => t.level == 5 && t.time >= oneMinuteAgo).Sum(t => t.size);

            //    value += reds > greens ? -1
            //            : greens > reds ? 1
            //            : 0;
            //}

            //TDAStreamerData.gaugeValues.Add(DateTime.Now, value);
            //TDAStreamerData.gaugeValues.RemoveAll((key, val) => key < DateTime.Now.AddSeconds(-printSeconds.Max()));
            GetElapsedTime();

            StateHasChanged();
        }

        private void GetElapsedTime()
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        }
    }
}

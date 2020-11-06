using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using tapeStream.Client.Data;

namespace tapeStream.Client.Components
{
    public partial class PrintArcGaugeChart
    {
        public int[] printSeconds { get; set; } = new int[] { 5, 10, 30, 60, 120, 240, 600 };

        private double myVar;

        [Parameter]
        public double value
        {
            get { return myVar; }
            set
            {
                myVar = value;
                stopWatch.Stop();
                 GetElapsedTime();
           }
        }

        Stopwatch stopWatch = new Stopwatch();
        Stopwatch stopWatchTotal = new Stopwatch();
        string elapsedTime;
        string totalTime;

        protected override async Task OnInitializedAsync()
        {
            //TDAStreamerData.OnTimeSalesStatusChanged += getPrintsData;
            stopWatch.Start();
            stopWatchTotal.Start();
            //GetElapsedTime();
            await Task.CompletedTask;
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
            TimeSpan tsTotal = stopWatchTotal.Elapsed;

            // Format and display the TimeSpan values.
            elapsedTime = String.Format("{0:00}s {1:00}f",ts.Seconds, ts.Milliseconds / 10);
            stopWatch.Restart();

            totalTime = String.Format(" {0:00}h {1:00}m",tsTotal.Hours, tsTotal.Minutes, tsTotal.Seconds);

            StateHasChanged();
        }
    }
}

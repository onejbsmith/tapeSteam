#define tracing

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using tapeStream.Client.Data;
using tapeStream.Shared.Data;
using tapeStream.Shared;
using tapeStream.Shared.Services;
using MatBlazor;
using Threader = System.Threading;
using tapeStream.Client.Components;
using Microsoft.JSInterop;

namespace tapeStream.Client.Pages
{
    public partial class Index
    {
        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] ChartService chartService { get; set; }

        Timer timerBookColumnsCharts = new Timer(500);


        public Dictionary<string, BookDataItem[]> bookColData
        {
            get { return _bookColData; }
            set
            {
                _bookColData = value;
            }
        }
        Dictionary<string, BookDataItem[]> _bookColData;

        protected override async Task OnInitializedAsync()
        {

            /// Init parameters so don't get "null" error
            await InitializeData();

            InitializeTimers();
        }

        private void InitializeTimers()
        {
            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender, e);
            timerBookColumnsCharts.Start();
        }
        private async Task InitializeData()
        {

            bookColData = CONSTANTS.newBookColumnsData;

            //await AppendLineChartData();
        }
        private async Task TimerBookColumnsCharts_Elapsed(object sender, ElapsedEventArgs e)
        {
            await GetBookColumnsData(ChartConfigure.seconds);
        }

        private async Task GetBookColumnsData(int seconds)
        {
            timerBookColumnsCharts.Stop();
            await Task.Yield();
            bookColData = await bookColumnsService.getBookColumnsData(seconds);

            var avgSizes = await bookColumnsService.getAverages(SurfaceChartConfigurator.longSeconds, jsruntime);
            var avgStSizes = await bookColumnsService.getAverages(SurfaceChartConfigurator.shortSeconds, jsruntime);
            var avgLtSizes = await bookColumnsService.getAverages(0, jsruntime);

            var avgRatios = await bookColumnsService.getRatios(SurfaceChartConfigurator.longSeconds, jsruntime);
            var avgStRatios = await bookColumnsService.getRatios(SurfaceChartConfigurator.shortSeconds, jsruntime);
            var avgLtRatios = await bookColumnsService.getRatios(0, jsruntime);


            TDAChart.bollingerBands = await chartService.getBollingerBands();
            TDAChart.lastCandle = await chartService.GetTDAChartLastCandle(0);
            TDAChart.svcDateTimeRaw = await chartService.GetSvcDate();
            TDAChart.svcDateTimeRaw = TDAChart.svcDateTimeRaw.Replace("\"", "");
            TDAChart.svcDateTime = Convert.ToDateTime(TDAChart.svcDateTimeRaw);

            //foreach (var name in avgSizes.averageSize.Keys)
            //    if (avgSizes.averageSize[name] > 0)
            //        TDAChart.avgSizes.averageSize[name] = avgSizes.averageSize[name];

            //foreach (var name in avgStSizes.averageSize.Keys)
            //    if (avgStSizes.averageSize[name] > 0)
            //        TDAChart.avgStSizes.averageSize[name] = avgStSizes.averageSize[name];

            TDAChart.avgSizes = avgSizes;
            TDAChart.avgStSizes = avgStSizes;
            TDAChart.avgLtSizes = avgLtSizes;

            TDAChart.avgRatios = avgRatios;
            TDAChart.avgStRatios = avgStRatios;
            TDAChart.avgLtRatios = avgLtRatios;

            var avgBuys = 0d;
            if (avgSizes.averageSize.ContainsKey("buys"))
                avgBuys = avgSizes.averageSize["buys"];

            var avgSells = 0d;
            if (avgSizes.averageSize.ContainsKey("sells"))
                avgSells = avgSizes.averageSize["sells"];

            TDAChart.countBuysRatioUp += avgBuys > avgSells ? 1 : 0;
            TDAChart.countSellRatioUp += avgSells > avgBuys ? 1 : 0;

            //TDAChart.lastCandles = await chartService.getLastCandles(2);
            await Task.Delay(100);

            //await jsruntime.InvokeVoidAsync("Dump", Dumps(), "TDAChart.lastCandle");
            //await jsruntime.InvokeAsync<string>("Confirm", "Task GetBookColumnsData");

#if tracing
            jsruntime.GroupTable(TDAChart.lastCandle, nameof(TDAChart.lastCandle));
#endif

            //jsruntime.Confirm("Task GetBookColumnsData");

            //Debug.WriteLine("2. BookColumnsCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
            StateHasChanged();
            timerBookColumnsCharts.Start();
            await Task.CompletedTask;
        }
    }
}
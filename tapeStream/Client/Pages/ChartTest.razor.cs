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
    public partial class ChartTest
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

            TDAChart.bollingerBands = await chartService.getBollingerBands();
            TDAChart.lastCandle = await chartService.GetTDAChartLastCandle(0);
            TDAChart.svcDateTimeRaw = await chartService.GetSvcDate();
            TDAChart.svcDateTimeRaw = TDAChart.svcDateTimeRaw.Replace("\"", "");
            TDAChart.svcDateTime = Convert.ToDateTime(TDAChart.svcDateTimeRaw);
            //TDAChart.lastCandles = await chartService.getLastCandles(2);
            await Task.Delay(100);

            //await jsruntime.InvokeVoidAsync("Dump", Dumps(), "TDAChart.lastCandle");
            //await jsruntime.InvokeAsync<string>("Confirm", "Task GetBookColumnsData");

            jsruntime.GroupTable(TDAChart.lastCandle, nameof(TDAChart.lastCandle));

            //jsruntime.Confirm("Task GetBookColumnsData");

            //Debug.WriteLine("2. BookColumnsCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
            StateHasChanged();
            timerBookColumnsCharts.Start();
            await Task.CompletedTask;
        }  }
}

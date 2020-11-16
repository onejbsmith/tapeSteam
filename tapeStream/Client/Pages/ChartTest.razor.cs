﻿using Microsoft.AspNetCore.Components;
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
            await Task.Delay(100);
            //Debug.WriteLine("2. BookColumnsCharts = " + Threader.Thread.CurrentThread.ManagedThreadId.ToString());
            StateHasChanged();
            timerBookColumnsCharts.Start();
            await Task.CompletedTask;
        }  }
}

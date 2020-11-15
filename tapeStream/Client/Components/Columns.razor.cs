using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using tapeStream.Shared;
using tapeStream.Shared.Data;
using tapeStream.Shared.Services;

namespace tapeStream.Client.Components
{
    public partial class Columns
    {
        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] ChartService chartService { get; set; }

        string symbol = "QQQ";

        Timer timerBookColumnsCharts = new Timer(500);


        /// <summary>
        /// BookColumnsChart
        /// </summary>
        public Dictionary<string, BookDataItem[]> bookColData
        {
            get { return _bookColData; }
            set
            {
                _bookColData = value;
            }
        }
        Dictionary<string, BookDataItem[]> _bookColData;


        static string clockFormat = "h:mm:ss MMM d, yyyy";


        KeyValuePair<DateTime, double>[] latestGaugeValues;
        int moduloPrints = 1;
        int moduloBook = 1;


        bool logHub;

        int lstTimeSales = 0;

        private string topicInput;
        private string messageInput;

        string clock;
        string logTopics;

        StringBuilder logTopicsb = new StringBuilder();  /// Called from Javascript
        Dictionary<string, int> dictTopicCounts = new Dictionary<string, int>();
        //private HubConnection hubConnection;
        private List<string> messages = new List<string>();

        protected override async Task OnInitializedAsync()
        {

            /// Init parameters so don't get "null" error
            await InitializeData();

            /// For the Hub Monitor


            InitializeTimers();

            //CheckThreadpool();
        }


        private void InitializeTimers()
        {
            timerBookColumnsCharts.Elapsed += async (sender, e) => await TimerBookColumnsCharts_Elapsed(sender, e);
            timerBookColumnsCharts.Start();

        }


        private async Task InitializeData()
        {
            /// For the Book Columns
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
        }


    }
}

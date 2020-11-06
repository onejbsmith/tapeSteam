using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Shared.Data;
using tapeStream.Shared.Services;

namespace tapeStream.Client.Components
{
    public partial class BookColumnsChart
    {
        [Inject] BookColumnsService bookColumnsService { get; set; }

        public string timeOfDay;

        RadzenChart myChart = new RadzenChart() { };

        float xAxisMin = 999999, xAxisMax = 0;
        float yAxisMax = 0;

        BookDataItem[] bidData = new BookDataItem[0];
        BookDataItem[] askData = new BookDataItem[0];
        BookDataItem[] salesAtBidData = new BookDataItem[0];
        BookDataItem[] salesAtAskData = new BookDataItem[0];

        static string title = "QQQ Real-time Supply/Demand";

        public static int seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                if (_seconds == 0)
                    title = "QQQ Real-time Supply/Demand";
                else
                    title = $"QQQ Real-time Supply/Demand Aggregated for {_seconds} seconds";
            }
        }
        private static int _seconds;

        Dictionary<string, BookDataItem[]> _bookData;
        [Parameter]
        public Dictionary<string, BookDataItem[]> bookData
        {
            get { return _bookData; }
            set
            {
                _bookData = value;
                bidData = _bookData["bids"];
                askData = _bookData["asks"];
                salesAtBidData = _bookData["salesAtBid"];
                salesAtAskData = _bookData["salesAtAsk"];
                timeOfDay = DateTime.Now.ToString("dddd, MMM d, yyyy   h:mm tt");

                //Console.WriteLine($"==========================>>>>>>>>>>>>>>>>>>>>>>>>>bookData={bidData.Length}-{askData.Length}-{salesAtBidData.Length}-{salesAtAskData.Length}");
                ScaleChartAxes(1, 5000);

            }
        }

        private void ScaleChartAxes(float xWidth, float yCeilingScale)
        {
            float xMin = 999999, xMax = 0, yMax = 0;
            foreach (var element in bookData)
            {
                try
                {
                    var elMaxPrice = element.Value.Max(el => (float)el.Price);
                    var elMinPrice = element.Value.Max(el => (float)el.Price);
                    xMax = Math.Max(elMaxPrice, xMax);
                    xMin = Math.Min(elMinPrice, xMin);

                    var elMaxSize = element.Value.Max(el => (float)el.Size);
                    yMax = Math.Max(elMaxSize, yMax);
                }
                catch { }

            }
            xAxisMin = (float)Math.Floor(xMin);
            xAxisMax = (float)Math.Ceiling(xMax);

            /// When xWidth is fractional, e.g. 0.50,
            /// we want the .50 below min and .50 above max
            /// or 1.00 below min and .50 above max
            /// or .50 below min and 1.00 above max

            if (xAxisMax - xAxisMin > xWidth)
            {
                float avg = (xAxisMax + xAxisMin) / 2;
                xAxisMin = avg - xWidth / 2;
                xAxisMax = avg + xWidth / 2;
            }


            /// Ceiing is next 1000 above the max
            yAxisMax = (float)Math.Ceiling(yMax / yCeilingScale) * yCeilingScale;

            ///// Add two points to scale the x Axis
            bidData = bidData.Prepend(new BookDataItem() { dateTime = DateTime.Now, Price = (decimal)xAxisMin, Size = .1 }).ToArray();
            askData = askData.Append(new BookDataItem() { dateTime = DateTime.Now, Price = (decimal)xAxisMax, Size = 5000 }).ToArray();
            myChart.Reload();
            //StateHasChanged();

        }

        private void PadData(ref KeyValuePair<string, BookDataItem[]> element)
        {
            /// Add points to the dollar above our max price 
            /// and the dollar below our min price,
            /// unless min below a dollar price and max above it
            /// when we just keep the range at a dollar 
            /// by moving the range to accomodate the short side
            /// taking away the same amount from the long side (slide)
            /// So we cound just start with a 100 point dollar range
            /// and 
            /// until both on same side
        }


        protected override async Task OnInitializedAsync()
        {
            //TDAStreamerData.OnStatusesChanged += getBookData;
            StateHasChanged();
            await Task.CompletedTask;
        }

        //public void getBookData()
        //{

        //    if (moduloBook == 0 || TDAStreamerData.lstAsks.Count % moduloBook != 0) return;

        //    TDAStreamerData.getBookColumnsData(ref askData, ref bidData, seconds, isPrintsSize, symbol);
        //    //myChart.Reload();
        //    timeOfDay = DateTime.Now;
        //    StateHasChanged();
        //    //       await Task.CompletedTask;
        //}

    }
}

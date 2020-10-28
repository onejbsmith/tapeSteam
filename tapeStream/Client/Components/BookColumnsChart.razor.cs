using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Client.Data;

namespace tapeStream.Client.Components
{
    public partial class BookColumnsChart
    {

        [Parameter]
        public Dictionary<string, BookDataItem[]> bookData
        {
            get { return _bookData; }
            set
            {
                _bookData = value;
                bidData = _bookData["bids"];
                askData = _bookData["asks"];
                timeOfDay = DateTime.Now;
                StateHasChanged();
            }
        }
        Dictionary<string, BookDataItem[]> _bookData;

        BookDataItem[] bidData = new BookDataItem[0];

        BookDataItem[] askData = new BookDataItem[0];


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

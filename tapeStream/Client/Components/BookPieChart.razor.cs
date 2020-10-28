using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Client.Data;

namespace tapeStream.Client.Components
{
    public partial class BookPieChart
    {
        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }


        private BookDataItem[] _bookData;
        
        [Parameter]
        public BookDataItem[] bookData
        {
            get
            {
                return _bookData;
            }
            set
            {
                _bookData = value;
                getBookData();
            }
        }
        //   {
        //new BookDataItem { Price = 350.11m, Size = 1000 },
        //new BookDataItem { Price = 351m, Size = 501 } };


        protected override async Task OnInitializedAsync()
        {
            TDAStreamerData.OnStatusesChanged += getBookData;
            StateHasChanged();
            await Task.CompletedTask;
        }

        public void getBookData()
        {
            //if (moduloBook == 0 || TDAStreamerData.lstAllBids.Count % moduloBook != 0) return;

            //TDAStreamerData.getBookPieData(ref bookData, seconds, isPrintsSize, symbol);


            if (bookData == null || bookData.Length == 0 || bookData[0] == null || bookData[0].Size == 0) return;

            if (bookData[0] != null && bookData[0].Size != 0 && bookData[1] != null)
            {

                bids = (100 * (bookData[1].Size / bookData[0].Size - 1)).ToString("n0");

                int nBids = Convert.ToInt16(bids);

                if (nBids < -20)
                    badgeClass = "alert-danger article";
                else if (nBids > 20)
                    badgeClass = "alert-success article";
                else
                    badgeClass = "alert-warning article";
            }

            StateHasChanged();
        }

    }
}

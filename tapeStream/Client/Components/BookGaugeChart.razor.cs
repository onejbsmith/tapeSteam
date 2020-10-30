using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Client.Data;

namespace tapeStream.Client.Components
{
    public partial class BookGaugeChart
    {



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
            //TDAStreamerData.OnStatusesChanged += getBookData;
            StateHasChanged();
            await Task.CompletedTask;
        }

        public void getBookData()
        {

            var isBookDataNotThere = bookData == null || bookData.Length < 2 || bookData[0] == null;
            if (isBookDataNotThere) return;
            
            try
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
            catch { }

            StateHasChanged();

        }

    }
}

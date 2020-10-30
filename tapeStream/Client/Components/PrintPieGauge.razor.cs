using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Client.Data;
using tapeStream.Shared;

namespace tapeStream.Client.Components
{
    public partial class PrintPieGauge
    {

        [Parameter]
        public DataItem[] printsData
        {
            get { return _printsData; }
            set
            {
                _printsData = value;
                getPrintsData();
            }
        }
        private DataItem[] _printsData;
        string total = "0";
        string bids;

        string badgeClass = "btn btn-circle";
        public int[] printSeconds { get; set; } = new int[] { 5, 10, 30, 60, 120, 240, 600 };


        //public DataItem[] printsData = new DataItem[] {
        // new DataItem { Quarter = "Price < Bid", Revenue = 100 }, // darkred
        // new DataItem { Quarter = "Price = Bid", Revenue = 200 }, // red
        // new DataItem { Quarter = "Price Between Bid & Ask", Revenue = 300 }, // cyan
        // new DataItem { Quarter = "Price = Ask", Revenue = 400 }, // green
        // new DataItem { Quarter = "Price > Ask", Revenue = 501 } }; // lime

        protected override async Task OnInitializedAsync()
        {
            StateHasChanged();
            await Task.CompletedTask;
        }

        public void getPrintsData()
        {
            if (moduloPrints == 0 || TDAStreamerData.timeSales[symbol].Count() % moduloPrints != 0) return;

            if (printsData == null) return;

            total = printsData.Sum(t => t.Revenue).ToString("n0");

            var reds = printsData[0].Revenue + printsData[1].Revenue;
            var greens = printsData[3].Revenue + printsData[4].Revenue;
            double nBids = 1;
            if (reds == 0 && greens == 0) nBids = 0;
            if (reds == 0) reds = (1 / 11) * greens;
            nBids = 100 * ((greens / reds) - 1);
            bids = nBids.ToString("n0");

            if (nBids < -10)
                badgeClass = "btn-danger btn btn-circle";
            else if (nBids > 10)
                badgeClass = "btn-success btn btn-circle";
            else
                badgeClass = " btn btn-circle";

            StateHasChanged();
        }

    }
}

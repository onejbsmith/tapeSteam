using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Shared.Data;
using MatBlazor;

namespace tapeStream.Client.Components
{
    public partial class RatiosChart
    {
        [Parameter]
        public bool showPrice { get; set; } = true;

        private List<RatioFrame> _ratioFrames;
        [Parameter]
        public List<RatioFrame> ratioFrames
        {
            get { return _ratioFrames; }
            set
            {
                _ratioFrames = value;

                var now = TDAChart.svcDateTime;
                var secs = SurfaceChartConfigurator.longSeconds;
                try
                {
                    if (_ratioFrames.Count > 0)
                    {
                        sumBuys = _ratioFrames.Where(t => now.Subtract(t.dateTime).Seconds < secs).Sum(t => t.buysRatio);
                        sumSells = _ratioFrames.Sum(t => t.sellsRatio);

                        buysTitle = $"Buys - {sumBuys.ToString("n0")}";
                        sellsTitle = $"Sells - {sumSells.ToString("n0")}";

                        currentPrice = ratioFrames.LastOrDefault().markPrice.ToString("c2");
                        StateHasChanged();
                    }
                }
                catch
                {

                }
#if tracing
                Console.WriteLine($"1. {id} UpdateLinesChart");
#endif
                //UpdateLinesChart(_ratioFrames);
            }
        }

        private string _buysField = "buysTradeSizes";
        [Parameter]
        public string buysField
        {
            get { return _buysField; }
            set
            {
                _buysField = value;
                sellsField = _buysField.Replace("buys", "sells").Replace("asks", "bids");

            }
        }

        string sellsField = "sellsTradeSizes";


        string currentPrice;

        bool smooth = true;

        static double sumBuys = 0;
        static double sumSells = 0;

        string buysTitle = $"Buys - {sumBuys}";
        string sellsTitle = $"Sells - {sumSells}";

        bool dialogIsOpen = false;

        MatChip[] selectedBuysChips = null;
        MatChip selectedBuyChip = null;

        MatChip[] selectedSellsChips = null;
        MatChip selectedSellChip = null;



        void OpenDialog()
        {
            dialogIsOpen = true;
        }

        void OkClick()
        {
            dialogIsOpen = false;
        }

        string FormatAsUSD(object value)
        {
            if ((double)value < 1)
                return ((double)value).ToString("n2", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            else
                return ((double)value).ToString("n0", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

        }

        void ProcessChips()
        {
            foreach (var chip in selectedBuysChips ?? new MatChip[0])
            {
                if ((chip.IsSelected))
                {
                    buysField = (string)chip.Value;
                    sellsField = buysField.Replace("buys", "sells").Replace("asks", "bids"); ;
                }
            }

        }

    }
}

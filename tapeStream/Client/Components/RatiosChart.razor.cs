﻿using Microsoft.AspNetCore.Components;
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

        Radzen.Blazor.RadzenChart radzenChart;
        [Parameter]
        public List<RatioFrame> allRatioFrames { get; set; }

        [Parameter]
        public bool showPrice { get; set; } = true;

        private List<RatioFrame> passedRatioFrames { get; set; } = new List<RatioFrame>();
        private List<RatioFrame> _ratioFrames;
        [Parameter]
        public List<RatioFrame> ratioFrames
        {
            get { return _ratioFrames; }
            set
            {
                _ratioFrames = value;

                var now = TDAChart.svcDateTime;
                var secs = TDABook.ratiosDepth;
                try
                {
                    if (_ratioFrames.Count > 0)
                    {

                        //_ratioFrames = _ratioFrames.ToList();
                        passedRatioFrames = _ratioFrames.TakeLast(secs).ToList();

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

                StateHasChanged();
            }
        }



        string currentPrice;

        bool smooth = true;

        static double sumBuys = 0;
        static double sumSells = 0;

        private string buysField = "buysTradeSizes";

        private string _buysField = "buysTradeSizes";
        [Parameter]
        public string initialBuysField
        {
            get { return _buysField; }
            set
            {
                _buysField = value;
                buysField = value;
                sellsField = _buysField.Replace("buys", "sells").Replace("bids", "asks");

            }
        }

        string sellsField = "sellsTradeSizes";

        string buysTitle = $"Buys - {sumBuys}";
        string sellsTitle = $"Sells - {sumSells}";

        bool dialogIsOpen = false;
        bool priceDialogIsOpen = false;

        MatChip[] selectedBuysChips = null;
        MatChip selectedBuyChip = null;

        MatChip[] selectedSellsChips = null;
        MatChip selectedSellChip = null;

        int ratiosBack = TDABook.ratiosBack;
        int ratiosDepth = TDABook.ratiosDepth;
        DateTime? endTime = TDABook.endTime;
        DateTime? startTime = TDABook.startTime;
        bool? isCurrentEndTime = TDABook.isCurrentEndTime;
        void OnChange(object value, string name, string format)
        {
            switch (name)
            {
                case "Start Time":
                    startTime = value as DateTime?;
                    break;

                case "End Time":
                    endTime = value as DateTime?;

                    break;
                case "End Time Current":
                    isCurrentEndTime = value as bool?;
                    break;
            }
        }

        string chartTitle()
        {
            return string.Join(" ", buysField.SplitCamelCase().Split(" ").Skip(1));
        }
        void OpenDialog()
        {
            dialogIsOpen = true;
        }

        void OkClick()
        {

            TDABook.ratiosDepth = ratiosDepth;
            TDABook.ratiosBack = ratiosBack;
            TDABook.endTime = endTime;
            TDABook.startTime = startTime;
            TDABook.isCurrentEndTime = isCurrentEndTime;

            dialogIsOpen = false;
        }

        void OpenPriceDialog()
        {
            priceDialogIsOpen = true;
        }

        void OkPriceClick()
        {
            priceDialogIsOpen = false;
        }

        string FormatAsUSD(object value)
        {
            if ((double)value < 1)
                return ((double)value).ToString("n2", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            else
                return ((double)value).ToString("n0", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

        }

        string ProcessChips()
        {
            foreach (var chip in selectedBuysChips ?? new MatChip[0])
            {
                if ((chip.IsSelected))
                {
                    buysField = (string)chip.Value;
                    sellsField = buysField.Replace("buys", "sells").Replace("bids", "asks"); ;
                }
            }
            return "";
        }

    }
}

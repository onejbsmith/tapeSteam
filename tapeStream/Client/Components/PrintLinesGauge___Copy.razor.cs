
using tapeStream.Client.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using tapeStream.Shared;

namespace tapeStream.Client.Components
{
    public partial class PrintLinesGauge___Copy
    {
        DataItem[] rawGaugesCombined = new DataItem[] {
            new DataItem
            {
                Date = DateTime.Parse("2019-12-01"),
                Revenue = 6
            }
        };
        //[CascadingParameter]



        private Dictionary<DateTime, double> _gaugeValues= new Dictionary<DateTime, double>();
        [Parameter]
        public Dictionary<DateTime, double> gaugeValues
        {
            get { return _gaugeValues; }
            set { _gaugeValues = value;
                getPrintsData();
            }
        }


        DataItem[] average10min, movingAverage5min, movingAverage30sec,
            staticValue7, staticValue0, staticValueMinus7
            = new DataItem[]
        {
            new DataItem
            {
                Date = DateTime.Parse("2019-01-01"),
                Revenue = 0
            }
        };
        protected override async Task OnInitializedAsync()
        {
            //TDAStreamerData.OnTimeSalesStatusChanged += getPrintsData;



            await Task.CompletedTask;
        }

        public void getPrintsData()
        {
            if (gaugeValues.Count() == 0) return;
            //if (moduloPrints == 0 || TDAStreamerData.timeSales[symbol].Count() % moduloPrints != 0) return;

            rawGaugesCombined = gaugeValues.Select(dict =>
            new DataItem()
            {
                Date = dict.Key,
                Revenue = dict.Value
            }
            ).ToArray();

            staticValue0 = staticValue(0);
            staticValue7 = staticValue(7);
            staticValueMinus7 = staticValue(-7);

            movingAverage5min = movingAverage(300);

            average10min = staticAverage(600);

            movingAverage30sec = movingAverage(30);

            StateHasChanged();
        }

        private DataItem[] staticAverage(int secs)
        {
            var maxDate = gaugeValues.Keys.Max();
            var avg5min = gaugeValues
                .Where(d => d.Key >= maxDate.AddSeconds(-secs))
                .Average(d => d.Value);

            return gaugeValues.Select(dict =>
            new DataItem()
            {
                Date = dict.Key,
                Revenue = avg5min
            }
            ).ToArray();
        }
        private DataItem[] staticValue(double val)
        {
            return gaugeValues.Select(dict =>
            new DataItem()
            {
                Date = dict.Key,
                Revenue = val
            }
            ).ToArray();
        }

        private DataItem[] movingAverage(int secs)
        {
            return gaugeValues.Select(dict =>
                 new DataItem()
                 {
                     Date = dict.Key,
                     Revenue = gaugeValues
                     .Where(d => d.Key <= dict.Key && d.Key >= dict.Key.AddSeconds(-secs))
                     .Select(d => d.Value).Average()
                 }
            ).ToArray();
        }

    }


}

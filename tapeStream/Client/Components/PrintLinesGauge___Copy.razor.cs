
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
        static DataItem newDataItem =
            new DataItem()
            {
                Date = DateTime.Parse("2019-12-01"),
                Revenue = 6
            };


        Dictionary<string, DataItem[]> dictAllLinePoints =
                new Dictionary<string, DataItem[]>();  /// need to be initted

        DataItem[] rawGaugesCombined = new DataItem[] { newDataItem };


        private Dictionary<DateTime, double> _gaugeValues = new Dictionary<DateTime, double>();
        [Parameter]
        public Dictionary<DateTime, double> gaugeValues
        {
            get { return _gaugeValues; }
            set
            {
                _gaugeValues = value;
                //getPrintsData();
            }
        }


        protected override async Task OnInitializedAsync()
        {
            //TDAStreamerData.OnTimeSalesStatusChanged += getPrintsData;

            foreach (var name in CONSTANTS.lineNames)
                dictAllLinePoints.Add(name,new DataItem[]{ newDataItem});

        await Task.CompletedTask;
    }



}


}

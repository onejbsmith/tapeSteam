using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tapeStream.Shared;

namespace tapeStream.Client.Components
{
    public partial class PrintLinesGauge___Copy
    {


         private Dictionary<string, DataItem[]> _dictAllLinePoints =
                new Dictionary<string, DataItem[]>();

        [Parameter]
        public  Dictionary<string, DataItem[]> dictAllLinePoints
        {
            get { return _dictAllLinePoints; }
            set { _dictAllLinePoints = value;
                rawGaugesCombined = dictAllLinePoints["rawGaugesCombined"];

            }
        }

 
       DataItem[] rawGaugesCombined = new DataItem[] { CONSTANTS.newDataItem };

        ////protected override async Task OnInitializedAsync()
        //{
        //    //TDAStreamerData.OnTimeSalesStatusChanged += getPrintsData;


        //    await Task.CompletedTask;
        //}



    }


}

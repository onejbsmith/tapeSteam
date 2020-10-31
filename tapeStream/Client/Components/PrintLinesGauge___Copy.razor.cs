using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tapeStream.Shared;

namespace tapeStream.Client.Components
{
    public partial class PrintLinesGauge___Copy
    {


        [Parameter]
        public Dictionary<string, DataItem[]> dictAllLinePoints { get; set; } =
                new Dictionary<string, DataItem[]>();  /// need to be initted

//        DataItem[] rawGaugesCombined = new DataItem[] { CONSTANTS.newDataItem };

        ////protected override async Task OnInitializedAsync()
        //{
        //    //TDAStreamerData.OnTimeSalesStatusChanged += getPrintsData;


        //    await Task.CompletedTask;
        //}



    }


}


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

        //[CascadingParameter]



        private Dictionary<DateTime, double> _gaugeValues= new Dictionary<DateTime, double>();
        [Parameter]
        public Dictionary<DateTime, double> gaugeValues
        {
            get { return _gaugeValues; }
            set { _gaugeValues = value;
                //getPrintsData();
            }
        }


        protected override async Task OnInitializedAsync()
        {
            //TDAStreamerData.OnTimeSalesStatusChanged += getPrintsData;



            await Task.CompletedTask;
        }



    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using tapeStream.Shared;

namespace tdaStreamHub.Components
{
    /// TODO: 1. Make Hub a Shared Component
    public partial class HubMonitor
    {
        static string clockFormat = CONSTANTS.clockFormat;
        readonly string[] valuesName = CONSTANTS.valuesName;

        protected override async Task OnInitializedAsync()
        {
            logHub = logHub0;
        }

        void logHubChange(bool? arg, string comment)
        {
            if (arg == false)
            {
                logTopics = "";
                StateHasChanged();
            }
        }

    }
}
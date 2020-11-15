using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using tapeStream.Server.Data;
using tapeStream.Server.Data.classes;

namespace tapeStream.Server.Pages
{
    public partial class Simulator
    {
        List<string> lstFeedDates = new List<string>();
        List<string> lstFeedTimes = new List<string>();

        string startTime = "9:30 AM";
        string endTime = "4:00 PM";
        protected SimulatorSettings simulatorSettings { get; set; } = new SimulatorSettings();


        protected override async Task OnInitializedAsync()
        {
            /// Fill in the dropdown of Available Feed Dates
            lstFeedDates = FilesManager.GetFeedDates();
            await Task.CompletedTask;
        }

        void FeedDateChange(object value, string name)
        {
            /// Got the Run Date
            simulatorSettings.runDate = Convert.ToDateTime(value);
            simulatorSettings.startTime = Convert.ToDateTime(startTime);
            simulatorSettings.endTime = Convert.ToDateTime(endTime);

            lstFeedTimes = FilesManager.GetFeedTimes(simulatorSettings.runDate);

            StateHasChanged();
        }

        void StartTimeChange(object value, string name)
        {
            /// Got the Run Date
            simulatorSettings.startTime = Convert.ToDateTime(value);
            StateHasChanged();
        }

        void EndTimeChange(object value, string name)
        {
            /// Got the Run Date
            simulatorSettings.endTime = Convert.ToDateTime(value);

            StateHasChanged();
        }
    }
}

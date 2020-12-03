using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using tapeStream.Server.Data;
using tapeStream.Server.Data.classes;
using Microsoft.AspNetCore.Components;

namespace tapeStream.Server.Pages
{
    public partial class Simulator
    {


        public string symbol { get; set; } = "QQQ";

        List<string> lstFeedDates = new List<string>();
        List<string> lstFeedTimes = new List<string>();

        string startTime = "9:30 AM";
        string endTime = "4:00 PM";
        protected SimulatorSettings simulatorSettings { get; set; } = new SimulatorSettings();


        protected override async Task OnInitializedAsync()
        {
            /// Fill in the dropdown of Available Feed Dates
            lstFeedDates = FilesManager.GetFeedDates(symbol).OrderByDescending(fileDate=>fileDate).ToList();
            await Task.CompletedTask;
        }

        void FeedDateChange( object value, string name, string symbol)
        {
            /// Got the Run Date
            var dateVal = value.ToString().Split('(')[0].Trim();
            simulatorSettings.runDate = dateVal;
            simulatorSettings.runDateDate = Convert.ToDateTime(dateVal);
            simulatorSettings.startTime = Convert.ToDateTime(startTime);
            simulatorSettings.endTime = Convert.ToDateTime(endTime);

            TDAStreamerData.simulatorSettings = simulatorSettings;

            //JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.simulatorSettings, "TDAStreamerData.simulatorSettings1");
            lstFeedTimes = FilesManager.GetFeedTimes(symbol, dateVal);

            StateHasChanged();
        }

        void StartTimeChange(object value, string name)
        {
            /// Got the Run Date
            simulatorSettings.startTime = Convert.ToDateTime(value);
            TDAStreamerData.simulatorSettings = simulatorSettings;
            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.simulatorSettings, "TDAStreamerData.simulatorSettings2");

            StateHasChanged();
        }

        void EndTimeChange(object value, string name)
        {
            /// Got the Run Date
            simulatorSettings.endTime = Convert.ToDateTime(value);
            TDAStreamerData.simulatorSettings = simulatorSettings;
            JsConsole.JsConsole.GroupTable(TDAStreamerData.jSRuntime, TDAStreamerData.simulatorSettings, "TDAStreamerData.simulatorSettings3");


            StateHasChanged();
        }
    }
}

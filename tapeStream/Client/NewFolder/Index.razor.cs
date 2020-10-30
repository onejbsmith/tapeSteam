using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using tapeStream.Shared;
using tapeStream.Shared.Data;
using System.Text.Json;

namespace tapeStream.TIMESALE_EQUITY.Pages
{
    public partial class Index
    {
        static int Count = 0;
        static string msg = "";
        static bool snackBarIsOpen = false;



        static string inputFolder = CONSTANTS.messageQinputFolder + CONSTANTS.TIMESALE_EQUITY;

        DirectoryInfo dirInput = new DirectoryInfo(inputFolder);

        protected override async Task OnInitializedAsync()
        {
            /// This is fired by Decode_TimeSales
            TDAStreamerData.OnTimeSalesStatusChanged += sendPrintsData;
        }

        double value = 0;
        Dictionary<int, DataItem[]> dictPies = new Dictionary<int, DataItem[]>();
        void sendPrintsData()
        {
            //if (moduloPrints ==0 || TDAStreamerData.timeSales[symbol].Count() % moduloPrints != 0) return;

            try
            {
                // Only summarize once every new 2 time and sales
                if (TDAStreamerData.timeSales["QQQ"].Count % 2 != 0) return;

                value = TDAPrints.GetPrintsGaugeScore("QQQ", ref dictPies);

                KeyValuePair<DateTime, double> pair =
                    new KeyValuePair<DateTime, double>(DateTime.Now, value);

                var jsonGaugeScore = JsonSerializer.Serialize<KeyValuePair<DateTime, double>>(pair);

                List<DataItem[]> lstPies = new List<DataItem[]>();
                foreach (var key in dictPies.Keys)
                    lstPies.Add(dictPies[key]);

                var jsonPrintsPies = JsonSerializer.Serialize<List<DataItem[]>>(lstPies);

                //Send("GaugeScore", jsonGaugeScore);
                //Send("PrintsPies", jsonPrintsPies);


                //TDAStreamerData.gaugeValues.Add(DateTime.Now, value);
                //TDAStreamerData.gaugeValues.RemoveAll((key, val) => key < DateTime.Now.AddSeconds(-printSeconds.Max()));

                // StateHasChanged();
            }
            catch (Exception e)
            {
                snackBarIsOpen = true;
                msg = e.ToString();
                //StateHasChanged();
            }
        }

        private async Task RunOnClick()
        {
            try
            {
                var filsList = dirInput.GetFiles().OrderBy(f => f.LastWriteTime);
                foreach (var file in filsList)
                {

                    /// Read earliest file from the TIMESALE_EQUITY folder
                    var json = File.ReadAllText(file.FullName);

                    /// Decode the json
                    TDAStreamerData.captureTdaServiceData(json);
                    //if (file.Name.StartsWith(CONSTANTS.TIMESALE_EQUITY))
                    //    TDAStreamerData.Decode_TimeSales(CONSTANTS.TIMESALE_EQUITY, json);
                    //else if (file.Name.StartsWith(CONSTANTS.QUOTE))
                    //    await TDAStreamerData.Decode_Quote(json);

                    /// Move file to Archive
                    var archiveFolder = CONSTANTS.messageQarchiveFolder + CONSTANTS.TIMESALE_EQUITY + @"\";
                    File.Move(file.FullName, archiveFolder + file.Name);

                    /// Create the output
                    /// Save Output to Output folder file
                    /// Send output to SignalR
                    /// 
                    if (TDAStreamerData.timeSales.ContainsKey("QQQ"))
                        Count = TDAStreamerData.timeSales["QQQ"].Count;

                    await Task.Yield();

                    StateHasChanged();
                }
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                snackBarIsOpen = true;
                msg = e.ToString();
                //StateHasChanged();
            }

        }

        void ResetArchive()
        {
            try
            {
                var destinationDirectory = CONSTANTS.messageQinputFolder + CONSTANTS.TIMESALE_EQUITY;
                var sourceDirectory = CONSTANTS.messageQarchiveFolder + CONSTANTS.TIMESALE_EQUITY;

                Directory.Delete(destinationDirectory);
                Directory.Move(sourceDirectory, destinationDirectory);
                Directory.CreateDirectory(sourceDirectory);

                TDAStreamerData.Reset();
            }
            catch (Exception e)
            {
                snackBarIsOpen = true;
                msg = e.ToString();
                //StateHasChanged();

            }


        }

        bool FilesInDestFolder()
        {
            dirInput = new DirectoryInfo(inputFolder);
            return dirInput.GetFiles().Length > 0;
        }
    }
}

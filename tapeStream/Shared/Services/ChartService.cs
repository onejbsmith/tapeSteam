﻿#define dev
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Shared.Data;

namespace tapeStream.Shared.Services
{
    public class ChartService

    {
        [Inject]
        static IConfiguration Configuration { get; set; }

        [Inject] HttpClient Http { get; set; } = new HttpClient();

        //#if dev
        //        string controllerUrl = "http://localhost:55540/api/Charts/";
        //#else
        //        string controllerUrl = "http://tapestreamserver.com/api/Charts/";
        //#endif

        string controllerUrl;

        public async Task<TDAChart.Bollinger> getBollingerBands()
        {
            try
            {
                string serverUrl = Configuration["ServerUrl"];
                controllerUrl = $"{serverUrl}api/Charts/";

                await Task.Yield();
                var x = await Http.GetFromJsonAsync<TDAChart.Bollinger>(controllerUrl);
                return x;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
            return new TDAChart.Bollinger();
        }


        public async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData(int seconds)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/Charts/";

            await Task.Yield();
            Dictionary<string, BookDataItem[]> values = CONSTANTS.newBookColumnsData;
            try
            {
                if (seconds == 0)
                    values = await Http.GetFromJsonAsync<Dictionary<string, BookDataItem[]>>(controllerUrl);
                else
                    values = await Http.GetFromJsonAsync<Dictionary<string, BookDataItem[]>>(controllerUrl + seconds.ToString());
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
            return values;
        }

        public async Task<TDAChart.Chart_Content> GetTDAChartLastCandle(int input)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/Charts/";

            await Task.Yield();
            var json = await Http.GetStringAsync(controllerUrl + input.ToString());
            var chartEntry = new TDAChart.Chart_Content();
            try
            {
                chartEntry = JsonSerializer.Deserialize<TDAChart.Chart_Content>(json);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Check that tapeStream Server {controllerUrl} is running.", ex);
            }
            return chartEntry;
        }
        public async Task<string> GetSvcDate()
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/Charts/";

            return await Http.GetStringAsync(controllerUrl + "getSvcDateTime");
        }
    }
}
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using tapeStream.Shared.Data;

namespace tapeStream.Shared.Services
{
    public class ChartService

    {
        [Inject] HttpClient Http { get; set; } = new HttpClient();

        string controllerUrl = "http://localhost:55540/api/Charts/";
        //string controllerUrl = "http://tapestream.com/api/Charts/";
        public async Task<TDAChart.Bollinger> getBollingerBands()
        {
            try
            {
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

        public async Task<string> GetValue(int input)
        {
            var value = await Http.GetStringAsync(controllerUrl + input.ToString());
            return value;
        }

    }
}
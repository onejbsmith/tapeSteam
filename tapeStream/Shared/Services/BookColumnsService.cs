using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using tapeStream.Shared.Data;

namespace tapeStream.Shared.Services
{
    public class BookColumnsService
    {
        [Inject] HttpClient Http { get; set; } = new HttpClient();

        //string controllerUrl = "http://localhost:55540/api/BookColumns/";
        string controllerUrl = "http://tapestream.com/api/BookColumns/";
        public async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData()
        {
            Dictionary<string, BookDataItem[]> values = CONSTANTS.newBookColumnsData;
            try
            {
                values = await Http.GetFromJsonAsync<Dictionary<string, BookDataItem[]>>(controllerUrl);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
            return values;
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

        public async Task<AverageSizes> getAverages(int seconds, IJSRuntime jSRuntime)
        {
            AverageSizes values = new AverageSizes();

#if tracing
            JSRuntimeExtensions.GroupTable(jSRuntime, values, "new AverageSizes");
#endif

            values = await Http.GetFromJsonAsync<AverageSizes>($"{controllerUrl}getLtAverages/{seconds}" );

#if tracing

            JSRuntimeExtensions.GroupTable(jSRuntime, values, "AverageSizes values");
#endif

            return values;
        }

        public async Task<string> GetValue(int input)
        {
            var value = await Http.GetStringAsync(controllerUrl + input.ToString());
            return value;
        }

    }
}
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace tapeStream.Shared.Services
{
    public class PrintsLineChartService
    {
        [Inject] HttpClient Http { get; set; } = new HttpClient();

        string controllerUrl = "https://localhost:44367/api/PrintsLineChart/";
        public async Task<string[]> GetValues()
        {
            var values = await Http.GetFromJsonAsync<string[]>(controllerUrl);
            return values;
        }

        /// <summary>
        /// Call the controller from the service
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, DataItem[]>> getPrintsLineChartData(int input)
        {
            var value = await Http.GetFromJsonAsync<Dictionary<string, DataItem[]>>(controllerUrl + input.ToString());
            return value;
        }

    }
}

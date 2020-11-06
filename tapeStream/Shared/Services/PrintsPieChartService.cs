using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace tapeStream.Shared.Services
{
    public class PrintsPieChartService
    {
        [Inject] HttpClient Http { get; set; } = new HttpClient();

        string controllerUrl = "http://localhost:55540/api/PrintsPieChart/";
        public async Task<double> GetPrintsGaugeScore()
        {
            var values = await Http.GetFromJsonAsync<double>(controllerUrl);
            return values;
        }

        public async Task<Dictionary<string, DataItem[]>> GetPrintsPies()
        {
            var value = await Http.GetFromJsonAsync<Dictionary<string, DataItem[]>>(controllerUrl + "1");
            return value;
        }

    }
}

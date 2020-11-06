using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using tapeStream.Shared.Data;

namespace tapeStream.Shared.Services
{
    public class BookPieChartsService
    {
        [Inject] HttpClient Http { get; set; } = new HttpClient();

        string controllerUrl = "http://localhost:55540/api/BookPieCharts/";
        public async Task<Dictionary<string, BookDataItem[]>> getBookPiesData()
        {
            var values = await Http.GetFromJsonAsync<Dictionary<string, BookDataItem[]>>(controllerUrl);
            return values;
        }

        public async Task<BookDataItem[]> getBookCompositePieData(int id)
        {
            var value = await Http.GetFromJsonAsync<BookDataItem[]>(controllerUrl + id.ToString());
            return value;
        }

    }
}

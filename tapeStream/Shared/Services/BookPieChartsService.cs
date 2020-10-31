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

        string controllerUrl = "https://localhost:44367/api/BookPieChart/";
        public async Task<Dictionary<string, List<BookDataItem>>> getBookPiesData()
        {
            var values = await Http.GetFromJsonAsync<Dictionary<string, List<BookDataItem>>>(controllerUrl);
            return values;
        }

        public async Task<BookDataItem[]> getBookCompositePieData(int id)
        {
            var value = await Http.GetFromJsonAsync<BookDataItem[]>(controllerUrl + id.ToString());
            return value;
        }

    }
}

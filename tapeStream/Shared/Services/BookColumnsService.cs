using Microsoft.AspNetCore.Components;
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

        string controllerUrl = "https://localhost:44367/api/BookColumns/";
        public async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData()
        {
            var values = await Http.GetFromJsonAsync<Dictionary<string, BookDataItem[]>>(controllerUrl);
            return values;
        }

        public async Task<string> GetValue(int input)
        {
            var value = await Http.GetStringAsync(controllerUrl + input.ToString());
            return value;
        }

    }
}
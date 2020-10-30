using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace tapeStream.Shared.Services
{
    public class TimeSalesService
    {
        [Inject] HttpClient Http { get; set; } = new HttpClient();
        public async Task<string[]> GetValues()
        {
            var values = await Http.GetFromJsonAsync<string[]>("https://localhost:44367/api/TimeSales");
            return values;
        }

        public async Task<string> GetValue(int input)
        {
            var value = await Http.GetStringAsync($"https://localhost:44367/api/TimeSales/" + input.ToString());
            return value;
        }


    }
}

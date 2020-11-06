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
            var values = await Http.GetFromJsonAsync<string[]>("http://localhost:55540/api/TimeSales");
            return values;
        }

        public async Task<string> GetValue(int input)
        {
            var value = await Http.GetStringAsync($"http://localhost:55540/api/TimeSales/" + input.ToString());
            return value;
        }


    }
}

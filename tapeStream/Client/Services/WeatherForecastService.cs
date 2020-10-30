using System;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Shared;
using System.Net.Http.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using MatBlazor;

namespace tapeStream.Client.Services
{
    public class WeatherForecastService
    {
        [Inject] HttpClient Http { get; set; }
        [Inject] protected IMatToaster Toaster { get; set; }

        public async Task<WeatherForecast[]> RefreshAsync()
        {
            try
            {
                var forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("https://localhost:44367/WeatherForecast");
                return forecasts;
            }
            catch(Exception ex)
            {
                var method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                Toaster.Add(ex.ToString(), MatToastType.Primary, method);

                return new WeatherForecast[5];
            }
        }

        //public async Task<WeatherForecast[]> Refresh()
        //{
        //    Http = new HttpClient();
        //    var it = await Http.GetStringAsync("https://localhost:44367/WeatherForecast");
        //    //var it = resp.Content.ToString();
        //    var x = JArray.Parse(it);
        //    var y = x.ToObject<WeatherForecast[]>();
        //    await Task.CompletedTask;
        //    return y;
        //}
    }
}

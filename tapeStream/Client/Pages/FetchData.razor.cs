using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using tapeStream.Client.Services;
using tapeStream.Shared;
using tapeStream.Shared.Services;

namespace tapeStream.Client.Pages
{
    public partial class FetchData
    {
        private WeatherForecast[] forecasts;

        private string[] values;

        string timeSalesValue;

        private int _sendValue;

        public int sendValue
        {
            get { return _sendValue; }
            set { _sendValue = value; RefreshTimeSalesValue(); }
        }



        [Inject] WeatherForecastService ForecastService { get; set; }

        [Inject] TimeSalesService timeSalesService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //RefreshWeather();
        }

        protected async void RefreshWeather()
        {
            forecasts = await ForecastService.RefreshAsync();
            StateHasChanged();
        }

        protected async void RefreshTimeSales()
        {
            values = await timeSalesService.GetValues();

            StateHasChanged();
        }

        protected async void RefreshTimeSalesValue()
        {
            var it = await timeSalesService.GetValue(sendValue);
            timeSalesValue = it;

            StateHasChanged();
        }

    }

}

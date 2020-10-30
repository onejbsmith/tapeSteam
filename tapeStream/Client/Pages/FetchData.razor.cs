using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using tapeStream.Shared;
using Microsoft.AspNetCore.Components;
using tapeStream.Data;
using Newtonsoft.Json.Linq;
using tapeStream.Client.Services;

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



        [Inject]
        WeatherForecastService ForecastService { get; set; }

        [Inject]
        TimeSalesService timeSalesService { get; set; }

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

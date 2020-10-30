﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;


namespace tapeStream.Shared.Services
{
    public class PrintsPieChartService
    {
        [Inject] HttpClient Http { get; set; } = new HttpClient();

        string controllerUrll = "https://localhost:44367/api/BookBarChart/";
        public async Task<string[]> GetValues()
        {
            var values = await Http.GetFromJsonAsync<string[]>(controllerUrll);
            return values;
        }

        public async Task<string> GetValue(int input)
        {
            var value = await Http.GetStringAsync(controllerUrll + input.ToString());
            return value;
        }

    }
}

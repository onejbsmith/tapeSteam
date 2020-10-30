// other usings
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using MatBlazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using tapeStream.Client.Services;
using tapeStream.Shared.Services;

namespace tapeStream.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services
              .AddBlazorise(options =>
              {
                  options.ChangeTextOnKeyPress = true;
              })
              .AddBootstrapProviders()
              .AddFontAwesomeIcons();

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            builder.Services.AddSingleton<WeatherForecastService>();

            builder.Services.AddSingleton<BookColumnsService>();
            builder.Services.AddSingleton<BookPieChartsService>();

            builder.Services.AddSingleton<PrintsLineChartService>();
            builder.Services.AddSingleton<PrintsPieChartService>();

            builder.Services.AddSingleton<TimeSalesService>();

            builder.Services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.BottomRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 3000;
            });



            builder.RootComponents.Add<App>("app");
            var host = builder.Build();

            host.Services
              .UseBootstrapProviders()
              .UseFontAwesomeIcons();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}

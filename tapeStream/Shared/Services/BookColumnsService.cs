using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Shared.Data;
using Microsoft.Extensions.Configuration;

namespace tapeStream.Shared.Services
{
    public class BookColumnsService
    {
        [Inject]
        static IConfiguration Configuration { get; set; }

        [Inject] HttpClient Http { get; set; } = new HttpClient();

        //#if dev
        //        string controllerUrl = "http://localhost:55540/api/BookColumns/";
        //#else
        //        string controllerUrl = "http://tapestreamserver.com/api/BookColumns/";
        //#endif

        string controllerUrl;

        public async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData()
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/BookColumns/";

            await Task.Yield();
            Dictionary<string, BookDataItem[]> values = CONSTANTS.newBookColumnsData;
            try
            {
                values = await Http.GetFromJsonAsync<Dictionary<string, BookDataItem[]>>(controllerUrl);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
            return values;
        }


        public async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData(int seconds)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/BookColumns/";

            Dictionary<string, BookDataItem[]> values = CONSTANTS.newBookColumnsData;
            try
            {
                await Task.Yield();
                if (seconds == 0)
                    values = await Http.GetFromJsonAsync<Dictionary<string, BookDataItem[]>>(controllerUrl);
                else
                    values = await Http.GetFromJsonAsync<Dictionary<string, BookDataItem[]>>(controllerUrl + seconds.ToString());
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
            return values;
        }

        public async Task<AverageSizes> getAverages(int seconds, IJSRuntime jSRuntime)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/BookColumns/";

            await Task.Yield();
            AverageSizes values = new AverageSizes();

#if tracing
            JSRuntimeExtensions.GroupTable(jSRuntime, values, "new AverageSizes");
#endif

            var url = $"{controllerUrl}getLtAverages/{seconds}";
            try
            {

                values = await Http.GetFromJsonAsync<AverageSizes>(url);

            }
            //catch { }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Check that tapeStream Server {controllerUrl} is running.", ex);
            }
#if tracing

            JSRuntimeExtensions.GroupTable(jSRuntime, values, "AverageSizes values");
#endif

            return values;
        }

        public async Task<AverageSizes> getLtRatios(int seconds, IJSRuntime jSRuntime)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/BookColumns/";

            await Task.Yield();
            AverageSizes values = new AverageSizes();

#if tracing
            JSRuntimeExtensions.GroupTable(jSRuntime, values, "new AverageSizes");
#endif
            try
            {
                values = await Http.GetFromJsonAsync<AverageSizes>($"{controllerUrl}getLtRatios/{seconds}");
            }
            catch { }

#if tracing

            JSRuntimeExtensions.GroupTable(jSRuntime, values, "AverageSizes Ratios values");
#endif

            return values;
        }

        public Task getRatioFrames(int longSeconds, object ratiosDepth, IJSRuntime jsruntime)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<RatioFrame>> getListLtRatios(int seconds, int last, IJSRuntime jSRuntime)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/BookColumns/";

            await Task.Yield();

            var route = $"getListLtRatios/{seconds}/{last}";

            List<RatioFrame> values = new List<RatioFrame>();

#if tracing
            JSRuntimeExtensions.GroupTable(jSRuntime, values, "new AverageSizes");
#endif
            try
            {
                values = await Http.GetFromJsonAsync<List<RatioFrame>>($"{controllerUrl}{route}");
            }
            catch { }

#if tracing

            JSRuntimeExtensions.GroupTable(jSRuntime, values, "AverageSizes Ratios values");
#endif

            return values;
        }
        public async Task<List<RatioFrame>> getRatioFrames(int seconds, int last, IJSRuntime jSRuntime)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/BookColumns/";

            await Task.Yield();

            var route = $"getRatioFrames/{seconds}/{last}";

            List<RatioFrame> values = new List<RatioFrame>();

#if tracing
            JSRuntimeExtensions.GroupTable(jSRuntime, values, "new AverageSizes");
#endif
            try
            {
                values = await Http.GetFromJsonAsync<List<RatioFrame>>($"{controllerUrl}{route}");
            }
            catch { }

#if tracing

            JSRuntimeExtensions.GroupTable(jSRuntime, values, "AverageSizes Ratios values");
#endif

            return values;
        }

        public async Task<RatioFrame> getIncrementalRatioFrames(int seconds, int last, IJSRuntime jSRuntime)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/BookColumns/";

            await Task.Yield();

            var route = $"getIncrementalRatioFrames/{seconds}/{last}";

            RatioFrame values = new RatioFrame();

#if tracing
            JSRuntimeExtensions.GroupTable(jSRuntime, values, "new AverageSizes");
#endif
            try
            {
                values = await Http.GetFromJsonAsync<RatioFrame>($"{controllerUrl}{route}");
            }
            catch { }

#if tracing

            JSRuntimeExtensions.GroupTable(jSRuntime, values, "AverageSizes Ratios values");
#endif

            return values;
        }
        public async Task<List<RatioFrame[]>> getAllRatioFrames(string symbol, double OADate, IJSRuntime jSRuntime)
        {
            string serverUrl = Configuration["ServerUrl"];
            controllerUrl = $"{serverUrl}api/BookColumns/";

            await Task.Yield();

            var route = $"getAllRatioFrames/{symbol}/{OADate}";

            List<RatioFrame[]> values = new List<RatioFrame[]>();

#if tracing
            JSRuntimeExtensions.GroupTable(jSRuntime, values, "new AverageSizes");
#endif
            try
            {
                var json = await Http.GetFromJsonAsync<string>($"{controllerUrl}{route}");
                values = JsonSerializer.Deserialize<List<RatioFrame[]>>(json);
            }
            catch (System.Exception ex)
            {
            }

#if tracing

            JSRuntimeExtensions.GroupTable(jSRuntime, values, "AverageSizes Ratios values");
#endif

            return values;
        }
    }
}
#undef dev
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

#if dev
        string controllerUrl = "http://localhost:55540/api/BookColumns/";
#else
        string controllerUrl = "http://tapestreamserver.com/api/BookColumns/";
#endif 
        public async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData()
        {
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
            Dictionary<string, BookDataItem[]> values = CONSTANTS.newBookColumnsData;
            try
            {
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

        public async Task<AverageSizes> getRatios(int seconds, IJSRuntime jSRuntime)
        {
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

            var route = $"getListLtRatios/{seconds}/{last}";

            List<RatioFrame>  values = new List<RatioFrame>();

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

            var route = $"getRatioFrames/{seconds}/{last}";

            List<RatioFrame>  values = new List<RatioFrame>();

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
    }
}
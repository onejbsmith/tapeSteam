using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Client.Components;
using tapeStream.Client.Components.HighCharts;
using tapeStream.Shared.Data;

namespace tapeStream.Client.Managers
{
    public class Surface3dManager
    {

        internal static Surface.StackedColumns3DSurface chart = new Surface.StackedColumns3DSurface();

        internal static decimal avgPrice = 0;

        private static int _height = 600;
        public static int height
        {
            get { return _height; }
            set
            {
                _height = value;
                //jsruntime.InvokeAsync<string>("setChartHeight", new object[] { _height });
            }
        }

        static string[] saPairs = new string[] { "0,0", "90,0", "0,90", "0,-90", "0,180", "360,-1", "360,360" };

        internal static List<string> lstPrices = new List<string>();

        internal static double maxSize;


        internal static void Initialize(string jsonResponse)
        {
            chart = JsonSerializer.Deserialize<Surface.StackedColumns3DSurface>(jsonResponse);
#if tracing
            await jsruntime.InvokeVoidAsync("Dump", chart.Dumps(), "chart");
#endif
            /// We set some static chart Properties here and pass back to js
            chart.title.text = "Surface";
            chart.chart.options3d.enabled = true;
            chart.yAxis.title.text = "Size";
            chart.xAxis.title.text = "Price";
            //chart.plotOptions.series.pointWidth = 100;
            chart.chart.backgroundColor = "darkgray";
        }


        internal static void PreSetProperties()
        {
            chart.yAxis.max = SurfaceChartConfigurator.yAxisHigh;


            chart.chart.options3d.alpha = SurfaceChartConfigurator.alpha;
            chart.chart.options3d.beta = SurfaceChartConfigurator.beta;
            chart.chart.options3d.depth = SurfaceChartConfigurator.chartDepth;
            chart.plotOptions.series.depth = SurfaceChartConfigurator.seriesDepth;
            chart.zAxis.max = chart.chart.options3d.depth / chart.plotOptions.series.depth;
            chart.chart.height = height;

            int n = Convert.ToInt16(SurfaceChartConfigurator.chipValue);
            string vals = saPairs[n];
            var alpha = Convert.ToInt16(vals.Split(',')[0]);
            var beta = Convert.ToInt16(vals.Split(',')[1]);
            if (alpha != 360)
            {
                chart.chart.options3d.alpha = alpha;
                chart.chart.options3d.beta = beta;
            }
            else if (beta == -1)
            {
                chart.chart.options3d.alpha = SurfaceChartConfigurator.alpha;
                chart.chart.options3d.beta = -SurfaceChartConfigurator.beta;
            }
            else
            {
                chart.chart.options3d.alpha = SurfaceChartConfigurator.alpha;
                chart.chart.options3d.beta = SurfaceChartConfigurator.beta;
            }
        }

        internal static void InitializePrices(Blazored.LocalStorage.ISyncLocalStorageService localStorage, string id)
        {
            var lst = localStorage.GetItem<List<string>>(id + "lstPrices");
            if (lst != null && lst.Count() > 20)
                lstPrices = lst.Take(20).ToList();
            else
                lstPrices = lst;

            if (lstPrices == null)
                lstPrices = new List<string>();
        }

      internal static void MaintainPriceAxis(Dictionary<string, BookDataItem[]> bookDataItems, Blazored.LocalStorage.ISyncLocalStorageService localStorage, string id)
        {
            var seriesOrder = new string[] { "salesAtBid", "bids", "salesAtAsk", "asks" };

            var minPrice = 999999m; // bookDataItems.Min(items => items.Value.Min(item => item.Price));
            var maxPrice = 0m; // bookDataItems.Max(items => items.Value.Max(item => item.Price));

            if (SurfaceChartConfigurator.resetXAxis)
            {
                lstPrices = new List<string>();
                //
                SurfaceChartConfigurator.resetXAxis = false;
                localStorage.SetItem(id + "lstPrices", "");
            }

            /// Set up the Categories list
            //var lstPrices = new List<string>();
            var sumPrices = 0m;
            var nPrices = 0;

#if tracingFine
            Console.WriteLine("5a. Surface ChartSetData");
#endif
            foreach (var name in seriesOrder)
            {
                //if (dictNumsizes.ContainsKey(name) == false) dictNumsizes[name] = 0;
                //if (dictSumSizes.ContainsKey(name) == false) dictSumSizes[name] = 0;
                ////if (seriesList.Count % 10 == 0)
                ////{
                //dictStNumsizes[name] = 0;
                //dictStSumSizes[name] = 0;
                //}
#if tracingFine
                Console.WriteLine("5b. Surface ChartSetData");
#endif
                foreach (var item in bookDataItems[name])
                {
#if tracingFine
                    Console.WriteLine("5c. Surface ChartSetData");

                    jsruntime.GroupTable(lstPrices, nameof(lstPrices));
                    jsruntime.GroupTable(item, nameof(item));
#endif

                    if (!lstPrices.Contains(item.Price.ToString("n2")))
                        lstPrices.Add(item.Price.ToString("n2"));
#if tracingFine
                    Console.WriteLine("5d. Surface ChartSetData");
#endif
                    sumPrices += item.Price;
#if tracingFine
                    Console.WriteLine("5e. Surface ChartSetData");
#endif
                    nPrices += 1;
                    minPrice = Math.Min(item.Price, minPrice);
                    maxPrice = Math.Max(item.Price, maxPrice);

#if tracingFine
                    Console.WriteLine("5f. Surface ChartSetData");
#endif
                    maxSize = Math.Max(item.Size, maxSize);
#if tracingFine
                    Console.WriteLine("5g. Surface ChartSetData");
#endif
                    //dictNumsizes[name] += 1;
                    //dictSumSizes[name] += item.Size;
                    //if (dictNumsizes[name] > 0)
                    //    dictAvgSizes[name] = dictSumSizes[name] / dictNumsizes[name];
#if tracingFine
                    Console.WriteLine("5h. Surface ChartSetData");
#endif
                    //dictStNumsizes[name] += 1;
                    //dictStSumSizes[name] += item.Size;
                    //if (dictStNumsizes[name] > 0)
                    //    dictStAvgSizes[name] = dictStSumSizes[name] / dictStNumsizes[name];
                }
#if tracingFine
                Console.WriteLine("5i. Surface ChartSetData");
#endif
            }


            //jsruntime.Confirm(seriesList.Count.ToString());

            avgPrice = sumPrices / nPrices;
            lstPrices.Sort();
#if tracingFine
            jsruntime.GroupTable(lstPrices, nameof(lstPrices));
#endif
            ////jsruntime.Confirm(nameof(lstPrices), true);
            localStorage.SetItem(id + "lstPrices", lstPrices);

            //jsruntime.Confirm(lstPrices.Count.ToString());

            //jsruntime.InvokeVoidAsync("Dump", seriesList.Dumps(), "seriesList");

            //foreach(var series in seriesList)
            //{
            //    foreach(var item in series.data)
            //    {
            //        minX = Math.Min((double)item.x, (double)minX);
            //        maxX = Math.Max((double)item.x, (double)maxX);

            //    }
            //}
            //jsruntime.InvokeVoidAsync("Dump", minX.Dumps(), "minX");
            //jsruntime.InvokeVoidAsync("Dump", maxX.Dumps(), "maxX");

            //var lst = new List<string>();//  lstPrices.Where(it =>  && Convert.ToDouble(it) < maxX + 0.05).ToArray();
            //foreach(var price in lstPrices)
            //{
            //    if (Convert.ToDouble(price) > minX - 0.05)
            //        lst.Add(price);
            //   else if (Convert.ToDouble(price) < maxX + 0.05)
            //        lst.Add(price);
            //}
            //jsruntime.InvokeVoidAsync("Dump", lst.Dumps(), "lst");


            /// Now remove all above max and beow min
            /// 

            //var midPrice = (minPrice + maxPrice) / 2;
            //var n = SurfaceChartConfigurator.xAxisMaxCategories;
            ///// Cull the list of prices if it's more than 100
            //if (lstPrices.Count > n)
            //{
            //    var avgPrice = (Convert.ToDecimal(lstPrices.First()) + Convert.ToDecimal(lstPrices.Last())) / 2;
            //    if (midPrice < avgPrice)
            //        /// Remove higher prices
            //        lstPrices = lstPrices.ToArray().Take(n).ToList();
            //    else
            //        /// Remove lower prices
            //        lstPrices = lstPrices.ToArray().Skip(lstPrices.Count - n).ToList();
            //}
            /// Picture the spread as two 0 points, one at high bid, one at low ask
        }

    }


}

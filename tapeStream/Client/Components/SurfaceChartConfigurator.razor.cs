using Microsoft.AspNetCore.Components;

namespace tapeStream.Client.Components
{

  public partial class SurfaceChartConfigurator
    {
        public static bool redrawChart;
        public static int xAxisMaxCategories = 100;

        public static int yAxisHigh = 5000;
         
        private int _yAxisMax = yAxisHigh;

        public int yAxisMax
        {
            get { return _yAxisMax; }
            set
            {
                _yAxisMax = value;
                yAxisHigh = _yAxisMax;
            }
        }

        public static bool yAxisMaxAutoReset = true;

        [Parameter]
        public string title
        {
            get { return _title1; }
            set
            {
                _title1 = value;
                title0 = value;
                seconds = seconds + 1;
                seconds = seconds - 1;
            }
        }
        private string _title1;

        public static string title1 { get; set; }
        static string title0;
        private static string showTitle { get; set; }


        public static int seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;

                if (_seconds == 0)
                    showTitle = title0;
                else
                    showTitle = $"{title0} over {seconds} seconds";
            }
        }
        private static int _seconds = 3;

    }
}

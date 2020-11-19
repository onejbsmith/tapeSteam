using Microsoft.AspNetCore.Components;

namespace tapeStream.Client.Components
{

    public partial class SurfaceChartConfigurator
    {
        public static bool resetXAxis = false;

        public static bool redrawChart = true;
        public static int xAxisMaxCategories = 1000;

        public static int yAxisHigh = 15000;

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
        public static bool isFlat = false;

        public static bool isTimeSalesOnly = false;
        public static int alpha { get; set; } = 15;

        public static int beta { get; set; } = 15;

        public static int chartDepth { get; set; } = 2500;

        public static int seriesDepth { get; set; } = 25;
        public static int chipValue { get; set; } = 5;
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
        public static string showTitle { get; set; }

        [Parameter]        
        public string symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
                symbol0 = value;
                seconds = 3;
                StateHasChanged();
            }
        }
        private string _symbol;

        public static string symbol0;

        public static int height { get; set; } = 1200;

        public static int seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;

                if (_seconds == 0)
                    showTitle = $"{symbol0}{title0}";
                else
                    showTitle = $"{symbol0}{title0} over {seconds} seconds";
            }
        }
        private static int _seconds = 0;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Data.highcharts
{
    public class Highcharts3DMarketColumnChartData
    {
        #region HighChart Classes
        public class HighchartChart
        {
            public Chart chart { get; set; }
            public Title title { get; set; }
            public Xaxis xAxis { get; set; }
            public Yaxis yAxis { get; set; }
            public Plotoptions plotOptions { get; set; }
            public Exporting exporting { get; set; }
            public MarketColumnSeries[] series { get; set; }
        }
        public class MarketColumnSeries
        {
            public string name { get; set; }
            public int?[] data { get; set; }
            public string stack { get; set; }
            public string color { get; set; }
        }

        public class Chart
        {
            public string type { get; set; }
            public Options3d options3d { get; set; }
            public string zoomType { get; set; }
            public bool panning { get; set; }
            public string panKey { get; set; }
        }
        public class Plotoptions
        {
            public Column column { get; set; }
            public Series1 series { get; set; }

        }

        public class Series1
        {
            public int pointWidth { get; set; }
            public bool animation { get; set; }
        }


        public class Exporting
        {
            public Menuitemdefinitions menuItemDefinitions { get; set; }
            public Buttons buttons { get; set; }
        }
        public class Options3d
        {
            public bool enabled { get; set; }
            public int alpha { get; set; }
            public int beta { get; set; }
            public int viewDistance { get; set; }
            public int depth { get; set; }
        }

        public class Title
        {
            public string text { get; set; }
        }

        public class Xaxis
        {
            public string[] categories { get; set; }
            public Labels labels { get; set; }
        }

        public class Labels
        {
            public bool skew3d { get; set; }
            public Style style { get; set; }
        }

        public class Style
        {
            public string fontSize { get; set; }
        }

        public class Yaxis
        {
            public bool allowDecimals { get; set; }
            public int min { get; set; }
            public Title1 title { get; set; }

            public int max { get; set; }

        }

        public class Title1
        {
            public string text { get; set; }
            public bool skew3d { get; set; }
        }

        public class Column
        {
            public string stacking { get; set; }
            public bool grouping { get; set; }
            public int depth { get; set; }
            public int pointWidth { get; set; }
        }


        public class Menuitemdefinitions
        {
            public Label label { get; set; }
        }

        public class Label
        {
            public string text { get; set; }
        }

        public class Buttons
        {
            public Contextbutton contextButton { get; set; }
        }

        public class Contextbutton
        {
            public string[] menuItems { get; set; }
        }
        #endregion

    }
}

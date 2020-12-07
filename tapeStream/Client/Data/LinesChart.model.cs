using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Data
{
    public class LinesChartData
    {

        public class Rootobject
        {
            public Chart chart { get; set; }
            public Title title { get; set; }
            public Subtitle subtitle { get; set; }
            public Xaxi[] xAxis { get; set; }
            public Yaxi1[] yAxis { get; set; }
            public Tooltip tooltip { get; set; }
            public Legend legend { get; set; }
            public Series[] series { get; set; }
            public Plotoptions plotOptions { get; set; }

            public Responsive responsive { get; set; }
        }

        public class Plotoptions
        {
            public Series1 series { get; set; }

           public  Line line { get; set; }

        }

        public class Line
        {
            public long pointInterval { get; set; }
            public long pointStart { get; set; }
        }
        public class Series1
        {
            public bool animation { get; set; }
        }
        public class Chart
        {
            public string zoomType { get; set; }
            public bool animation { get; set; }


        }

        public class Title
        {
            public string text { get; set; }
            public string align { get; set; }
        }

        public class Subtitle
        {
            public string text { get; set; }
            public string align { get; set; }
        }

        public class Tooltip
        {
            public bool shared { get; set; }
        }

        public class Legend
        {
            public string layout { get; set; }
            public string align { get; set; }
            public int x { get; set; }
            public string verticalAlign { get; set; }
            public int y { get; set; }
            public bool floating { get; set; }
            public string backgroundColor { get; set; }
        }

        public class Responsive
        {
            public Rule[] rules { get; set; }
        }

        public class Rule
        {
            public Condition condition { get; set; }
            public Chartoptions chartOptions { get; set; }
            public string _id { get; set; }
        }

        public class Condition
        {
            public int maxWidth { get; set; }
        }

        public class Chartoptions
        {
            public Legend1 legend { get; set; }
            public Yaxi[] yAxis { get; set; }
        }

        public class Legend1
        {
            public bool floating { get; set; }
            public string layout { get; set; }
            public string align { get; set; }
            public string verticalAlign { get; set; }
            public int x { get; set; }
            public int y { get; set; }
        }

        public class Yaxi
        {
            public Labels labels { get; set; }
            public bool showLastLabel { get; set; }
            public bool visible { get; set; }


        }

        public class Labels
        {
            public string align { get; set; }
            public int x { get; set; }
            public int y { get; set; }
        }

        public class Xaxi
        {
            public string[] categories { get; set; }
            public bool crosshair { get; set; }
            public int index { get; set; }
            public bool isX { get; set; }
            public string type { get; set; }
        }

        public class Yaxi1
        {
            public int gridLineWidth { get; set; }
            public Title1 title { get; set; }
            public Labels1 labels { get; set; }
            public bool opposite { get; set; }
            public int index { get; set; }
        }

        public class Title1
        {
            public string text { get; set; }
            public Style style { get; set; }
        }

        public class Style
        {
            public string color { get; set; }
        }

        public class Labels1
        {
            public string format { get; set; }
            public Style1 style { get; set; }
        }

        public class Style1
        {
            public string color { get; set; }
        }

        public class Series
        {
            public string name { get; set; }
            public string color { get; set; }
            public string type { get; set; }
            public int yAxis { get; set; }
            public float[] data { get; set; }
            public Tooltip1 tooltip { get; set; }
            public Marker marker { get; set; }
            public string dashStyle { get; set; }

            public bool showInLegend { get; set; }

        }

        public class Tooltip1
        {
            public string valueSuffix { get; set; }
        }

        public class Marker
        {
            public bool enabled { get; set; }
        }

    }
}

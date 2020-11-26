using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Data
{
    public class ClockGaugeData
    {

        public class HighchartChart
        {
            public Chart chart { get; set; }
            public Credits credits { get; set; }
            public Title title { get; set; }
            public Pane pane { get; set; }
            public Yaxis yAxis { get; set; }
            public Tooltip tooltip { get; set; }
            public Series[] series { get; set; }
        }

        public class Chart
        {
            public string type { get; set; }
            public string backgroundColor { get; set; }
            public object plotBackgroundColor { get; set; }
            public object plotBackgroundImage { get; set; }
            public int plotBorderWidth { get; set; }
            public bool plotShadow { get; set; }
            public string height { get; set; }
        }

        public class Credits
        {
            public bool enabled { get; set; }
        }

        public class Title
        {
            public string text { get; set; }
        }

        public class Pane
        {
            public Background[] background { get; set; }
        }

        public class Background
        {
            public Backgroundcolor backgroundColor { get; set; }
        }

        public class Backgroundcolor
        {
            public Radialgradient radialGradient { get; set; }
            public object[][] stops { get; set; }
        }

        public class Radialgradient
        {
            public float cx { get; set; }
            public float cy { get; set; }
            public float r { get; set; }
        }

        public class Yaxis
        {
            public Labels labels { get; set; }
            public int min { get; set; }
            public int max { get; set; }
            public int lineWidth { get; set; }
            public bool showFirstLabel { get; set; }
            public string minorTickInterval { get; set; }
            public int minorTickWidth { get; set; }
            public int minorTickLength { get; set; }
            public string minorTickPosition { get; set; }
            public int minorGridLineWidth { get; set; }
            public string minorTickColor { get; set; }
            public int tickInterval { get; set; }
            public int tickWidth { get; set; }
            public string tickPosition { get; set; }
            public int tickLength { get; set; }
            public string tickColor { get; set; }
            public Title1 title { get; set; }
        }

        public class Labels
        {
            public int distance { get; set; }
        }

        public class Title1
        {
            public string text { get; set; }
            public Style style { get; set; }
            public int y { get; set; }
        }

        public class Style
        {
            public string color { get; set; }
            public string fontWeight { get; set; }
            public string fontSize { get; set; }
            public string lineHeight { get; set; }
        }

        public class Tooltip
        {
        }

        public class Series
        {
            public Datum[] data { get; set; }
            public bool animation { get; set; }
            public Datalabels dataLabels { get; set; }
        }

        public class Datalabels
        {
            public bool enabled { get; set; }
        }

        public class Datum
        {
            public string id { get; set; }
            public float y { get; set; }
            public Dial dial { get; set; }
            public string color { get; set; }
        }

        public class Dial
        {
            public string radius { get; set; }
            public int baseWidth { get; set; }
            public string baseLength { get; set; }
            public object rearLength { get; set; }
        }

    }
}

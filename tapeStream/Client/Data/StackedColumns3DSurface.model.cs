

namespace tapeStream.Client.Components.HighCharts
{


    public partial class Surface
    {

        public class StackedColumns3DSurface
        {
            public Chart chart { get; set; }
            public Plotoptions plotOptions { get; set; }
            public Title title { get; set; }
            public Subtitle subtitle { get; set; }
            public Yaxis yAxis { get; set; }
            public Xaxis xAxis { get; set; }
            public Zaxis zAxis { get; set; }
            public Series1[] series { get; set; }
        }

        public class Chart
        {
            public string type { get; set; }
            public Options3d options3d { get; set; }
        }

        public class Options3d
        {
            public bool enabled { get; set; }
            public int alpha { get; set; }
            public int beta { get; set; }
            public int depth { get; set; }
            public Panning panning { get; set; }
            public string panKey { get; set; }
            public int viewDistance { get; set; }
            public Frame frame { get; set; }
        }

        public class Panning
        {
            public bool enabled { get; set; }
            public string type { get; set; }
        }

        public class Frame
        {
            public Bottom bottom { get; set; }
        }

        public class Bottom
        {
            public int size { get; set; }
            public string color { get; set; }
        }

        public class Plotoptions
        {
            public Series series { get; set; }
        }

        public class Series
        {
            public int groupZPadding { get; set; }
            public int depth { get; set; }
            public int groupPadding { get; set; }
            public bool grouping { get; set; }
        }

        public class Title
        {
            public string text { get; set; }
        }

        public class Subtitle
        {
            public string text { get; set; }
        }


        public class Title1
        {
            public string text { get; set; }
        }

        public class Xaxis
        {
            public Title2 title { get; set; }
            public double min { get; set; }
            public double max { get; set; }
        }
        public class Yaxis
        {
            public Title1 title { get; set; }
            public double min { get; set; }
            public double max { get; set; }
        }



        public class Zaxis
        {
            public double min { get; set; }
            public double max { get; set; }
            public string[] categories { get; set; }
            public Labels labels { get; set; }
        }
        public class Title2
        {
            public string text { get; set; }
            public bool skew3d { get; set; }
        }
        public class Labels
        {
            public double y { get; set; }
            public int rotation { get; set; }
        }

        //public class Series1
        //{
        //    public int[][] data { get; set; }
        //}

        public class Series1
        {
            public Datum[] data { get; set; }
            public string color { get; set; }
            public bool showInLegend { get; set; }
        }

        public class Datum
        {
            public double? x { get; set; }
            public double? y { get; set; }
            public string color { get; set; }
        }


    }





}

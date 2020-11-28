

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
            public string zoomType { get; set; }
            public string backgroundColor { get; set; }
            public bool panning { get; set; }
            public bool animation { get; set; } = false;
            public string panKey { get; set; }
            public Options3d options3d { get; set; }
            public int height { get; set; }
            public Annotation[] annotations { get; set; }

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

            public Column column  { get; set; }
        }

        public class Column
        {
            public string stacking { get; set; }
            public bool colorByPoint { get; set; }

            public bool grouping { get; set; }
            public int depth { get; set; }

            public Animation animation { get; set; }

        }

        public class Animation
        {
            public int defer { get; set; }
            public int duration { get; set; }
        }
        public class Series
        {
            public int groupZPadding { get; set; }
            public int depth { get; set; }
            public int groupPadding { get; set; }
            public bool grouping { get; set; }
            public string stacking { get; set; }

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
             public string[] categories { get; set; }
           public Title2 title { get; set; }
            //public double min { get; set; }
            //public double max { get; set; }
            public Plotband[] plotBands { get; set; }
            public Plotline[] plotLines { get; set; }
            public bool reversed { get; set; }
        }
        public class Yaxis
        {
            public Title1 title { get; set; }
            public double min { get; set; }
            public double max { get; set; }
            public Plotband[] plotBands { get; set; }
            public Plotline[] plotLines { get; set; }
            public bool reversed { get; set; }
        }



        public class Zaxis
        {
            public bool reversed { get; set; }
            public double min { get; set; }
            public double max { get; set; }
            public Labels labels { get; set; }
        }

        public class Plotline
        {
            public string color { get; set; }
            public string dashStyle { get; set; }
            public decimal value { get; set; }
            public int width { get; set; }
            public int zIndex { get; set; }

        }

        public class Plotband
        {
            public string color { get; set; }
            public decimal from { get; set; }
            public decimal to { get; set; }
            public Label label { get; set; }
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
            public Datum?[] data { get; set; }
            public string color { get; set; }
            public bool showInLegend { get; set; }
            public bool selected { get; set; }
        }

        public class Datum
        {
            public double? x { get; set; }
            public double? y { get; set; }
            public int? z { get; set; }
            public string color { get; set; }
        }

        public class Annotation
        {
            public bool visible { get; set; }
            public Animation animation { get; set; }
            public string draggable { get; set; }
            public Labeloptions labelOptions { get; set; }
            public Shapeoptions shapeOptions { get; set; }
            public Controlpointoptions controlPointOptions { get; set; }
            public Events1 events { get; set; }
            public int zIndex { get; set; }
            public LabelA[] labels { get; set; }
        }

        public class Labeloptions
        {
            public string align { get; set; }
            public bool allowOverlap { get; set; }
            public string backgroundColor { get; set; }
            public string borderColor { get; set; }
            public int borderRadius { get; set; }
            public int borderWidth { get; set; }
            public string className { get; set; }
            public bool crop { get; set; }
            public bool includeInDataExport { get; set; }
            public string overflow { get; set; }
            public int padding { get; set; }
            public bool shadow { get; set; }
            public string shape { get; set; }
            public Style style { get; set; }
            public bool useHTML { get; set; }
            public string verticalAlign { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public bool justify { get; set; }
        }

        public class Style
        {
            public string fontSize { get; set; }
            public string fontWeight { get; set; }
            public string color { get; set; }
            public string textOutline { get; set; }
        }

        public class Shapeoptions
        {
            public string stroke { get; set; }
            public int strokeWidth { get; set; }
            public string fill { get; set; }
            public int r { get; set; }
            public int snap { get; set; }
        }

        public class Controlpointoptions
        {
            public string symbol { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public Style1 style { get; set; }
            public bool visible { get; set; }
            public Events events { get; set; }
        }

        public class Style1
        {
            public string stroke { get; set; }
            public int strokewidth { get; set; }
            public string fill { get; set; }
        }

        public class Events
        {
        }

        public class Events1
        {
        }

        public class LabelA
        {
            public Point point { get; set; }
            public string text { get; set; }
            public string align { get; set; }
            public bool allowOverlap { get; set; }
            public string backgroundColor { get; set; }
            public string borderColor { get; set; }
            public int borderRadius { get; set; }
            public int borderWidth { get; set; }
            public string className { get; set; }
            public bool crop { get; set; }
            public bool includeInDataExport { get; set; }
            public string overflow { get; set; }
            public int padding { get; set; }
            public bool shadow { get; set; }
            public string shape { get; set; }
            public Style2 style { get; set; }
            public bool useHTML { get; set; }
            public string verticalAlign { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public Controlpointoptions1 controlPointOptions { get; set; }
            public bool justify { get; set; }
        }

        public class Point
        {
            public int xAxis { get; set; }
            public int yAxis { get; set; }
            public float x { get; set; }
            public int y { get; set; }
        }

        public class Style2
        {
            public string fontSize { get; set; }
            public string fontWeight { get; set; }
            public string color { get; set; }
            public string textOutline { get; set; }
        }

        public class Controlpointoptions1
        {
            public string symbol { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public Style3 style { get; set; }
            public bool visible { get; set; }
            public Events2 events { get; set; }
        }

        public class Style3
        {
            public string stroke { get; set; }
            public int strokewidth { get; set; }
            public string fill { get; set; }
        }

        public class Events2
        {
        }


    }





}

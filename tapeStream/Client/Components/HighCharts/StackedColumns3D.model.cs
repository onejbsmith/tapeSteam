

namespace tapeStream.Client.Components.HighCharts
{


    #region HighChart.Chart classes
    public class StackedColumns3DChart
    {
        public Chart chart { get; set; }
        public Plotoptions plotOptions { get; set; }
        public Title title { get; set; }
        public Xaxis xAxis { get; set; }
        public Yaxis yAxis { get; set; }
        public Series1[] series { get; set; }
    }

    public class Chart
    {
        public string type { get; set; }
        public string zoomType { get; set; }
        public string backgroundColor { get; set; }
        public bool panning { get; set; }
        public string panKey { get; set; }
        public Options3d options3d { get; set; }
    }

    public class Options3d
    {
        public bool enabled { get; set; }
        public int alpha { get; set; }
        public int beta { get; set; }
        public int viewDistance { get; set; }
        public int depth { get; set; }
    }

    public class Plotoptions
    {
        public Column column { get; set; }
        public Series series { get; set; }
    }

    public class Column
    {
        public string stacking { get; set; }
        public bool grouping { get; set; }
        public int depth { get; set; }
    }

    public class Series
    {
        public bool animation { get; set; }
    }

    public class Title
    {
        public string text { get; set; }
    }

    public class Xaxis
    {
        public string[] categories { get; set; }
        public Labels labels { get; set; }
        public Plotband[] plotBands { get; set; }
        public Plotline[] plotLines { get; set; }
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

    public class Plotband
    {
        public string color { get; set; }
        public decimal from { get; set; }
        public decimal to { get; set; }
        public Label label { get; set; }
    }

    public class Label
    {
        public string text { get; set; }
        public string align { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Plotline
    {
        public string color { get; set; }
        public string dashStyle { get; set; }
        public decimal value { get; set; }
        public int width { get; set; }
    }

    public class Yaxis
    {
        public bool allowDecimals { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public Title1 title { get; set; }
        public Plotband1[] plotBands { get; set; }
        public Plotline1[] plotLines { get; set; }
    }

    public class Title1
    {
        public string text { get; set; }
        public bool skew3d { get; set; }
    }

    public class Plotband1
    {
        public string color { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public Label1 label { get; set; }
    }

    public class Label1
    {
        public string text { get; set; }
        public string align { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Plotline1
    {
        public string color { get; set; }
        public string dashStyle { get; set; }
        public int value { get; set; }
        public int width { get; set; }
    }

    public class Series1
    {
        public string name { get; set; }
        public int?[] data { get; set; }
        public string color { get; set; }
        public string stack { get; set; }
    }

    #endregion
}

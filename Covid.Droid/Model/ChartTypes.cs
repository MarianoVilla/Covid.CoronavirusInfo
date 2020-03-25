using System.ComponentModel;

namespace Covid.Droid.Model
{
    public enum ChartTypes
    {
        [Description("Líneas")]
        LineChart,
        [Description("Barras")]
        BarChart,
        [Description("Puntos")]
        PointChart
    }
}
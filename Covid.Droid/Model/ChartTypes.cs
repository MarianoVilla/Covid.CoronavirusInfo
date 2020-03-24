﻿using System.ComponentModel;

namespace Covid.Droid.Model
{
    public enum ChartTypes
    {
        [Description("Línea")]
        LineChart,
        [Description("Barras")]
        BarChart,
        [Description("Puntos")]
        PointChart
    }
}
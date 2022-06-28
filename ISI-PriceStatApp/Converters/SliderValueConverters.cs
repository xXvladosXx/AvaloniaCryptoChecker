using System;
using Avalonia.Data.Converters;

namespace ISI_PriceStatApp.Converters;

public static class SliderValueConverters
{
    private static double ToLineChartSmoothnessImpl(double val)
    {
        val = Math.Min(val / 100, 1.0);
        return Math.Max(val, 0);
    }
    
    private static double FromLineChartSmoothnessImpl(double val)
    {
        val = Math.Min(val * 100, 100);
        return Math.Max(val, 0);
    }
    
    public static readonly IValueConverter ChartSmoothnessConverter =
        new FuncValueTwoWayConverter<double, double>(FromLineChartSmoothnessImpl, ToLineChartSmoothnessImpl);
}
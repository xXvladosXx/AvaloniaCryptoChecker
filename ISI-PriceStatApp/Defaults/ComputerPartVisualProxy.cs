using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ISI_PriceStatApp.Defaults;

public class ComputerPartVisualProxy : LineSeriesVisualProxy
{
    public ComputerPartVisualProxy(LineSeries<DateTimePoint> series) : base(series)
    { }
}
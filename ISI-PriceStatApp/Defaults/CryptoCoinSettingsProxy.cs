using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ISI_PriceStatApp.Defaults;

public class CryptoCoinVisualProxy : LineSeriesVisualProxy
{
    public CryptoCoinVisualProxy(LineSeries<DateTimePoint> series) : base(series)
    { }
}
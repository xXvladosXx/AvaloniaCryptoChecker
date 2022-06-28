using System;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

namespace ISI_PriceStatApp.Defaults;

public class LineSeriesVisualProxy : ReactiveObject
{
    private readonly LineSeries<DateTimePoint> _series;

    protected LineSeriesVisualProxy(LineSeries<DateTimePoint> series)
    {
        _series = series;
        Name = series.Name ?? string.Empty;
    }
    
    public string Name { get; }
    
    public SKColor? Color
    {
        get => this._series.Fill is SolidColorPaint c ? c.Color : null;
        set
        {
            if (value is null)
            {
                return;
            }
            
            var pains = new (IPaint<SkiaSharpDrawingContext>?, Action<SolidColorPaint>, Func<SKColor, SKColor>)[]
            {
                (this._series.Fill, newFill => this._series.Fill = newFill, c => new SKColor(c.Red, c.Green, c.Blue, 51)),
                (this._series.Stroke, newFill => this._series.Stroke = newFill, c => new SKColor(c.Red, c.Green, c.Blue, 255)),
                (this._series.GeometryStroke, newFill => this._series.GeometryStroke = newFill, c => new SKColor(c.Red, c.Green, c.Blue, 255)),
            };

            foreach (var (paint, action, colorFunc) in pains)
            {
                if (paint is SolidColorPaint scp)
                {
                    scp = (scp.CloneTask() as SolidColorPaint)!;
                    scp.Color = colorFunc(value.Value);
                    action(scp);
                }
            }
            
            this.RaisePropertyChanged();
        }
    }

    public double LineSmoothness
    {
        get => this._series.LineSmoothness;
        set
        {
            if (value is < 0 or > 1)
            {
                return;
            }
            
            this._series.LineSmoothness = value;
            this.RaisePropertyChanged();
        }
    }

    public bool IsVisible
    {
        get => this._series.IsVisible;
        set
        {
            this._series.IsVisible = value;
            this.RaisePropertyChanging();
        }
    }
}
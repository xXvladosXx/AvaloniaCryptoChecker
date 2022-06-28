using Avalonia.Data.Converters;

namespace ISI_PriceStatApp.Converters;

public static class ColorConverters
{
    private static Avalonia.Media.Color SKColorToAvaloniaMediaColorImpl(SkiaSharp.SKColor color) 
        => new (color.Alpha, color.Red, color.Green, color.Blue);
    
    private static SkiaSharp.SKColor AvaloniaMediaColorTOSKColorImpl(Avalonia.Media.Color color) 
        => new (color.R, color.G, color.B, color.A);

    public static readonly IValueConverter SKColorAvaloniaMediaColorConverter =
        new FuncValueTwoWayConverter<SkiaSharp.SKColor, Avalonia.Media.Color>(SKColorToAvaloniaMediaColorImpl, AvaloniaMediaColorTOSKColorImpl);
    
    private static Avalonia.Media.Color LiveChartsColorToColorPickerColorImpl(SkiaSharp.SKColor color) 
        => new (255, color.Red, color.Green, color.Blue);
    
    private static SkiaSharp.SKColor ColorPickerColorToLiveChartsColorImpl(Avalonia.Media.Color color) 
        => new (color.R, color.G, color.B, color.A);

    public static IValueConverter LiveChartsColorColorPickerConverter =
        new FuncValueTwoWayConverter<SkiaSharp.SKColor, Avalonia.Media.Color>(LiveChartsColorToColorPickerColorImpl, ColorPickerColorToLiveChartsColorImpl);
}
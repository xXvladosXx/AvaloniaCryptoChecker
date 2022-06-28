using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Utilities;

namespace ISI_PriceStatApp.Converters;

public class FuncValueTwoWayConverter<TIn, TOut> : IValueConverter
{
    private readonly Func<TIn, TOut> _convert;
    private readonly Func<TOut, TIn> _convertBack;
    
    public FuncValueTwoWayConverter(Func<TIn, TOut> convert, Func<TOut, TIn> convertBack)
    {
        _convert = convert;
        _convertBack = convertBack;
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => TypeUtilities.CanCast<TIn>(value) && value != null ? _convert((TIn)value)! : AvaloniaProperty.UnsetValue;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => TypeUtilities.CanCast<TOut>(value) && value != null ? _convertBack((TOut)value)! : AvaloniaProperty.UnsetValue;
}